using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace wipm.exchangestats.audit.listener {


    // Audit service - 
    // 
    // Ensure that the handlers for audited messages are started and stoped 
    // when the service is started or stopped.
    //
    // The topic handlers are registered in the constructor and when the 
    // service is started of stopped it will ensure that all registered
    // handlers are started or stopped.
    class Service {

        public void Start() {

            foreach( var handler in topicHandlers ) {
                startHandler( handler );
            }
        }

        public void Stop() {

            foreach( var handler in topicHandlers ) {
                stopHandler( handler );
            }
        }

        public Service() {

            topicHandlers = new Collection<TopicHandler>();

            topicHandlers.Add( new IngressGatewayTopicHandler() );
        }


        private void startHandler
                       ( TopicHandler topicHandler ) {

            if ( topicHandler == null ) throw new ArgumentNullException( nameof( topicHandler ) );


            topicHandler.Start();

            Trace.TraceInformation( $"{topicHandler.Name} - Started" );
        }

        private void stopHandler
                       (TopicHandler topicHandler ) {

            if ( topicHandler == null ) throw new ArgumentNullException( nameof( topicHandler ) );


            topicHandler.Stop();

            Trace.TraceInformation( $"{topicHandler.Name} - Stopped" );
        }


        private readonly ICollection<TopicHandler> topicHandlers;
    }
}
