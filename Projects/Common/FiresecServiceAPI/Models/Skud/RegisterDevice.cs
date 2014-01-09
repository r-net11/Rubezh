using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiresecAPI.Models.Skud
{
	public class RegisterDevice
	{
		public Guid Uid { get; set; }
		public bool CanControl { get; set; }
	}
}
