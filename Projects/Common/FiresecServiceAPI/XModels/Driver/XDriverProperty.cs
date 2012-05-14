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
			AlternativePareterNames = new List<string>();
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
		public bool IsHighByte { get; set; }

		[DataMember]
		public byte Offset { get; set; }

		[DataMember]
		public List<XDriverPropertyParameter> Parameters { get; set; }

		[DataMember]
		public List<string> AlternativePareterNames { get; set; }

		[DataMember]
		public XDriverPropertyTypeEnum DriverPropertyType { get; set; }

		[DataMember]
		public short Min { get; set; }

		[DataMember]
		public short Max { get; set; }
	}
}