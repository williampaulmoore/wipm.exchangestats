using System.ComponentModel.DataAnnotations;


namespace wipm.exchangestats.data.ingress.core {

    public class ExchangeModel {

        [Key]
        public string Code { get; set; }

        public string Name { get; set; }
    }

}
