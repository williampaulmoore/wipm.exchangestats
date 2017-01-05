using System.Collections.Generic;

namespace wipm.exchangestats.audit.core {

    public interface UnhandledMessageModelSet {

        IEnumerable<UnhandledMessageModel> All { get; }

        void Add( UnhandledMessageModel request );

    }
}
