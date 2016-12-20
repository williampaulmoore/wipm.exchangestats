using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace wipm.exchangestats.data.ingress.interfaces {

    /// <summary>
    ///    Serialise and Deserialise Exchange data to json
    /// </summary>  
    public class ExchangeDataJsonConverter {

        public static string Serialise
                               ( ExchangeData exchangeData ) {

            if ( exchangeData == null ) throw new ArgumentNullException( nameof( exchangeData ) );

            return 
              JsonConvert.SerializeObject( exchangeData );

        }

        public static ExchangeData Deserialise
                                    ( string json ) {

            if ( string.IsNullOrWhiteSpace( json ) ) throw new ArgumentException(  nameof( json ) );


            return
              JsonConvert.DeserializeObject<ExchangeData>( json );

        }


        public static string SerialiseEnumerable
                               ( IEnumerable<ExchangeData> exchangeData ) {

            if ( exchangeData == null ) throw new ArgumentNullException( nameof( exchangeData ) );

            return 
              JsonConvert.SerializeObject( exchangeData );

        }


        public static IEnumerable<ExchangeData> DeserialiseEnumerable
                                                 ( string json ) {

            if ( string.IsNullOrWhiteSpace( json ) ) throw new ArgumentException(  nameof( json ) );


            return
              JsonConvert.DeserializeObject<IEnumerable<ExchangeData>>( json );

        }

    }
}
