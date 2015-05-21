using System.Collections.Generic;

namespace PowerCalculator.Models
{
	public class Configuration
	{
		public Configuration()
		{
			Lines = new List<Line>();
			DeviceRepositoryItems = new List<DeviceRepositoryItem>();
			CableRepositoryItems = new List<CableRepositoryItem>();
		}

		public List<Line> Lines { get; set; }
		public List<DeviceRepositoryItem> DeviceRepositoryItems { get; set; }
		public List<CableRepositoryItem> CableRepositoryItems { get; set; }
	}
}