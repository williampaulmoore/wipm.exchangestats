using System;

namespace wipm.exchangestats.data.ingress.interfaces {

    public static class Messages {

        public const string Exchanges = "Exchages";

        public static Q Match<Q>
                         ( this string messageType
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
