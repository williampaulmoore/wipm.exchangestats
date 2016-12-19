using lqd.net.functional;
using System;
using System.Collections.Generic;
using System.Linq;
using wipm.exchangestats.data.ingress.interfaces;

namespace wipm.exchangestats.data.ingress.core {

    public class UpdateExchangeError : RecursiveStateChangeError {

        public UpdateExchangeError( IEnumerable<StateChangeError> errors ) : base( errors ) {}

    }

    public class ExchangeUpdated : StateChangeEvent { }

    public class UpdateExistingExchange 
                  : ExchangeDataCommand {

        public Either<StateChangeError, StateChangeEvent> Execute() {

            return
              this
                .validate( data )
                .LiftIfRight( updateModel );

        }

        public UpdateExistingExchange
                ( ExchangeModel exchange
                , ExchangeModelSet exchanges
                , ExchangeData data ) {

            if ( exchange == null ) throw new ArgumentNullException( nameof( exchange ) );
            if ( exchanges == null ) throw new ArgumentNullException( nameof( exchanges ) );
            if ( data == null ) throw new ArgumentNullException( nameof( data ) );
            if ( exchange.Code != data.Code ) throw new ArgumentException( "Data and model's codes do not match" );


            this.exchange = exchange;
            this.exchanges = exchanges;
            this.data = data;
        }


        private Either<StateChangeError,UpdateRequest> validate
                                                      ( ExchangeData data ) {

            var errors = ExchangeDataValidator.Validate( 

                new ExchangeDataValidationRequest (
                   exchanges: exchanges.All
                  ,data: data
                  ,checkCodeIsUnique: false // it should exist
                )

            );

            return errors.Count() == 0
                 ? Either<StateChangeError,UpdateRequest>.Right( new UpdateRequest( data, exchange ) )
                 : Either<StateChangeError,UpdateRequest>.Left( new UpdateExchangeError( errors ) )
                 ;
        }

        private StateChangeEvent updateModel
                                 ( UpdateRequest request ) {


            request.model.Name = request.data.Name;

            return new ExchangeUpdated();
        }


        private readonly ExchangeModel exchange;
        private readonly ExchangeModelSet exchanges;
        private readonly ExchangeData data;

        private class UpdateRequest {

            public readonly ExchangeData data;
            public readonly ExchangeModel model;


            public UpdateRequest
                    ( ExchangeData data
                    , ExchangeModel model ) {

                if ( data == null) throw new ArgumentNullException( nameof( data ) );
                if ( model == null ) throw new ArgumentNullException( nameof( model ) );

                this.data = data;
                this.model = model;
            }

        }
    }

}
