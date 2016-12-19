using System;

namespace wipm.exchangestats.data.ingress.listener {

    public class MessageOutcome {

        public readonly Guid RequestId;
        public readonly string OutcomeType;
        public readonly string Outcome;

        public MessageOutcome
                ( Guid requestId 
                , string outcomeType
                , string outcome ) { 

            if ( requestId == null ) throw new ArgumentNullException( nameof( requestId ) );
            if ( string.IsNullOrWhiteSpace( outcomeType ) ) throw new ArgumentNullException( nameof( outcomeType ) );
            if ( string.IsNullOrWhiteSpace( outcome ) ) throw new ArgumentNullException( nameof( outcome ) );


            RequestId = requestId;
            OutcomeType = outcomeType;
            Outcome = outcome;
        }

    }
}
