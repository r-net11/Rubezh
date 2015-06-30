using System.Collections.Generic;

namespace PowerCalculator.Models
{
	public class Configuration
	{
		public Configuration()
		{
			Lines = new List<Line>();
			DeviceSpecificationItems = new List<DeviceSpecificationItem>();
			CableSpecificationItems = new List<CableSpecificationItem>();
		}

		public List<Line> Lines { get; set; }
		public List<DeviceSpecificationItem> DeviceSpecificationItems { get; set; }
		public List<CableSpecificationItem> CableSpecificationItems { get; set; }
	}
}