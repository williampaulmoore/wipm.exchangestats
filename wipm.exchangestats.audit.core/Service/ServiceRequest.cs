using System;

namespace wipm.exchangestats.audit.core {

    public class ServiceRequest {

        public readonly AuditDataModel DataModel;

        public readonly Guid RequestId;

        public readonly Guid MessageId;

        public readonly string MessageSource;

        public readonly string MessageType;

        public readonly string Message; 

        public readonly DateTime ReceivedAt;


        public ServiceRequest
                ( AuditDataModel dataModel
                , Guid requestId
                , Guid messageId
                , string messageSource
                , string messageType
                , string message
                , DateTime receivedAt ) {

            if ( dataModel == null ) throw new ArgumentNullException( nameof( dataModel ) );
            if ( requestId == Guid.Empty ) throw new ArgumentException( nameof( requestId ) );
            if ( messageId == Guid.Empty ) throw new ArgumentException( nameof( messageId ) );
            if ( string.IsNullOrWhiteSpace( messageType ) ) throw new ArgumentException( nameof( messageType ) );
            if ( string.IsNullOrWhiteSpace( message ) ) throw new ArgumentException( nameof( message ) );

            this.DataModel = dataModel;
            this.RequestId = requestId;
            this.MessageId = messageId;
            this.MessageSource = messageSource;
            this.MessageType = messageType;
            this.Message = message;
            this.ReceivedAt = receivedAt;
        }


    }

    internal static class ServiceRequestExtension {

        // Create a MessageDescriptionModel from a ServiceRequest
        public static MessageDescriptionModel ToMessageDesciptionModel
                                                ( this ServiceRequest serviceRequest ) {

            if ( serviceRequest == null ) throw new ArgumentNullException( nameof( serviceRequest ) );


            return 
              new MessageDescriptionModel {
                  MessageSource = serviceRequest.MessageSource,
                  MessageType = serviceRequest.MessageType
              };

        }

    }

}
