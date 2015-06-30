using System.Collections.Generic;

namespace FiresecAPI.GK
{
	public class GKDriverProperty
	{
		public GKDriverProperty()
		{
			Parameters = new List<GKDriverPropertyParameter>();
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
		public List<GKDriverPropertyParameter> Parameters { get; set; }
		public GKDriverPropertyTypeEnum DriverPropertyType { get; set; }
		public ushort Min { get; set; }
		public ushort Max { get; set; }
		public double Multiplier { get; set; }
		public bool CanNotEdit { get; set; }
	}
}