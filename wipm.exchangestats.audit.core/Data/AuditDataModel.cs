namespace wipm.exchangestats.audit.core {

    public interface AuditDataModel {

        RequestModelSet RequestModels { get; }

        UnhandledMessageModelSet UnhandledMessageModels { get; }

    }

}
