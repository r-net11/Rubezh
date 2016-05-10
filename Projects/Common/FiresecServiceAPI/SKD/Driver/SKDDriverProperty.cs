using System.Collections.Generic;

namespace StrazhAPI.SKD
{
	public class SKDDriverProperty
	{
		public SKDDriverProperty()
		{
			Parameters = new List<SKDDriverPropertyParameter>();
		}

		public byte No { get; set; }

		public string Name { get; set; }

		public string Caption { get; set; }

		public string ToolTip { get; set; }

		public int Default { get; set; }

		public string StringDefault { get; set; }

		public List<SKDDriverPropertyParameter> Parameters { get; set; }

		public SKDDriverType DriverPropertyType { get; set; }

		public int Min { get; set; }

		public int Max { get; set; }
	}
}