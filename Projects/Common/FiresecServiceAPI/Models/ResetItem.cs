using System.Collections.Generic;

namespace FiresecAPI.Models
{
	public class ResetItem
	{
		public ResetItem()
		{
			States = new List<DeviceDriverState>();
		}
		public DeviceState DeviceState { get; set; }
		public List<DeviceDriverState> States { get; set; }
	}
}