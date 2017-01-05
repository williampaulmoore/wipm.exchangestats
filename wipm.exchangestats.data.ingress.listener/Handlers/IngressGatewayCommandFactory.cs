using wipm.exchangestats.data.ingress.core;
using wipm.exchangestats.infrastrcuture;

namespace wipm.exchangestats.data.ingress.listener {

    class IngressGatewayCommandFactory
            : ServiceCommandFactory {

        public IngressGatewayCommandFactory() {

            Register( IngressGatewayTopic.Messages.Exchanges, () => new ExchangeDataReceived() );
        }

    }
}
