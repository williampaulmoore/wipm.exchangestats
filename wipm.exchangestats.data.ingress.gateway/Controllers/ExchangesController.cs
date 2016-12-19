
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using wipm.exchangestats.data.ingress.interfaces;
using wipm.library.messaging;

namespace wipm.exchangestats.data.ingress.gateway.Controllers {

    [Route("v1/exchanges")]
    public class ExchangesController 
                   : ApiController  {

        [HttpPost]
        public HttpResponseMessage Post
                                     ( IEnumerable<ExchangeData> exchanges ) {
            
            if ( exchanges == null ) return new HttpResponseMessage( HttpStatusCode.BadRequest );
            if ( exchanges.Count() == 0 ) return new HttpResponseMessage( HttpStatusCode.BadRequest );


            var queueEntry = new IngressQueueEntry<IEnumerable<ExchangeData>>(
                RequestId: requestIdentityProvider.GetId()
               ,Message: exchanges
               ,MessageType: Messages.Exchanges
            );

            Trace.TraceInformation( $"[{queueEntry.RequestId}] - Publishing" );

            ingressGatewayQueue.Dispatch( queueEntry );

            Trace.TraceInformation( $"[{queueEntry.RequestId}] - Published" );

            return new HttpResponseMessage( HttpStatusCode.OK );
        }


        public ExchangesController
                ( IRequestIdentityProvider requestIdentityProvider
                , IIngressGatewayQueue ingressGatewayQueue ) {

            this.requestIdentityProvider = requestIdentityProvider;
            this.ingressGatewayQueue = ingressGatewayQueue;

        }


        private readonly IRequestIdentityProvider requestIdentityProvider;        
        private readonly IIngressQueue ingressGatewayQueue;
    }
}
