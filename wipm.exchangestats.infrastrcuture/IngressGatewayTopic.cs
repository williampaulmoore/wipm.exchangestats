using System; 

namespace wipm.exchangestats.infrastrcuture {

    public class IngressGatewayTopic {

        public const string Name = "ingressGateway";

        public static class Subscriptions {

            public const string DataIngressListener = "dataIngressListener";
            public const string AuditListener = "auditListener";

        }


        public static class Messages {

            public const string Exchanges = "Exchages";

            public static Q Match<Q>
                             ( string messageType
                             , Func<Q> exchanges ) {

                if ( string.IsNullOrWhiteSpace( messageType ) ) throw new ArgumentNullException( nameof( messageType ) );
                if ( exchanges == null  ) throw new ArgumentNullException( nameof( exchanges ) );


                if ( messageType.Equals( Exchanges ) ) {
                    return exchanges();
                }
                throw new Exception( "Unexpected type" );
            }
        }


    }
}
