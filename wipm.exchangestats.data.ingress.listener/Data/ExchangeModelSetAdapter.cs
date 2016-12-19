using System;
using System.Collections.Generic;
using System.Data.Entity;
using wipm.exchangestats.data.ingress.core;

namespace wipm.exchangestats.data.ingress.listener {


    /// <summary>
    /// Adapts a DbSet to ExchangeModels set, 
    /// </summary>
    class ExchangeModelSetAdapter 
            : ExchangeModelSet {

        public IEnumerable<ExchangeModel> All {
            get {
                return exchangeSet;
            }
        }

        public void Add
                      ( ExchangeModel exchange ) {
            
            if (  exchange == null ) throw new ArgumentNullException( nameof( exchange  ) );


            exchangeSet.Add( exchange );
        }


        public ExchangeModelSetAdapter
                 ( DbSet<ExchangeModel> exchangeSet ) {

            if ( exchangeSet == null ) throw new ArgumentNullException( nameof( exchangeSet ) );


            this.exchangeSet = exchangeSet;
        }

        private readonly DbSet<ExchangeModel> exchangeSet;

    }
}
