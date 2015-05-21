
using System;
namespace FiresecAPI.GK
{
	public class CurrentConsumption
	{
		public CurrentConsumption()
		{
			UID = Guid.NewGuid();
		}

		public Guid UID { get; set; }
		public Guid KauUID { get; set; }
		public DateTime DateTime { get; set; }
		public int Current { get; set; }
	}

	public class CurrentConsumptionFilter
	{
		public Guid KauUID { get; set; }
		public DateTime StartDateTime { get; set; }
		public DateTime EndDateTime { get; set; }
	}
}
