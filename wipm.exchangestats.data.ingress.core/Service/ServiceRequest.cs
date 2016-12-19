﻿using System;

namespace wipm.exchangestats.data.ingress.core {

    /// <summary>
    /// Request expected by all service commands
    /// </summary>
    public class ServiceRequest {

        public readonly DataModel DataModel;

        public readonly string Message;


        public ServiceRequest
                 ( DataModel dataModel
                 , string message) {


            if ( dataModel == null ) throw new ArgumentNullException( nameof( dataModel ) );
            if ( string.IsNullOrWhiteSpace( message ) ) throw new ArgumentException( nameof( message ));


            this.DataModel = dataModel;
            this.Message = message;
        }


    }
}
