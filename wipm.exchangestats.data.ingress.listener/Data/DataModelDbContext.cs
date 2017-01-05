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
            , DataIngressDataModel {

        public ExchangeModelSet ExchangeModels {
            get {
                return exchagenModels ?? ( exchagenModels = new ExchangeModelSetAdapter( Exchanges ) );
            }
        }
        private ExchangeModelSet exchagenModels;

        
        public bool HasMessage( Guid messageId ) {
            
            return Messages.Any( m => m.MessageId == messageId );
        }

        public void WriteMessage
                     ( Message message 
                     , Guid OutcomeMessageId ) {

            if ( message == null ) throw new ArgumentNullException( nameof( message ) );


            var model = new MessageModel {
                 RequestId = message.RequestId
                ,MessageId = message.MessageId
                ,Messsage = message.Body
                ,Published = false
                ,OutcomeMessageId = OutcomeMessageId
            };

            Messages.Add( model );

            SaveChanges();
        }


        public void WriteMessageOutcome( Message message ) {
            
            var messageModel 
                  = Messages.Single( m => m.OutcomeMessageId == message.MessageId );

            if ( messageModel == null ) throw new OuctomeMessageNotFoundException( message.MessageId );


            messageModel.OutcomeType = message.MessageType;
            messageModel.Outcome = message.Body;
            
            SaveChanges();

        }

        public void SetOutcomeToPublished( Guid messageId ) {

            var message 
                  = Messages.Single( m => m.OutcomeMessageId == messageId );

            if ( message == null ) throw new MessageNotFoundException( messageId );

            message.Published = true;

            SaveChanges();
        }


        public IEnumerable<Message> GetUnpublishedMessages() {

            return 
              Messages
                .Where( m => !m.Published )
                .ToList()
                .Select( m => 
                    new Message (
                       m.RequestId
                      ,m.OutcomeMessageId
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

        public MessageNotFoundException( Guid messageId ) 
                : base( $"Messages not found for :{messageId}" ) {}

    }

    public class OuctomeMessageNotFoundException
                   : Exception {

        public OuctomeMessageNotFoundException( Guid messageId ) 
                : base( $"Outcome messages not found for :{messageId}" ) {}



    } 

}
