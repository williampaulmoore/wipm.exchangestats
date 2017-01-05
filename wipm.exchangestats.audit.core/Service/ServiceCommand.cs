namespace wipm.exchangestats.audit.core {


    public interface ServiceCommand {

        void Handle
               ( ServiceRequest serviceRequest );
    }
}
