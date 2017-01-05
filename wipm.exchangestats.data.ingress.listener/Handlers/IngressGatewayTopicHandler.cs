using lqd.net.functional;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using wipm.exchangestats.data.ingress.core;
using wipm.exchangestats.infrastrcuture;
using wipm.library.messaging;

namespace wipm.exchangestats.data.ingress.listener {


    class IngressGatewayTopicHandler 
            : TopicHandler {

        public string Name {
            get { return GetType().Name; }
        }

        public void Start() {

            if ( ingressTopicSubscription == null || ingressTopicSubscription.IsClosed ) {

                var connectionString 
                      = ConfigurationManager.AppSettings[ "ingress_gateway_service_bus_connection_string" ];

                ingressTopicSubscription
                  = SubscriptionClient.CreateFromConnectionString(
                         connectionString
                        ,IngressGatewayTopic.Name
                        ,IngressGatewayTopic.Subscriptions.DataIngressListener
                    );

                publishUnpublishedOutcomes(); 
                ingressTopicSubscription.OnMessage( handleMessage );

            } else {
                Trace.TraceInformation( $"{Name} - already started" );
            }

        }

        public void Stop() {

            if ( ingressTopicSubscription != null && !ingressTopicSubscription.IsClosed ) {

                ingressTopicSubscription.Close();
            }
        }


        private void publishUnpublishedOutcomes() {

            Trace.TraceInformation( $"{Name} - started publishing unpublished messages" );

            var serviceDataModel 
                  = new DataModelDbContext();

            try {
                
                var serviceContext 
                      = new ServiceContext(
                           serviceDataModel: serviceDataModel
                          ,serviceCommandFactory: new IngressGatewayCommandFactory()
                          ,dataIngressTopic: new DataIngressTopic() 
                        );


                foreach ( var message in serviceDataModel.GetUnpublishedMessages() ) {

                    Trace.TraceInformation( $"{Name} - [{message.MessageId}] Publishing unpublished messages" );

                    var outcomeContext 
                          = new MessageContext( serviceContext, message );

                    publishOutcome( outcomeContext );

                    Trace.TraceInformation( $"{Name} - [{message.MessageId}] Published unpublished messages" );
                }


            } catch ( Exception e ) {
                Trace.TraceError( $"{Name} - Message : {e.Message}, Stack trace : {e.StackTrace}" );
                throw;

            } finally {
                serviceDataModel.Dispose();
            }

            Trace.TraceInformation( $"{Name} - completed publishing unpublished messages" );

        }


        private static void publishOutcome
                              ( MessageContext outcomeContext ) {

            if ( outcomeContext == null ) throw new ArgumentNullException( nameof( outcomeContext ) );


            // This does not need to be wrapped in transaction because it does
            // not perform any validation that requires querying the database
            // so we are not having to artificially pretend that the world is
            // a stable place.

            outcomeContext                       
              .fmap( publishOutcomeToDataIngressTopic )
              .fmap( identifyMessageAsPublished );

            outcomeContext
              .ServiceContext
              .ServiceDataModel
              .SaveChanges();

        }

        private static MessageContext publishOutcomeToDataIngressTopic
                                        ( MessageContext outcomeContext ) {

            if ( outcomeContext == null ) throw new ArgumentNullException( nameof( outcomeContext ) );


            // post the outcome to the message queue
            var outcomeMessage = new IngressQueueEntry(
                requestId: outcomeContext.Message.RequestId
                // todo:  Change this to use a message identity provider
               ,messageId: Guid.NewGuid() 
               ,message:  outcomeContext.Message.Body
               ,messageType: outcomeContext.Message.MessageType
            );

            outcomeContext
              .ServiceContext
              .DataIngressTopic
              .Dispatch( outcomeMessage );

            return outcomeContext;
        }


        private static MessageContext identifyMessageAsPublished
                                        ( MessageContext outcomeContext ) {

            // update outcome to say that it has been published. 
            outcomeContext
              .ServiceContext
              .ServiceDataModel
              .SetOutcomeToPublished( outcomeContext.Message.MessageId )
              ;

            return outcomeContext;

        }




        private void handleMessage
                       ( BrokeredMessage message ) {

            if ( message == null ) throw new ArgumentNullException( nameof( message ) );


            Trace.TraceInformation( $"{Name} - [{message.MessageId}] Received message" );

            var serviceDataModel
                  = new DataModelDbContext();

            try {
                var messageContext
                      = IngressGatewayTriggerMessageContextFactory
                          .create( message, serviceDataModel );

                // There may be stale messages in the queue if we had a dirty shutdown
                // as such we check each message first to see if it has been processed.
                // 
                // The way the service is written if we have processed a message there
                // will always be a message entry with the same id.
                if ( !messageContext
                        .MessageContext
                        .ServiceContext
                        .ServiceDataModel
                        .HasMessage( messageContext.MessageContext.Message.MessageId ) ) {

                    Trace.TraceInformation( $"{Name} - [{message.MessageId}] Processing message" );

                    // Process message performs any state changes and then 
                    // writes the state changes, the message, and message 
                    // outcome in one transaction.
                    //
                    // Publish outcome post the message outcome to the 
                    // dataIngress topic and then set the message to published.
                    //
                    // This along with publishing unpublished messages when 
                    // starting the service and the above check to see if 
                    // the message has previously been published should 
                    // ensure consistency so long as the message bus technology
                    // can detect duplicate messages which Service bus can.  
                    // It is needed becasue we have two separate components 
                    // ( the database and the Azure Service bus ) and you can 
                    // never tell when the service may fail.
                    messageContext
                      .fmap( processMessage )
                      .fmap( publishOutcome )
                      ;                

                }

                Trace.TraceInformation( $"{Name} - [{message.MessageId}] Completed message" );

            } catch ( Exception e ) {
                Trace.TraceError( $"{Name} - [{message.MessageId}] Error - Message: {e.Message}, Stack trace: {e.StackTrace}" );
                message.Abandon();
                throw;

            } finally {
                serviceDataModel.Dispose();
            }

            

        }


        private static MessageContext processMessage
                                        ( TriggerMessageContext triggerContext ) {

            if ( triggerContext == null ) throw new ArgumentNullException( nameof( triggerContext ) );


            var transaction 
                 = triggerContext
                    .MessageContext
                    .ServiceContext
                    .ServiceDataModel
                    .Database
                    .BeginTransaction( IsolationLevel.Serializable );

            try {

                var outcome
                     = triggerContext                       
                         .fmap( recordMessageInMessageStore )
                         .fmap( processMessageInCore )
                         .fmap( recordMessageOutcome )
                         ;

                triggerContext
                  .MessageContext
                  .ServiceContext
                  .ServiceDataModel
                  .SaveChanges();

                transaction.Commit();

                return outcome;

            } finally {
                transaction.Dispose(); // will roll back the transaction if needed
            }
        }

        private static TriggerMessageContext recordMessageInMessageStore
                                               ( TriggerMessageContext context ) {

            if ( context == null ) throw new ArgumentNullException( nameof( context ) );


            context
              .MessageContext
              .ServiceContext
              .ServiceDataModel
              .WriteMessage( context.MessageContext.Message, context.OutomceMessageId );


            context
              .MessageContext
              .ServiceContext
              .ServiceDataModel
              .SaveChanges();

            return context;
        }

        private static MessageContext processMessageInCore
                                        ( TriggerMessageContext context ) {

            // get the handler for the message
            var serviceCommand
                  = context
                     .MessageContext
                     .ServiceContext
                     .ServiceCommandFactory[ context.MessageContext.Message.MessageType ];

            var serviceRequest 
                  = new ServiceRequest(
                       dataModel: context.MessageContext.ServiceContext.ServiceDataModel
                      ,message: context.MessageContext.Message.Body
                    );

            var serviceResponse
                  = serviceCommand.Handle( serviceRequest );

            // todo: move this to a factory method ( note fix outcomType todo first )
            var outcomeContext
                  = new MessageContext(
                       serviceContext: context.MessageContext.ServiceContext
                      ,message: new Message(
                          requestId: context.MessageContext.Message.RequestId
                         ,messageId: context.OutomceMessageId
                         ,messageType: $"{serviceCommand.GetType().Name}Result"
                         ,message: ServiceResponseJsonConverter.Serialise( serviceResponse )  
                       )
                    );

            return outcomeContext;
        }

        private static MessageContext recordMessageOutcome 
                                        ( MessageContext outcomeContext ) {

            if ( outcomeContext == null  ) throw new ArgumentNullException( nameof( outcomeContext ) );


            outcomeContext
              .ServiceContext
              .ServiceDataModel
              .WriteMessageOutcome( outcomeContext.Message );

            return outcomeContext;
        }


        private SubscriptionClient ingressTopicSubscription;
    }
}
