using System;
using System.Collections.Generic;
using System.Data.Entity;
using wipm.exchangestats.audit.core;

namespace wipm.exchangestats.audit.listener {


    /// <summary>
    /// Adapts a DbSet to RequestModels set, 
    /// </summary>
    class RequestModelSetAdapter 
            : RequestModelSet {

        public IEnumerable<RequestModel> All {
            get {
                return requestSet;
            }
        }

        public void Add
                      ( RequestModel request ) {
            
            if (  request == null ) throw new ArgumentNullException( nameof( request  ) );


            requestSet.Add( request );
        }


        public RequestModelSetAdapter
                 ( DbSet<RequestModel> requestSet ) {

            if ( requestSet == null ) throw new ArgumentNullException( nameof( requestSet ) );


            this.requestSet = requestSet;
        }

        private readonly DbSet<RequestModel> requestSet;

    }
}
