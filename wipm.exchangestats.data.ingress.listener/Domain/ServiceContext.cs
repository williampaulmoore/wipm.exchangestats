using System;
using wipm.exchangestats.data.ingress.core;
using wipm.library.messaging;

namespace wipm.exchangestats.data.ingress.listener {

    class ServiceContext {

        public readonly DataModelDbContext ServiceDataModel;
        public readonly ServiceCommandFactory ServiceCommandFactory;
        public readonly IIngressQueue DataIngressTopic;

        public ServiceContext
                ( DataModelDbContext serviceDataModel
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
