namespace wipm.exchangestats.data.ingress.core {

    /// <summary>
    /// General interface that all service commands should conform to
    /// </summary>
    public interface ServiceCommand {

        ServiceResponse Handle
                         ( ServiceRequest request );

    }
}
