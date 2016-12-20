using System;

namespace wipm.exchangestats.data.ingress.core {

    /// <summary>
    /// Request expected by all service commands
    /// </summary>
    public class ServiceRequest {

        public readonly CoreDataModel DataModel;

        public readonly string Message;


        public ServiceRequest
                 ( CoreDataModel dataModel
                 , string message) {


            if ( dataModel == null ) throw new ArgumentNullException( nameof( dataModel ) );
            if ( string.IsNullOrWhiteSpace( message ) ) throw new ArgumentException( nameof( message ));


            this.DataModel = dataModel;
            this.Message = message;
        }


    }
}
