using System.Collections.Generic;

namespace FiresecAPI.GK
{
	public class GKDriversConfiguration
	{
		public GKDriversConfiguration()
		{
			Drivers = new List<GKDriver>();
		}

		public List<GKDriver> Drivers { get; set; }
	}
}