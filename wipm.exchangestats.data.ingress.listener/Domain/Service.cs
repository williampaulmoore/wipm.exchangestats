using lqd.net.functional;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using System;
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

                if ( !messageContext
                        .ServiceContext
                        .ServiceDataModel
                        .HasMessage( messageContext.Message.RequestId ) ) { 

                    Trace.TraceInformation( $"[{message.MessageId}] Processing message" );

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


            return
              messageContext                       
                .fmap( recordMessageInMessageStore )
                .fmap( processMessageInCore )
                .fmap( recordMessageOutcome )
                .fmap( commitMessage )
                ;

        }

        private static void publishOutcome
                             ( OutcomeContext outcomeContext ) {

            if ( outcomeContext == null ) throw new ArgumentNullException( nameof( outcomeContext ) );
            

            outcomeContext                       
              .fmap( publishOutcomeToDataIngressTopic )
              .fmap( identifyMessageAsPublished )
              .fmap( commitOutcome );
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
                       
        private static OutcomeContext commitMessage
                                       ( OutcomeContext outcomeContext ) {

            if ( outcomeContext == null ) throw new ArgumentNullException( nameof( outcomeContext ) );


            outcomeContext
              .ServiceContext
              .ServiceDataModel
              .Commit();

            return outcomeContext;
        }

        private static OutcomeContext publishOutcomeToDataIngressTopic
                                       ( OutcomeContext outcomeContext ) {

            if ( outcomeContext == null ) throw new ArgumentNullException( nameof( outcomeContext ) );


            // post the outcome to the message queue
            var outcomeMessage = new IngressQueueEntry<string>(
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

        private static void commitOutcome
                             ( OutcomeContext outcomeContext ) {

            if ( outcomeContext == null ) throw new ArgumentNullException( nameof( outcomeContext ) );


            outcomeContext
                .ServiceContext
                .ServiceDataModel
                .Commit();

        }


        private readonly SubscriptionClient messageSourceSubscription;
        private readonly IIngressQueue outcomeSink;
        private readonly ServiceCommandFactory serviceCommandFatory;
        private readonly TopicDescription messageSourceTopic;
    }

}
