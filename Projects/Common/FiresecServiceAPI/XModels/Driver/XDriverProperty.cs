using System.Collections.Generic;
using System.Runtime.Serialization;

namespace XFiresecAPI
{
    [DataContract]
    public class XDriverProperty
    {
        public XDriverProperty()
        {
            Parameters = new List<XDriverPropertyParameter>();
        }

        [DataMember]
        public byte No { get; set; }

        [DataMember]
        public bool IsInternalDeviceParameter { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Caption { get; set; }

        [DataMember]
        public string ToolTip { get; set; }

        [DataMember]
        public object Default { get; set; }

        [DataMember]
        public List<XDriverPropertyParameter> Parameters { get; set; }

        [DataMember]
        public XDriverPropertyTypeEnum DriverPropertyType { get; set; }
    }
}