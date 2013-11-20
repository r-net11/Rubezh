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
			IsAUParameter = true;
			Mask = 0;
		}
		[DataMember]
		public byte No { get; set; }

		[DataMember]
		public bool IsReadOnly { get; set; }

		[DataMember]
		public bool IsAUParameter { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Caption { get; set; }

		[DataMember]
		public string ToolTip { get; set; }

		[DataMember]
		public ushort Default { get; set; }

		[DataMember]
		public string StringDefault { get; set; }

		[DataMember]
		public bool IsHieghByte { get; set; }

		[DataMember]
		public bool IsLowByte { get; set; }

		[DataMember]
		public short Mask { get; set; }

		[DataMember]
		public bool HighByte { get; set; }

		[DataMember]
		public List<XDriverPropertyParameter> Parameters { get; set; }

		[DataMember]
		public XDriverPropertyTypeEnum DriverPropertyType { get; set; }

		[DataMember]
		public ushort Min { get; set; }

		[DataMember]
		public ushort Max { get; set; }

		[DataMember]
		public double Multiplier { get; set; }

		[DataMember]
		public bool IsMPTOrMRORegime { get; set; }
	}
}