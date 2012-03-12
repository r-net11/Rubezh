using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace GroupControllerModule.Models
{
    [DataContract]
    public class GCProperty
    {
        [DataMember]
        public string Name { get; set; }
    }
}