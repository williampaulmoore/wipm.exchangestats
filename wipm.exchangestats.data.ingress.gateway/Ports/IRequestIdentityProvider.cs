using System;

namespace wipm.exchangestats.data.ingress.gateway {

    // Identity provider for messages that are posted onto the data ingress request queue
    public interface IRequestIdentityProvider {

        Guid GetId();

    }
}
