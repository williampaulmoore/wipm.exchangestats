using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace wipm.exchangestats.audit.core {


    public class RequestModel {

        [Key]
        public virtual Guid RequestId { get; set; }

        public virtual MessageDescriptionModel InitiatedFrom { get; internal set; }

        public virtual DateTime FirstRecordedAt { get; internal set; }

        public virtual DateTime UpdatedAt { get; internal set; }

        public virtual ICollection<RequestUpdateModel> Updates {
            get { return updates ?? ( updates = new Collection<RequestUpdateModel>() ) ; }
            set { updates = value; }
        }
        private ICollection<RequestUpdateModel> updates;
    }
}
