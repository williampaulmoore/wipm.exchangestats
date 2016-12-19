namespace wipm.exchangestats.data.ingress.listener {

    /// <summary>
    /// Factory that creates a DataModelDbContext version of the 
    /// ServiceDataModel
    /// </summary>
    class DataModelDbContextFactory
           : ServiceDataModelFactory {

        public ServiceDataModel CreateServiceDataModel() {
            var context = new DataModelDbContext();

            return context;
        }
    }
}
