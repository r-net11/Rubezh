using System.Collections.Generic;
using System.Runtime.Serialization;

namespace XFiresecAPI
{
    [DataContract]
    public class XDriversConfiguration
    {
        public XDriversConfiguration()
        {
            XDrivers = new List<XDriver>();
        }

        [DataMember]
        public List<XDriver> XDrivers { get; set; }
    }
}