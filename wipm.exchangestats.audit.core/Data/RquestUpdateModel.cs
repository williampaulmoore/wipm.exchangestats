using System;
using System.ComponentModel.DataAnnotations;

namespace wipm.exchangestats.audit.core {

    public class RequestUpdateModel {

        [Key]
        public virtual Guid MessageId { get; set; }

        public virtual DateTime RecordedAt { get; set; }

        public virtual MessageDescriptionModel MessageDescription { get; set; }

        public virtual string MessageBody { get; set; }

    }
}
