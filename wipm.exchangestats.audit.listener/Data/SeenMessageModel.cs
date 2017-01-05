using System;
using System.ComponentModel.DataAnnotations;

namespace wipm.exchangestats.audit.listener {

    public class SeenMessageModel {

        [Key]
        public Guid MessageId { get; set; }

    }
}
