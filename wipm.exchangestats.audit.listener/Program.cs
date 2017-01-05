using System.Data.Entity;
using Topshelf;


namespace wipm.exchangestats.audit.listener {

    class Program {

        static void Main
                      ( string[] args ) {

            Database.SetInitializer( new DataModelDbContextInitializer() );


            HostFactory.Run( hostConfiguration => {

                hostConfiguration.Service<Service>( serviceConfiguration => {

                    serviceConfiguration.ConstructUsing( settings => {

                        return new Service();
                    });
                    serviceConfiguration.WhenStarted( tc => tc.Start() );
                    serviceConfiguration.WhenStopped( tc => tc.Stop() );

                });

                hostConfiguration.SetDescription( "Updates the audit trails from the requests" );
                hostConfiguration.SetDisplayName( "wipm.exchangestats.audit.listener" );
                hostConfiguration.SetServiceName( "wipm.exchangestats.audit.listener" );

            });

        }
    }
}
