using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Configuration;
using System.Data.Entity;
using Topshelf;
using wipm.exchangestats.data.ingress.interfaces;

namespace wipm.exchangestats.data.ingress.listener {

    public class Program {

        static void Main
                     ( string[] args ) {

            Database.SetInitializer(new DataModelDbContextInitializer());


            HostFactory.Run( hostConfiguration => {

                hostConfiguration.Service<Service>( serviceConfiguration => {

                    serviceConfiguration.ConstructUsing( settings => {

                        var connectionString 
                              = ConfigurationManager.AppSettings[ "ingress_gateway_service_bus_connection_string" ];

                        var namespaceManager 
                              = NamespaceManager.CreateFromConnectionString( connectionString );

                        var ingressTopic 
                              = namespaceManager.GetTopic( Topics.IngressGatewayTopicName );

                        var ingressTopicSubscription 
                              = SubscriptionClient.CreateFromConnectionString(
                                     connectionString
                                    ,Topics.IngressGatewayTopicName
                                    ,"dataIngressListener"
                                );

                        return new Service(
                            messageSourceTopic: ingressTopic
                           ,messageSourceSubscription : ingressTopicSubscription
                           ,outcomeSink : new AzureServiceBusIngressDataTopic()
                           ,dataModelFactory : new DataModelDbContextFactory()
                        );
                    });
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
