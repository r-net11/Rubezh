using System.Collections.Generic;

namespace XFiresecAPI
{
	public class XDriverDeviceParameter
	{
		public XDriverDeviceParameter()
		{
			Parameters = new List<XDriverPropertyParameter>();
		}

		public byte No { get; set; }
		public string Name { get; set; }
		public string Default { get; set; }
		public List<XDriverPropertyParameter> Parameters { get; set; }
		public XDriverPropertyTypeEnum DriverPropertyType { get; set; }
	}
}