using System;

namespace wipm.exchangestats.audit.core {

    public class UnhandledMessage
            : ServiceCommand {

        public void Handle
                     ( ServiceRequest serviceRequest ) {

            if ( serviceRequest == null ) throw new ArgumentNullException( nameof( serviceRequest ) );


            var messageDescription 
                  = serviceRequest.ToMessageDesciptionModel();

            var unhandledMessage
                    = new UnhandledMessageModel {
                        MessageId = serviceRequest.MessageId,
                        MessageDescription = messageDescription,
                        RequestId = serviceRequest.RequestId,
                        MessageBody = serviceRequest.Message,
                        RecorderAt = serviceRequest.ReceivedAt
                    };

            serviceRequest
                .DataModel
                .UnhandledMessageModels
                .Add( unhandledMessage );

        }
    }
}
