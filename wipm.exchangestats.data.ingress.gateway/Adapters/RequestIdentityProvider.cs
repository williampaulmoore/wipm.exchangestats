using System;

namespace wipm.exchangestats.data.ingress.gateway {

    public class RequestIdentityProvider
                  : IRequestIdentityProvider {

        public Guid GetId() {

            return Guid.NewGuid();
        }
    }
}