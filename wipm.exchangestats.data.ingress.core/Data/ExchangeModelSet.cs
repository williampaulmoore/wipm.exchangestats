using System.Collections.Generic;

namespace wipm.exchangestats.data.ingress.core {

    public interface ExchangeModelSet {

        IEnumerable<ExchangeModel> All { get; }

        void Add( ExchangeModel exchange );

    }

}
