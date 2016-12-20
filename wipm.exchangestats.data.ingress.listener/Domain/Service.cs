using lqd.net.functional;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Diagnostics;
using wipm.exchangestats.data.ingress.core;
using wipm.exchangestats.data.ingress.interfaces;
using wipm.library.messaging;

namespace wipm.exchangestats.data.ingress.listener {

    class Service {

        public void Start() {
            Trace.TraceInformation( "Start" );

            try { 
                publishUnpublisheOutcomes();
                messageSourceTopic.Status = EntityStatus.Active;

                Trace.TraceInformation( "Listening for messages" );

            } catch (Exception e )  {
                Trace.TraceError( $"Message: {e.Message}, Stack trace: {e.StackTrace}"  );
                throw;
            }
        }

        public void Stop() {
            Trace.TraceInformation( "Stop" );

            messageSourceTopic.Status = EntityStatus.ReceiveDisabled;
        }

        public Service
                ( TopicDescription messageSourceTopic
                , SubscriptionClient messageSourceSubscription
                , IIngressQueue outcomeSink ) {


            if ( messageSourceTopic == null ) throw new ArgumentNullException( nameof( messageSourceTopic ) );
            if ( messageSourceSubscription == null ) throw new ArgumentNullException( nameof ( messageSourceSubscription ) );
            if ( outcomeSink == null ) throw new ArgumentNullException( nameof( outcomeSink ) );


            this.messageSourceTopic = messageSourceTopic;
            this.messageSourceSubscription = messageSourceSubscription;
            this.outcomeSink = outcomeSink;
            this.serviceCommandFatory = createServiceCommandFactory();


            this.messageSourceTopic.Status = EntityStatus.ReceiveDisabled;
            messageSourceSubscription.OnMessage( run );


        }

        private static ServiceCommandFactory createServiceCommandFactory() {

            var factory 
                  = new ServiceCommandFactory();

            factory.Register( Messages.Exchanges, () => new ExchangeDataReceived() );

            return factory;
        }

