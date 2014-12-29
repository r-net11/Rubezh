using System.Collections.Generic;

namespace FiresecAPI.GK
{
	public class GKDriverDeviceParameter
	{
		public GKDriverDeviceParameter()
		{
			Parameters = new List<GKDriverPropertyParameter>();
		}

		public byte No { get; set; }
		public string Name { get; set; }
		public string Default { get; set; }
		public List<GKDriverPropertyParameter> Parameters { get; set; }
		public GKDriverPropertyTypeEnum DriverPropertyType { get; set; }
	}
}