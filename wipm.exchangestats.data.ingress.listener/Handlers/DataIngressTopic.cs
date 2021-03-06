﻿using Microsoft.ServiceBus.Messaging;
using System.Configuration;
using wipm.library.messaging;

namespace wipm.exchangestats.data.ingress.listener {

    class DataIngressTopic
           : IIngressQueue {


        public void Dispatch
                     ( IngressQueueEntry envelope ) {

            
            var message = new BrokeredMessage( envelope.Message );
                message.CorrelationId = envelope.RequestId.ToString();
                message.MessageId = envelope.MessageId.ToString();
                message.ContentType = envelope.MessageType;

            ingressDataTopic.Send( message );
        }


        public DataIngressTopic() {
            var connectionString 
                  = ConfigurationManager.AppSettings[ "ingress_data_service_bus_connection_string" ];

            ingressDataTopic 
              = TopicClient.CreateFromConnectionString( 
                   connectionString
                  ,infrastrcuture.DataIngressTopic.Name
                );

        }


        private TopicClient ingressDataTopic;
    }

}
