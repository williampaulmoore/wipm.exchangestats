using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using System;
using System.Configuration;
using wipm.exchangestats.data.ingress.interfaces;
using wipm.library.messaging;

namespace wipm.exchangestats.data.ingress.listener {

    class AzureServiceBusIngressDataTopic
           : IIngressQueue {


        public void Dispatch<P>
                     ( IngressQueueEntry<P> envelope ) {

            var json = JsonConvert.SerializeObject( envelope.Message );

            var message = new BrokeredMessage( json );
                message.MessageId = envelope.RequestId.ToString();
                message.ContentType = envelope.MessageType;

            ingressDataTopic.Send( message );
        }


        public AzureServiceBusIngressDataTopic() {
            var connectionString = ConfigurationManager.AppSettings[ "ingress_data_service_bus_connection_string" ];

            ingressDataTopic = TopicClient.CreateFromConnectionString( 
                 connectionString
                ,Topics.DataIngressTopicName 
            );

        }


        private TopicClient ingressDataTopic;
    }

}
