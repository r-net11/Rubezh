using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
	public class DeviceDriverState
	{
		public DriverState DriverState { get; set; }
		public DateTime? Time { get; set; }
	}
}