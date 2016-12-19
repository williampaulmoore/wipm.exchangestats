namespace wipm.exchangestats.data.ingress.interfaces {


    // Representation of the excpected exchange data supplied to the 
    // ingress gateway.
    public class ExchangeData {

        public string Code { get; set; }

        public string Name { get; set; }

        public override bool Equals( object obj ) {

            var exchange = obj as ExchangeData;
            
            return exchange != null
                && this.Code.Equals( exchange.Code )
                && this.Name.Equals( exchange.Name )
                ;
        }

        public override int GetHashCode() {
            return (Code + Name).GetHashCode();
        }

    }
}
