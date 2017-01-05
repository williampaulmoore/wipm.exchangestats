using System;

namespace wipm.exchangestats.data.ingress.listener {

    class MessageContext {
    
        public readonly ServiceContext ServiceContext;
        public readonly Message Message;
    
        public MessageContext
                ( ServiceContext serviceContext
                , Message message ) {
    
            if ( serviceContext == null ) throw new ArgumentNullException( nameof( serviceContext ) );
            if ( message == null ) throw new ArgumentNullException( nameof( message ) );
    
            this.ServiceContext = serviceContext;
            this.Message = message;
    
        }
    
    }



}
