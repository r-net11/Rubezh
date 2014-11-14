using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Automation;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
    [DataContract]
    public class ShowDialogArguments : UIArguments
    {
        [DataMember]
        public Guid Layout { get; set; }
        [DataMember]
        public bool IsModal { get; set; }
    }
}
