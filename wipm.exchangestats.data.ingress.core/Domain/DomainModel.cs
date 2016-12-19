using lqd.net.functional;
using System;
using System.Collections.Generic;
using System.Linq;
using wipm.exchangestats.data.ingress.interfaces;

namespace wipm.exchangestats.data.ingress.core {

    public class StateChangeOutcome { }

    public class StateChangeEvent 
                   : StateChangeOutcome { }

    public class StateChangeError 
                   : StateChangeOutcome { }

    public class RecursiveStateChangeError 
                   : StateChangeError {

        public readonly IEnumerable<StateChangeError> Errors;

        public RecursiveStateChangeError
                 ( IEnumerable<StateChangeError> errors ) {

            if ( errors == null ) throw new ArgumentNullException( nameof( errors ) );
            if ( errors.Count() ==0 ) throw new ArgumentException( nameof( errors ) );

            this.Errors = errors;
        }

    }



    public class DomainModel {

        public ExchangeDataCommand GetCommandForNewExchangeData       
                                     ( ExchangeData data ) {

            return
              this
               .getExchange( data.Code )
               .Match(
                   just: exchange => new UpdateExistingExchange( exchange, dataModel.ExchangeModels, data ) as ExchangeDataCommand
                  ,nothing: () => new AddExchange( dataModel.ExchangeModels, data )
               );

        }


        private Maybe<ExchangeModel> getExchange
                                       ( string code ) {

            var exchange 
                  = dataModel
                     .ExchangeModels
                     .All
                     .SingleOrDefault( e => e.Code.Equals( code ) );


            return exchange != null
                 ? Maybe<ExchangeModel>.Just( exchange )
                 : Maybe<ExchangeModel>.Nothing()
                 ;

        }


        public DomainModel
                 ( DataModel dataModel ) {

            if ( dataModel == null ) throw new ArgumentNullException( nameof( dataModel ) );


            this.dataModel = dataModel;

        }

        private DataModel dataModel;
    }
   
}
