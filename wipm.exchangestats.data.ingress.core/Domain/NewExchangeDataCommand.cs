using lqd.net.functional;
using System;

namespace wipm.exchangestats.data.ingress.core {

    public interface ExchangeDataCommand {

        Either<StateChangeError,StateChangeEvent> Execute();

    }

}