        private void run
                       ( BrokeredMessage message  ) {

            if ( message == null ) throw new ArgumentNullException( nameof( message ) );


            Trace.TraceInformation( $"[{message.MessageId}] Received message" );

            var world 
                  = new DataModelDbContext();

            try {
                var messageContext
                      = createMessageContext( message, world, outcomeSink );
                
                // There may be stale messages in the queue if we had a dirty shutdown
                // as such we check each message first to see if it has been processed.
                // 
                // The way the service is written if we have processed a message there
                // will always be a message entry with the same id.

                if ( !messageContext
                        .ServiceContext
                        .ServiceDataModel
                        .HasMessage( messageContext.Message.RequestId ) ) {

                    Trace.TraceInformation( $"[{message.MessageId}] Processing message" );


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
                message.Complete();

                Trace.TraceInformation( $"[{message.MessageId}] Completed message" );

            } catch ( Exception e ) {
                Trace.TraceError( $"[{message.MessageId}] Error - Message: {e.Message}, Stack trace: {e.StackTrace}" );
                throw;

            } finally {
                world.Dispose();
            }
        }
       
        private void publishUnpublisheOutcomes() {

            Trace.TraceInformation( $"Publishing unpublished messages" );

            var dataModel 
                  = new DataModelDbContext();

            try { 

                var serviceContext 
                      = createServiceContext( dataModel, outcomeSink );

                foreach ( var outcome in dataModel.GetUnpublishedOutcomes() ) {

                    Trace.TraceInformation( $"[{outcome.RequestId}] Publishing unpublished messages" );

                    var outcomeContext 
                          = new OutcomeContext( serviceContext, outcome );

                    publishOutcome( outcomeContext );

                    Trace.TraceInformation( $"[{outcome.RequestId}] Published unpublished messages" );
                }

            } catch ( Exception e ) {
                Trace.TraceError( $"Message: {e.Message}, Stack trace: {e.StackTrace}" );
                throw;

            } finally {
                dataModel.Dispose();
            }
        }


        private static MessageContext createMessageContext
                                       ( BrokeredMessage brokeredMessage
                                       , DataModelDbContext serviceDataModel 
                                       , IIngressQueue dataIngressTopic ) {

            if ( brokeredMessage == null ) throw new ArgumentNullException( nameof( brokeredMessage ) );
            if ( serviceDataModel == null ) throw new ArgumentNullException( nameof( serviceDataModel ) );


            var serviceContext 
                  = createServiceContext( serviceDataModel, dataIngressTopic );

            var message 
                 = createMessage( brokeredMessage );

            var messageContext 
                 = new MessageContext ( serviceContext, message );

            return messageContext;
        }

        private static ServiceContext createServiceContext
                                       ( DataModelDbContext serviceDataModel
                                       , IIngressQueue dataIngressTopic ) {

            return new ServiceContext(
               serviceDataModel: serviceDataModel
              ,serviceCommandFactory: createServiceCommandFactory()
              ,dataIngressTopic: dataIngressTopic 
            );

        }

        private static Message createMessage
                                ( BrokeredMessage brokeredMessage ) {

            return new Message(
                requestId:  Guid.Parse( brokeredMessage.MessageId )
               ,message: brokeredMessage.GetBody<string>()
               ,messageType: brokeredMessage.ContentType
            );
        }


        private static OutcomeContext processMessage
                                       ( MessageContext messageContext ) {

            if ( messageContext == null ) throw new ArgumentNullException( nameof( messageContext ) );


            var transaction 
                 = messageContext
                    .ServiceContext
                    .ServiceDataModel
                    .Database
                    .BeginTransaction( IsolationLevel.Serializable );

            try {

                var outcome
                     = messageContext                       
                         .fmap( recordMessageInMessageStore )
                         .fmap( processMessageInCore )
                         .fmap( recordMessageOutcome )
                         ;

                messageContext
                  .ServiceContext
                  .ServiceDataModel
                  .SaveChanges();

                transaction.Commit();

                return outcome;

            } finally {
                transaction.Dispose(); // will roll back the transaction if needed
            }
        }

        private static void publishOutcome
                             ( OutcomeContext outcomeContext ) {

            if ( outcomeContext == null ) throw new ArgumentNullException( nameof( outcomeContext ) );


            // This does not need to be wrapped in transaction because it does
            // not perform any validation that requires querying the database
            // so we are not having to artificially pretend that the world is
            // a stabl place.

            outcomeContext                       
              .fmap( publishOutcomeToDataIngressTopic )
              .fmap( identifyMessageAsPublished );

            outcomeContext
              .ServiceContext
              .ServiceDataModel
              .SaveChanges();

        }


        private static MessageContext recordMessageInMessageStore
                                       ( MessageContext context ) {

            if ( context == null ) throw new ArgumentNullException( nameof( context ) );


            context
              .ServiceContext
              .ServiceDataModel
              .WriteMessage( context.Message );

            return context;
        }


        private static OutcomeContext processMessageInCore
                                       ( MessageContext context ) {

            // get the handler for the message
            var serviceCommand
                  = context.ServiceContext.ServiceCommandFactory[ context.Message.MessageType ];

            var serviceRequest 
                  = createServiceRequest( context );

            var serviceResponse
                  = serviceCommand.Handle( serviceRequest );

            // todo: move this to a factory method ( note fix outcomType todo first )
            var outcomeContext
                  = new OutcomeContext(
                       serviceContext: context.ServiceContext
                      ,messageOutcome: new MessageOutcome(
                          requestId: context.Message.RequestId
                          // todo: logic is needed to preserver the information type for the outcome
                         ,outcomeType: $"{serviceCommand.GetType().Name}Result"
                         ,outcome: JsonConvert.SerializeObject( serviceResponse )  
                       )
                    );

            return outcomeContext;
        }

        private static ServiceRequest createServiceRequest
                                       ( MessageContext messageContext ) {

            return new ServiceRequest(
               dataModel: messageContext.ServiceContext.ServiceDataModel
              ,message: messageContext.Message.Body
            );

        }

        private static OutcomeContext recordMessageOutcome 
                                       ( OutcomeContext outcomeContext ) {

            if ( outcomeContext == null  ) throw new ArgumentNullException( nameof( outcomeContext ) );


            outcomeContext
              .ServiceContext
              .ServiceDataModel
              .WriteMessageOutcome( outcomeContext.MessageOutcome );

            return outcomeContext;
        }
                       
        private static OutcomeContext publishOutcomeToDataIngressTopic
                                       ( OutcomeContext outcomeContext ) {

            if ( outcomeContext == null ) throw new ArgumentNullException( nameof( outcomeContext ) );


            // post the outcome to the message queue
            var outcomeMessage = new IngressQueueEntry(
                RequestId: outcomeContext.MessageOutcome.RequestId
               ,Message:  outcomeContext.MessageOutcome.Outcome 
               ,MessageType: outcomeContext.MessageOutcome.OutcomeType
            );

            outcomeContext
              .ServiceContext
              .DataIngressTopic
              .Dispatch( outcomeMessage );

            return outcomeContext;
        }


        private static OutcomeContext identifyMessageAsPublished
                                       ( OutcomeContext outcomeContext ) {

            // update outcome to say that it has been published. 
            outcomeContext
              .ServiceContext
              .ServiceDataModel
              .SetMessageToPublished( outcomeContext.MessageOutcome.RequestId )
              ;

            return outcomeContext;

        }


        private readonly SubscriptionClient messageSourceSubscription;
        private readonly IIngressQueue outcomeSink;
        private readonly ServiceCommandFactory serviceCommandFatory;
        private readonly TopicDescription messageSourceTopic;
    }

}
