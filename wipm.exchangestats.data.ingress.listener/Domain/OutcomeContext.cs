using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wipm.exchangestats.data.ingress.listener {

    class OutcomeContext  {

        public readonly ServiceContext ServiceContext;
        public readonly MessageOutcome MessageOutcome;


        public OutcomeContext
                ( ServiceContext serviceContext
                , MessageOutcome messageOutcome ) { 

            if ( serviceContext == null ) throw new ArgumentNullException( nameof( serviceContext ) );
            if ( messageOutcome == null ) throw new ArgumentNullException( nameof( messageOutcome ) );


            ServiceContext = serviceContext;
            MessageOutcome = messageOutcome;
        }

    }

}
