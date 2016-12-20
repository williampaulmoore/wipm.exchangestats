using System;
using System.Collections.Generic;
using wipm.exchangestats.data.ingress.core;

namespace wipm.exchangestats.data.ingress.listener {

    public interface ServiceDataModel
                      : CoreDataModel
                      , IDisposable {

        IEnumerable<MessageOutcome> GetUnpublishedOutcomes();


        void WriteMessage( Message message );
        void WriteMessageOutcome( MessageOutcome messageOutcome );
        void SetMessageToPublished( Guid requestId );
        bool HasMessage( Guid requestId );

        void Commit();
    }


    public interface ServiceDataModelFactory {

        ServiceDataModel CreateServiceDataModel();

    }
}
