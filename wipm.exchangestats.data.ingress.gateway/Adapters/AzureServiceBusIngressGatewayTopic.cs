using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using System;
using System.Configuration;
using wipm.exchangestats.data.ingress.interfaces;
using wipm.library.messaging;

namespace wipm.exchangestats.data.ingress.gateway {

    public class AzureServiceBusIngressGatewayTopic
                  : IIngressGatewayQueue {

        public void Dispatch<P>
                     ( IngressQueueEntry<P> queueEntry ) {

            if ( queueEntry == null ) throw new ArgumentNullException( nameof( queueEntry ) );


            var json = JsonConvert.SerializeObject( queueEntry.Message );

            var message = new BrokeredMessage( json );
                message.MessageId = queueEntry.RequestId.ToString();
                message.ContentType = queueEntry.MessageType;

            ingressGatewayTopic.Send( message );            
        }


        public AzureServiceBusIngressGatewayTopic() {
            var connectionString = ConfigurationManager.AppSettings[ "ingress_gateway_service_bus_connection_string" ];

            ingressGatewayTopic = TopicClient.CreateFromConnectionString( 
                 connectionString
                ,Topics.IngressGatewayTopicName
            );
        }


        private readonly TopicClient ingressGatewayTopic;
    }
}