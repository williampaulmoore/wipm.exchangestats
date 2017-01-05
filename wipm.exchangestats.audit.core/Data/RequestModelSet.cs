using System.Collections.Generic;

namespace wipm.exchangestats.audit.core {

    public interface RequestModelSet {


        IEnumerable<RequestModel> All { get; }

        void Add( RequestModel request );

    }

}
