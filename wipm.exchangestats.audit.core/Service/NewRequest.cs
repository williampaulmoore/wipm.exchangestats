using System;


namespace wipm.exchangestats.audit.core {
  
    public class NewRequest 
                   : ServiceCommand {

        public void Handle
                      ( ServiceRequest serviceRequest ) {

            if ( serviceRequest == null ) throw new ArgumentNullException( nameof( serviceRequest ) );

            // Note - Delegate the action of checking that there is not already
            //        a Request for this RequestId to persistence layer via the 
            //        Key attributer on the model  
            var messageDescription 
                  = serviceRequest.ToMessageDesciptionModel();


            var request 
                  = new RequestModel {
                      RequestId = serviceRequest.RequestId,
                      InitiatedFrom = messageDescription,
                      FirstRecordedAt = serviceRequest.ReceivedAt,
                      UpdatedAt = serviceRequest.ReceivedAt,
                      Updates = {
                          new RequestUpdateModel {
                            MessageId = serviceRequest.MessageId,
                            MessageDescription = messageDescription,
                            MessageBody = serviceRequest.Message,
                            RecordedAt = serviceRequest.ReceivedAt
                          }
                      }
                    };
            
            serviceRequest
              .DataModel
              .RequestModels
              .Add( request );
            
        }
    }
}
