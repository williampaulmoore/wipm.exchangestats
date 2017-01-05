using lqd.net.functional;
using System;
using System.Collections.Generic;
using wipm.exchangestats.infrastrcuture;

namespace wipm.exchangestats.audit.core {


    /// <summary>
    /// Factory that provides a service command for a give cammand type.
    /// </summary>
    /// <remarks>
    /// This is intended to be uses from a json message recieved
    /// </remarks>
    public class ServiceCommandFactory {


        public Maybe<ServiceCommand> this[ string  index ] {

            get {

                return factories.ContainsKey( index )
                     ? Maybe<ServiceCommand>.Just(  factories[ index ]() )
                     : Maybe<ServiceCommand>.Nothing()
                     ;
            }
        }


        public ServiceCommandFactory() {

            factories 
              = new Dictionary<string,Func<ServiceCommand>>();


            factories.Add( IngressGatewayTopic.Messages.Exchanges, () => new NewRequest() );

        }


        private readonly Dictionary<string,Func<ServiceCommand>> factories;
    }
}
