using System.Data.Entity;

namespace wipm.exchangestats.data.ingress.listener {

    class DataModelDbContextInitializer 
            : DropCreateDatabaseIfModelChanges<DataModelDbContext>  { }
}
