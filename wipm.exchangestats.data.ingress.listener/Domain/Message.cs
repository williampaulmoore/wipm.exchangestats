using System;

namespace wipm.exchangestats.data.ingress.listener {
    
    /// <summary>
    /// Represent a message taken from
    /// </summary> 
    public class Message {

        public readonly Guid RequestId;
        public readonly Guid MessageId;
        public readonly string Body;
        public readonly string MessageType;


        public Message
                ( Guid requestId 
                , Guid messageId
                , string message
                , string messageType ) {


            if ( requestId == Guid.Empty ) throw new ArgumentException( nameof( requestId  ) );
            if ( messageId == Guid.Empty ) throw new ArgumentException( nameof( messageId  ) );
            if ( string.IsNullOrWhiteSpace( message ) ) throw new ArgumentNullException( nameof( message ) );
            if ( string.IsNullOrWhiteSpace( messageType )) throw new ArgumentException( nameof( messageType ) );


            RequestId = requestId;
            MessageId = messageId;
            Body = message;
            MessageType = messageType;
        }

    }

}
