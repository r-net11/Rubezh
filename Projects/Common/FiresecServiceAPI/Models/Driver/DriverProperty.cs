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
			IsHeighByte = true;
			IsLowByte = false;
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
		public int Offset { get; set; }

		[DataMember]
		public int MinOffset { get; set; }

		[DataMember]
		public int MaxOffset { get; set; }

		[DataMember]
		public List<string> AlternativePareterNames { get; set; }

		[DataMember]
		public ushort Min { get; set; }

		[DataMember]
		public ushort Max { get; set; }

		[DataMember]
		public bool IsHeighByte { get; set; }

		[DataMember]
		public bool IsLowByte { get; set; }
    }
}