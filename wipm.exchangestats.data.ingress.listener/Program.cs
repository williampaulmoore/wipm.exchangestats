using System.Data.Entity;
using Topshelf;

namespace wipm.exchangestats.data.ingress.listener {

    public class Program {

        static void Main
                     ( string[] args ) {

            Database.SetInitializer( new DataModelDbContextInitializer() );


            HostFactory.Run( hostConfiguration => {

                hostConfiguration.Service<Service>( serviceConfiguration => {

                    serviceConfiguration.ConstructUsing( settings => new Service() );
                    serviceConfiguration.WhenStarted( tc => tc.Start() );
                    serviceConfiguration.WhenStopped( tc => tc.Stop() );

                });

                hostConfiguration.SetDescription( "Loads data into system" );
                hostConfiguration.SetDisplayName( "wipm.exchangestats.data.ingress.listener" );
                hostConfiguration.SetServiceName( "wipm.exchangestats.data.ingress.listener" );

            });

        }
    }
}
