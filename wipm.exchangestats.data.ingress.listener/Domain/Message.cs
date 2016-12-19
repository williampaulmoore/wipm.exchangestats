using System;

namespace wipm.exchangestats.data.ingress.listener {
    
    /// <summary>
    /// Represent a message taken from
    /// </summary> 
    public class Message {

        public readonly Guid RequestId;
        public readonly string Body;
        public readonly string MessageType;


        public Message
                ( Guid requestId 
                , string message
                , string messageType ) {

            if ( string.IsNullOrWhiteSpace( message ) ) throw new ArgumentNullException( nameof( message ) );
            if ( string.IsNullOrWhiteSpace( messageType )) throw new ArgumentException( nameof( messageType ) );


            RequestId = requestId;
            Body = message;
            MessageType = messageType;
        }

    }

}
