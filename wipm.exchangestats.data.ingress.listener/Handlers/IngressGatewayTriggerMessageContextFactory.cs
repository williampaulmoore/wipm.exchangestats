using Microsoft.ServiceBus.Messaging;
using System;

namespace wipm.exchangestats.data.ingress.listener {

    class IngressGatewayTriggerMessageContextFactory {

        public static TriggerMessageContext create
                                              ( BrokeredMessage brokeredMessage
                                              , DataModelDbContext serviceDataModel ) {

            if ( brokeredMessage == null ) throw new ArgumentNullException( nameof( brokeredMessage ) );
            if ( serviceDataModel == null ) throw new ArgumentNullException( nameof( serviceDataModel ) );


            var serviceContext 
                  =  new ServiceContext(
                        serviceDataModel: serviceDataModel
                       ,serviceCommandFactory: new IngressGatewayCommandFactory()
                       ,dataIngressTopic: new DataIngressTopic() 
                     );

            var message 
                 = new Message(
                       requestId: Guid.Parse( brokeredMessage.CorrelationId )
                      ,messageId: Guid.Parse( brokeredMessage.MessageId )
                      ,message: brokeredMessage.GetBody<string>()
                      ,messageType: brokeredMessage.ContentType
                   );

            var messageContext 
                 = new MessageContext( 
                      serviceContext: serviceContext
                     ,message: message
                   );

            return 
              new TriggerMessageContext( Guid.NewGuid(), messageContext );
                   
        }

    }
}
