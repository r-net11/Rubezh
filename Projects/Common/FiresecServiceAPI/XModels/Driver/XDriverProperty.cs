using System.Collections.Generic;

namespace XFiresecAPI
{
	public class XDriverProperty
	{
		public XDriverProperty()
		{
			Parameters = new List<XDriverPropertyParameter>();
			IsAUParameter = true;
			Mask = 0;
		}

		public byte No { get; set; }
		public bool IsReadOnly { get; set; }
		public bool IsAUParameter { get; set; }
		public string Name { get; set; }
		public string Caption { get; set; }
		public string ToolTip { get; set; }
		public ushort Default { get; set; }
		public string StringDefault { get; set; }
		public bool IsHieghByte { get; set; }
		public bool IsLowByte { get; set; }
		public short Mask { get; set; }
		public bool HighByte { get; set; }
		public List<XDriverPropertyParameter> Parameters { get; set; }
		public XDriverPropertyTypeEnum DriverPropertyType { get; set; }
		public ushort Min { get; set; }
		public ushort Max { get; set; }
		public double Multiplier { get; set; }
		public bool IsMPTOrMRORegime { get; set; }
	}
}