using System;

namespace Infrastructure.Models
{
	public class ElementDeviceReference
	{
		public Guid DeviceUID { get; set; }
		public Guid AlternativeUID { get; set; }
	}
}