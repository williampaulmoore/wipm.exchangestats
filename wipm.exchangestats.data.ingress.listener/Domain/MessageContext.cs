using System;
using wipm.exchangestats.data.ingress.core;
using wipm.library.messaging;

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

    class ServiceContext {

        public readonly ServiceDataModel ServiceDataModel;
        public readonly ServiceCommandFactory ServiceCommandFactory;
        public readonly IIngressQueue DataIngressTopic;

        public ServiceContext
                ( ServiceDataModel serviceDataModel
                , ServiceCommandFactory serviceCommandFactory
                , IIngressQueue dataIngressTopic ){

            if ( serviceDataModel == null ) throw new ArgumentNullException( nameof( serviceDataModel ) );
            if ( serviceCommandFactory == null ) throw new ArgumentNullException( nameof( serviceCommandFactory ) );
            if ( dataIngressTopic == null ) throw new ArgumentNullException( nameof( dataIngressTopic ) );


            ServiceDataModel = serviceDataModel;
            ServiceCommandFactory = serviceCommandFactory;
            DataIngressTopic = dataIngressTopic;

        }


    }

}
