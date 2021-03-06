﻿using System;
using System.ComponentModel.DataAnnotations;

namespace wipm.exchangestats.data.ingress.listener {

    public class MessageModel {

        [Key]
        public virtual Guid MessageId { get; set; }

        public virtual Guid RequestId { get; set; }
        
        public virtual string Messsage { get; set; }

        public virtual bool Published { get; set; }

        public virtual Guid OutcomeMessageId { get; set; }

        public virtual string OutcomeType { get; set; }

        // todo: rename ot OutcomMessage
        public virtual string Outcome { get; set; }
    }
}
