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
			AlternativePareterNames = new List<string>();
        }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Caption { get; set; }

        [DataMember]
        public string ToolTip { get; set; }

        [DataMember]
        public string Default { get; set; }

        [DataMember]
        public bool Visible { get; set; }

        [DataMember]
        public bool IsHidden { get; set; }

		[DataMember]
		public string BlockName { get; set; }

		[DataMember]
		public bool IsControl { get; set; }

        [DataMember]
        public List<DriverPropertyParameter> Parameters { get; set; }

        [DataMember]
        public DriverPropertyTypeEnum DriverPropertyType { get; set; }

		// свойства для конфигурации параметров устройств

		[DataMember]
		public bool IsInternalDeviceParameter { get; set; }

		[DataMember]
		public byte No { get; set; }

		[DataMember]
		public byte Offset { get; set; }

		[DataMember]
		public List<string> AlternativePareterNames { get; set; }

		[DataMember]
		public short Min { get; set; }

		[DataMember]
		public short Max { get; set; }
    }
}