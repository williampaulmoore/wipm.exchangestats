using lqd.net.functional;
using System;
using System.Collections.Generic;
using System.Linq;
using wipm.exchangestats.data.ingress.interfaces;

namespace wipm.exchangestats.data.ingress.core {


    /// <summary>
    /// Error reported if there is an validation error with the Exchange data.
    /// </summary>
    public class AddExchangeError 
                  : RecursiveStateChangeError {

        public AddExchangeError
                 ( IEnumerable<StateChangeError> errors ) : base( errors ) {}

    }

    /// <summary>
    /// Event that is reported when the command is successfull
    /// </summary>
    public class ExchangeAdded 
                  : StateChangeEvent { }


    /// <summary>
    /// Add an exchange based on the exchange data supplied at construction time.
    /// </summary>
    public class AddExchange 
                  : ExchangeDataCommand {

        public Either<StateChangeError,StateChangeEvent> Execute() {

            return
              this
                .validate( data )
                .LiftIfRight( addExchange  )
                ;

        }

        public AddExchange
                 ( ExchangeModelSet exchanges
                 , ExchangeData data ) {

            if ( exchanges == null ) throw new ArgumentNullException( nameof( exchanges ) );
            if ( data == null ) throw new ArgumentNullException( nameof( data ) ); 


            this.exchanges = exchanges;
            this.data = data;
        }



        private Either<StateChangeError,ExchangeModel> validate
                                                         ( ExchangeData data ) {

            if ( data == null ) throw new ArgumentNullException( nameof( data ) );

            // todo: validator should not have a checkCode is Unique flag need to think about validation and the implications
            var errors = ExchangeDataValidator.Validate( 

                new ExchangeDataValidationRequest (
                   exchanges: exchanges.All
                  ,data: data
                  ,checkCodeIsUnique: true
                )

            );

            return errors.Count() == 0
                 ? Either<StateChangeError,ExchangeModel>.Right( new ExchangeModel { Code =data.Code, Name = data.Name } )
                 : Either<StateChangeError,ExchangeModel>.Left( new AddExchangeError( errors ) )
                 ;
        }


        private StateChangeEvent addExchange
                                   ( ExchangeModel exchange ) {

            if ( exchange == null ) throw new ArgumentNullException( nameof( exchange ) );


            exchanges.Add( exchange );

            return new ExchangeAdded();
        }


        private readonly ExchangeModelSet exchanges;
        private readonly ExchangeData data;
    }

}
