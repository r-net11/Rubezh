using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.Models
{
	public class ElementDeviceReference
	{
		public Guid DeviceUID { get; set; }
		public Guid AlternativeUID { get; set; }
	}
}
