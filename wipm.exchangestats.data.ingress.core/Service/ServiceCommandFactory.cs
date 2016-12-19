using System;
using System.Collections.Generic;

namespace wipm.exchangestats.data.ingress.core {


    /// <summary>
    /// Factory that provides a service command for a give cammand type.
    /// </summary>
    /// <remarks>
    /// This is intended to be uses from a json message recieved
    /// </remarks>
    public class ServiceCommandFactory {


        public ServiceCommand this[ string  index ] {

            get {
                var factoryMethod 
                      = factories[ index ];

                return factoryMethod();
            }
        }


        public void Register 
                      ( string index
                      , Func<ServiceCommand> factoryMethod ) {

            if ( string.IsNullOrWhiteSpace( index ) ) throw new ArgumentException( nameof( index ) );
            if ( factories.ContainsKey( index ) ) throw new ArgumentException( "Key already exists" );
            if ( factoryMethod == null ) throw new ArgumentNullException( nameof( factoryMethod ) );


            factories.Add( index, factoryMethod );
        }


        private readonly Dictionary<string,Func<ServiceCommand>> factories = new Dictionary<string, Func<ServiceCommand>>();

    }
}
