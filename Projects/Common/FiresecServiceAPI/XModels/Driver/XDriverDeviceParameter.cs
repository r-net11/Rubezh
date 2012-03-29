using System.Collections.Generic;
using System.Runtime.Serialization;

namespace XFiresecAPI
{
    [DataContract]
    public class XDriverDeviceParameter
    {
        public XDriverDeviceParameter()
        {
            Parameters = new List<XDriverPropertyParameter>();
        }

        [DataMember]
        public byte No { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Default { get; set; }

        [DataMember]
        public List<XDriverPropertyParameter> Parameters { get; set; }

        [DataMember]
        public XDriverPropertyTypeEnum DriverPropertyType { get; set; }
    }
}