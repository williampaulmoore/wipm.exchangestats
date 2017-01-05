using lqd.net.functional;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Data;
using System.Diagnostics;
using wipm.exchangestats.data.ingress.core;
using wipm.exchangestats.infrastrcuture;
using wipm.library.messaging;

namespace wipm.exchangestats.data.ingress.listener {

    class Service {

        public void Start() {
            
            startHandler( ingressGatewayHandler  );

        }

        public void Stop() {

            stopHandler( ingressGatewayHandler );
        }

        public Service() {

            ingressGatewayHandler = new IngressGatewayTopicHandler();

        }

        private void startHandler
                       ( TopicHandler topicHandler ) {

            if ( topicHandler == null ) throw new ArgumentNullException( nameof( topicHandler ) );


            Trace.TraceInformation( $"{topicHandler.Name} - Starting" );

            topicHandler.Start();

            Trace.TraceInformation( $"{topicHandler.Name} - Started" );
        }

        private void stopHandler
                       ( TopicHandler topicHandler ) {

            if ( topicHandler == null ) throw new ArgumentNullException( nameof( topicHandler ) );


            Trace.TraceInformation( $"{topicHandler.Name} - Stopped" );

            topicHandler.Stop();

            Trace.TraceInformation( $"{topicHandler.Name} - Stopping" );
        }


        private readonly IngressGatewayTopicHandler ingressGatewayHandler;

    }

}
