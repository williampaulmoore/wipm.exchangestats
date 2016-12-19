using System;
using System.Collections.Generic;

namespace wipm.exchangestats.data.ingress.core {


    /// <summary>
    /// Response that is returned by a service command
    /// </summary>
    public class ServiceResponse {

            public readonly IEnumerable<StateChangeEvent> Events;
            public readonly IEnumerable<StateChangeError> Errors;


            public ServiceResponse
                     ( IEnumerable<StateChangeEvent> events
                     , IEnumerable<StateChangeError> errors ) {

                if ( events == null ) throw new ArgumentNullException( nameof( events ) );
                if ( errors == null ) throw new ArgumentNullException( nameof( errors ) );


                this.Events = events;
                this.Errors = errors;

            }
    }
}
