using System;
using System.ComponentModel.DataAnnotations;

namespace wipm.exchangestats.audit.core {


    public class UnhandledMessageModel {

        [Key]
        public virtual Guid MessageId  { get; set; }

        public virtual MessageDescriptionModel MessageDescription { get; set; }

        public virtual Guid RequestId { get; set; }

        public virtual string MessageBody { get; set; }

        public virtual DateTime RecorderAt { get; set; }

    }
}
