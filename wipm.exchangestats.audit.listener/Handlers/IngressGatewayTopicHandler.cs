using lqd.net.functional;
using System;
using System.Data;
using Microsoft.ServiceBus.Messaging;
using System.Configuration;
using System.Diagnostics;
using wipm.exchangestats.infrastrcuture;
using wipm.exchangestats.audit.core;

namespace wipm.exchangestats.audit.listener {


    class IngressGatewayTopicHandler
            : TopicHandler {

        private const string TopicSourceName = IngressGatewayTopic.Name;
        private const string TopicSubscriptionName = IngressGatewayTopic.Subscriptions.AuditListener;



        public string Name {
            get {
                return this.GetType().Name;
            }
        }

        public void Start() {

            Trace.TraceInformation( "Start" );

            // A new subscription client is created on start and which closed
            // on close.
            try {                 

                if ( ingressTopicSubscription == null || ingressTopicSubscription.IsClosed ) {

                    var connectionString 
                          = ConfigurationManager.AppSettings[ "ingress_gateway_service_bus_connection_string" ];


                    ingressTopicSubscription
                      = SubscriptionClient.CreateFromConnectionString(
                             connectionString
                            ,TopicSourceName
                            ,TopicSubscriptionName
                        );

                    ingressTopicSubscription.OnMessage( handleMessage );


                    Trace.TraceInformation( $"{ this.GetType().Name } - Listening for messages" );
                }


            } catch ( Exception e )  {
                Trace.TraceError( $"{ this.GetType().Name } - Message: { e.Message }, Stack trace: { e.StackTrace }"  );
                throw;
            }

        }

        public void Stop() {

            ingressTopicSubscription.Close();

        }


        public IngressGatewayTopicHandler() {

            serviceCommandFactory = new ServiceCommandFactory();
        }



        private void handleMessage
                       ( BrokeredMessage message ) {

            if ( message == null ) throw new ArgumentNullException( nameof( message ) );


            // When the service is stopped we call the close method on the
            // topic.  The following guard statement is needed because there 
            // can be a delay between calling close on the topic and it 
            // stopping the message pump.

            if (   ingressTopicSubscription != null 
               && !ingressTopicSubscription.IsClosed ) {
                
                Trace.TraceInformation( $"[{message.MessageId}] Received message" );

                var dataModel 
                      = new DataModelDbContext();

                try {

                    Trace.TraceInformation( $"[{message.MessageId}] Processing message" );


                    processMessageInServiceLayer( 
                      dataModel, 
                      message, 
                      serviceCommandFactory 
                    );
                    message.Complete();

                    // Note - The audit domain is a essentially a query domain it
                    //        records informations to allow querying for audit events
                    //        as such it does not trigger any events which is why 
                    //        after completing the message there is not more processing
                    //        need to ensure that the message has been handled correctly/

                    Trace.TraceInformation( $"[{message.MessageId}] Completed message" );                   

                } catch ( Exception e ){ 
                    Trace.TraceError( $"[{message.MessageId}] Error - Message: {e.Message}, Stack trace: {e.StackTrace}" );
                    throw;

                } finally {
                    dataModel.Dispose();
                }

            }
        }


        private static void processMessageInServiceLayer
                             ( DataModelDbContext dataModel
                             , BrokeredMessage message
                             , ServiceCommandFactory serviceCommandFactory ) {

            if ( dataModel == null ) throw new ArgumentNullException( nameof( dataModel ) );
            if ( message == null ) throw new ArgumentNullException( nameof( message ) );
            if ( serviceCommandFactory == null ) throw new ArgumentNullException( nameof( serviceCommandFactory ) );


            var messageId 
                  = Guid.Parse( message.MessageId );

            // If there is an exception between before the commit you do not
            // want to complete the message.  If there there is a problem 
            // between the commit and the message being completed then we 
            // have a message that will be processed twice so we take the 
            // hit and just check to see if we have seen the message before
            // and if so just return and let the message be cleared of the
            // subscription.
            if ( dataModel.HasSeenMessage( messageId ) ) return ;


            var transaction 
                   = dataModel
                       .Database
                       .BeginTransaction( IsolationLevel.Serializable );
                  
            try {
                
                dataModel
                  .AddMessageAsSeen( messageId );

                                 
                var serviceRequest 
                      = new ServiceRequest(
                           dataModel
                          ,Guid.Parse( message.CorrelationId )
                          ,messageId
                          ,TopicSourceName
                          ,message.ContentType
                          ,message.GetBody<string>() 
                          ,DateTime.Now
                      );


                // Because topics are heterogeneous we need to identify the 
                // correct service command for the message. Different messages
                // have different handlers.
                //
                // It may be valid that we are not interested in auditing 
                // certain messages in which case we need a default handler.
                serviceCommandFactory[ message.ContentType ]
                  .Match( 
                     just: serviceCommand => serviceCommand
                    ,nothing: () => new UnhandledMessage()
                  )
                  .Handle( serviceRequest );

                dataModel.SaveChanges();
                transaction.Commit();
                  
            } finally {
                transaction.Dispose();
            }
        }

        private SubscriptionClient ingressTopicSubscription;
        private readonly ServiceCommandFactory serviceCommandFactory;
    }

}