using System;

namespace wipm.library.messaging {

    // Queue that data ingress requests are posted to. 
    public interface IIngressQueue {

        void Dispatch
               ( IngressQueueEntry envelope );

    }

    // Represents an entry on the queue
    public class IngressQueueEntry {

        // Unique id for this request
        public Guid RequestId { get; private set; }

        // Domain specific message
        public string Message { get; private set; }

        // Get the type that was intended rather
        public string MessageType { get; private set; }

        public IngressQueueEntry
                ( Guid RequestId
                , string Message
                , string MessageType ) {

            if ( RequestId == Guid.Empty ) throw new ArgumentException( nameof( RequestId ) );
            if ( Message == null ) throw new ArgumentNullException( nameof( Message ) );
            if ( String.IsNullOrWhiteSpace( MessageType ) ) throw new ArgumentException( nameof(  MessageType ) );


            this.RequestId = RequestId;
            this.Message = Message;
            this.MessageType = MessageType;
        }

    }
 
}