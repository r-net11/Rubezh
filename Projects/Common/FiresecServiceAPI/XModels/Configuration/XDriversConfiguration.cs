using System.Collections.Generic;
using System.Runtime.Serialization;

namespace XFiresecAPI
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