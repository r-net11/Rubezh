using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [DataContract]
    public class DriverProperty
    {
        public DriverProperty()
        {
            Parameters = new List<DriverPropertyParameter>();
        }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Caption { get; set; }

        [DataMember]
        public string Default { get; set; }

        [DataMember]
        public bool Visible { get; set; }

        [DataMember]
        public bool IsHidden { get; set; }

        [DataMember]
        public List<DriverPropertyParameter> Parameters { get; set; }

        [DataMember]
        public DriverPropertyTypeEnum DriverPropertyType { get; set; }
    }
}