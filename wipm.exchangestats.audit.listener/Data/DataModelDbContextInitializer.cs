using System.Data.Entity;

namespace wipm.exchangestats.audit.listener {

    class DataModelDbContextInitializer 
            : DropCreateDatabaseIfModelChanges<DataModelDbContext>  { }
}
