using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResursAPI
{
	public class DeviceCommand
	{
		public DeviceCommand(Guid uid)
		{
			UID = uid;
		}

		public Guid UID { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
	}
}
