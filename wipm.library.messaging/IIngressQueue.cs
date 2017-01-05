using System;

namespace wipm.library.messaging {

    // Queue that data ingress requests are posted to. 
    public interface IIngressQueue {

        void Dispatch
               ( IngressQueueEntry envelope );

    }

    // Represents an entry on the queue
    public class IngressQueueEntry {

        // Unique id for this request a request may trigger a sequence of 
        // messages from different parts of the system
        public readonly Guid RequestId;

        // Unique id for this message
        public readonly Guid MessageId;

        // Domain specific message
        public readonly string Message;

        // Get the type that was intended rather
        public readonly string MessageType;

        public IngressQueueEntry
                ( Guid requestId
                , Guid messageId
                , string message
                , string messageType ) {

            if ( requestId == Guid.Empty ) throw new ArgumentException( nameof( requestId ) );
            if ( messageId == Guid.Empty ) throw new ArgumentException( nameof( messageId ) );
            if ( message == null ) throw new ArgumentNullException( nameof( message ) );
            if ( String.IsNullOrWhiteSpace( messageType ) ) throw new ArgumentException( nameof(  messageType ) );


            this.RequestId = requestId;
            this.MessageId = messageId; 
            this.Message = message;
            this.MessageType = messageType;
        }

    }
 
}