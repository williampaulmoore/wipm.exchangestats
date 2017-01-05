using System;

namespace wipm.exchangestats.data.ingress.listener {

    // A trigger message is a message that came from the message pump.
    class TriggerMessageContext {

        public readonly Guid OutomceMessageId;

        public readonly MessageContext MessageContext;

        public TriggerMessageContext
                 ( Guid outcomeMessageId
                 , MessageContext messageContext ) {

            if ( outcomeMessageId == Guid.Empty ) throw new ArgumentException( nameof( outcomeMessageId ) );
            if ( messageContext == null ) throw new ArgumentNullException( nameof( messageContext ) );

            this.OutomceMessageId = outcomeMessageId;
            this.MessageContext = messageContext;
        }

    }
}
