using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using System;
using System.Configuration;
using wipm.exchangestats.data.ingress.interfaces;
using wipm.exchangestats.infrastrcuture;
using wipm.library.messaging;

namespace wipm.exchangestats.data.ingress.gateway {

    public class AzureServiceBusIngressGatewayTopic
                  : IIngressGatewayQueue {

        public void Dispatch
                     ( IngressQueueEntry queueEntry ) {

            if ( queueEntry == null ) throw new ArgumentNullException( nameof( queueEntry ) );


            var message = new BrokeredMessage( queueEntry.Message );
                message.CorrelationId = queueEntry.RequestId.ToString();
                message.MessageId = queueEntry.MessageId.ToString();
                message.ContentType = queueEntry.MessageType;

            ingressGatewayTopic.Send( message );            
        }


        public AzureServiceBusIngressGatewayTopic() {
            var connectionString = ConfigurationManager.AppSettings[ "ingress_gateway_service_bus_connection_string" ];

            ingressGatewayTopic = TopicClient.CreateFromConnectionString( 
                 connectionString
                , IngressGatewayTopic.Name 
            );
        }


        private readonly TopicClient ingressGatewayTopic;
    }
}