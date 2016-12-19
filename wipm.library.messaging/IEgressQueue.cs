using System;

namespace wipm.library.messaging {

    public interface IEgressQueue {

        void OnMessage( Action<EgressQueueEntry> action );
        void Completed( EgressQueueEntry entry );
    }

    public class EgressQueueEntry {


        // Unique id for this request
        public Guid RequestId { get; private set; }

        // Serialise message
        public string Message { get; private set; }

        // Get the type that was intended rather
        public string MessageType { get; private set; }


        public EgressQueueEntry
                ( Guid requestId
                , string message
                , string messageType ) {

            if ( requestId == Guid.Empty ) throw new ArgumentNullException( nameof( requestId ) );
            if ( string.IsNullOrWhiteSpace( message ) ) throw new ArgumentException( nameof( message ) );
            if ( string.IsNullOrWhiteSpace( messageType ) ) throw new ArgumentException( nameof( messageType ) );


            this.RequestId = requestId;
            this.Message = message;
            this.MessageType = messageType;

        }

    }
}
