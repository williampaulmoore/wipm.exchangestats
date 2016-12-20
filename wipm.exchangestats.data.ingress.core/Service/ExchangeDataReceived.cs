using lqd.net.functional;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using wipm.exchangestats.data.ingress.interfaces;

namespace wipm.exchangestats.data.ingress.core {

    public class ErrorDeserialisingExchangeDataRecievedMessage 
                  : StateChangeError { }


    /// <summary>
    /// Receives a set of exchange data records and for each record either 
    /// updates an existing exchange or create a new one.
    /// </summary>
    public class ExchangeDataReceived
                  : ServiceCommand {


        public ServiceResponse Handle
                                 ( ServiceRequest request ) {

            if ( request == null ) throw new ArgumentNullException( nameof( request ) );


            var domainModel = new DomainModel( request.DataModel );
            var updateDomain = getUpdateDomainFunc( domainModel );

            return
              this 
                .deserialise( request.Message )
                .Select( updateDomain )
                .Aggregate(
                    new ResponseAccumulator()
                   ,collateUpdateResponses
                   ,( acc ) => new ServiceResponse( acc.Events, acc.Errors )
                );

        }


        // Convert the json into ExchangeData.  Note we can not be sure 
        // the the json will deserialise into Exchange data so it is 
        // wrapped in an Either Monad. 
        //
        // Note - 
        //  The entire collection has to deserialise correctly this
        //  this should be changes as there is no reason for the 
        //  list to be atomic.  i.e. just because one record in
        //  the list is not valid json should not stop the other 
        //  entries from being processed.
        //
        private IEnumerable<Either<StateChangeError,ExchangeData>> deserialise
                                                                     ( string message ) {

            if ( string.IsNullOrWhiteSpace( message ) ) throw new ArgumentNullException( nameof( message ) );


            try {

                var exchanges 
                      = ExchangeDataJsonConverter
                          .DeserialiseEnumerable( message );

                return 
                  exchanges
                    .Select( Either<StateChangeError,ExchangeData>.Right );

            } catch {

                return new [] { Either<StateChangeError,ExchangeData>.Left( new ErrorDeserialisingExchangeDataRecievedMessage() ) };

            }
        }


        // partially apply the domain model to updateDomainForExchangeData.
        private Func<Either<StateChangeError,ExchangeData>,Either<StateChangeError,StateChangeEvent>> getUpdateDomainFunc
                                                                                                        ( DomainModel domainModel ) {

            if ( domainModel == null ) throw new ArgumentNullException( nameof( domainModel ) );


            return ( either => either.IfRight( data => updateDomainForExchangeData( domainModel, data ) ) );
        }

        /// Update the exchange if the exchange already exists or create a 
        /// new exchange if it does not.
        private Either<StateChangeError,StateChangeEvent> updateDomainForExchangeData
                                                            ( DomainModel domainModel
                                                            , ExchangeData data  ) {

            if ( domainModel == null ) throw new ArgumentNullException( nameof( domainModel ) );
            if ( data == null ) throw new ArgumentNullException( nameof( data ) );


            return
              domainModel
                .GetCommandForNewExchangeData( data )  
                .Execute();

        }

        private ResponseAccumulator collateUpdateResponses
                                      ( ResponseAccumulator accumulator
                                      , Either<StateChangeError,StateChangeEvent> eitherStateChangeErrorOrEvent ) {
                  
            eitherStateChangeErrorOrEvent.Match(
                left: accumulator.Errors.Add   // error  Note - this is following the Haskell convention
               ,right: accumulator.Events.Add  // event 
            );                    
                  
            return accumulator;
        }


        private class ResponseAccumulator {

            public readonly List<StateChangeError> Errors;

            public readonly List<StateChangeEvent> Events;


            public ResponseAccumulator() {
                Errors = new List<StateChangeError>();
                Events = new List<StateChangeEvent>();
            }
        }

    }

}
