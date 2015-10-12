using System.Collections.Generic;

namespace RubezhAPI.GK
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