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
		public bool IsAUParameter { get; set; }

		[DataMember]
		public byte No { get; set; }

		[DataMember]
		public int BitOffset { get; set; }

		[DataMember]
		public int MinBit { get; set; }

		[DataMember]
		public int MaxBit { get; set; }

		[DataMember]
		public ushort Min { get; set; }

		[DataMember]
		public ushort Max { get; set; }

		[DataMember]
		public bool UseMask { get; set; }

		[DataMember] 
		public bool HighByte { get; set; }

		[DataMember]
		public bool MptHighByte { get; set; }

		[DataMember]
		public bool MptLowByte { get; set; }
    }
}