using System.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wipm.exchangestats.data.ingress.listener {

    class DataModelDbContextInitializer 
            : DropCreateDatabaseIfModelChanges<DataModelDbContext>  { }
}
