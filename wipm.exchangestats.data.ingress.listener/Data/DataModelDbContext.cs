using System;
using System.Data.Entity;
using System.Linq;
using wipm.exchangestats.data.ingress.core;
using System.Collections.Generic;

namespace wipm.exchangestats.data.ingress.listener {


    /// <summary>
    /// Data model that implements the service and core datamodels
    /// </summary>
    class DataModelDbContext
            : DbContext
            , CoreDataModel {

        public ExchangeModelSet ExchangeModels {
            get {
                return exchagenModels ?? ( exchagenModels = new ExchangeModelSetAdapter( Exchanges ) );
            }
        }
        private ExchangeModelSet exchagenModels;
            

        public void WriteMessage( Message message ) {

            if ( message == null ) throw new ArgumentNullException( nameof( message ) );


            var model = new MessageModel {
                 RequestId = message.RequestId
                ,Messsage = message.Body
                ,Published = false
            };

            Messages.Add( model );
        }

        public void WriteMessageOutcome( MessageOutcome messageOutcome ) {
            
            var message 
                 = Messages.Find( messageOutcome.RequestId );

            if ( message == null ) throw new MessageNotFoundException( messageOutcome.RequestId );


            message.OutcomeType = messageOutcome.OutcomeType;
            message.Outcome = messageOutcome.Outcome;
            
        }

        public void SetMessageToPublished( Guid requestId ) {

            var message 
                 = Messages.Find( requestId );

            if ( message == null ) throw new MessageNotFoundException( requestId );

            message.Published = true;
        }

        public bool HasMessage( Guid requestId ) {
            
            return Messages.Any( m => m.RequestId == requestId );
        }

        public IEnumerable<MessageOutcome> GetUnpublishedOutcomes() {

            return 
              Messages
                .Where( m => !m.Published )
                .ToList()
                .Select( m => 
                    new MessageOutcome (
                       m.RequestId
                      ,m.OutcomeType
                      ,m.Outcome
                    )
                );
        }

        public DbSet<ExchangeModel> Exchanges { get; set; }

        public DbSet<MessageModel> Messages { get; set; }

    }


    public class MessageNotFoundException 
                   : Exception {

        public MessageNotFoundException( Guid requestId ) 
                : base( $"Messages not found for request :{requestId}" ) {}

    }

}
