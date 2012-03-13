using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace GroupControllerModule.Models
{
    [DataContract]
    public class XDriversConfiguration
    {
        public XDriversConfiguration()
        {
            Drivers = new List<XDriver>();
        }

        [DataMember]
        public List<XDriver> Drivers { get; set; }
    }
}