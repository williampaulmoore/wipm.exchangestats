using System;
using System.Data.Entity;
using System.Linq;
using wipm.exchangestats.audit.core;

namespace wipm.exchangestats.audit.listener {

    public class DataModelDbContext
                   : DbContext
                   , AuditDataModel {


        public RequestModelSet RequestModels {
            get {
                return requestModels 
                    ?? ( requestModels = new RequestModelSetAdapter( Requests ) );
            }
        }
        private RequestModelSetAdapter requestModels;

        public DbSet<RequestModel> Requests { get; set; }



        public UnhandledMessageModelSet UnhandledMessageModels {
            get {
                return unhandledMessageModels 
                    ?? ( unhandledMessageModels = new UnhandledMessageModelSetAdapter( UnhandledMessages ) );
           }
        }
        private UnhandledMessageModelSet unhandledMessageModels;

        public DbSet<UnhandledMessageModel> UnhandledMessages { get; set; }


        public DbSet<SeenMessageModel> SeenMessages { get; set; }

        public void AddMessageAsSeen
                      ( Guid messageId ) {

            if ( messageId == Guid.Empty ) throw new ArgumentException( nameof( messageId ) );


            var seenMessage 
                  = new SeenMessageModel {
                        MessageId = messageId
                    };

            SeenMessages.Add( seenMessage );

        }

        public bool HasSeenMessage
                      ( Guid messageId ) {

            return 
              SeenMessages.Any( m => m.MessageId == messageId );
        }

        protected override void OnModelCreating
                                 ( DbModelBuilder modelBuilder ) {
        
            base.OnModelCreating( modelBuilder );

            modelBuilder.ComplexType<MessageDescriptionModel>();
        }

    }
}
