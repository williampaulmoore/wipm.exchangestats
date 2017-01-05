using System;
using System.Collections.Generic;
using System.Data.Entity;
using wipm.exchangestats.audit.core;

namespace wipm.exchangestats.audit.listener {

    class UnhandledMessageModelSetAdapter 
            : UnhandledMessageModelSet {

        public IEnumerable<UnhandledMessageModel> All {
            get {
                return unhandledMessageSet;
            }
        }

        public void Add
                      ( UnhandledMessageModel message ) {
            
            if (  message == null ) throw new ArgumentNullException( nameof( message  ) );


            unhandledMessageSet.Add( message );
        }

        public UnhandledMessageModelSetAdapter            
                 ( DbSet<UnhandledMessageModel> unhandledMessageSet ) {

            if ( unhandledMessageSet == null ) throw new ArgumentNullException( nameof( unhandledMessageSet ) );


            this.unhandledMessageSet = unhandledMessageSet;
        }

        private readonly DbSet<UnhandledMessageModel> unhandledMessageSet;


    }
}
