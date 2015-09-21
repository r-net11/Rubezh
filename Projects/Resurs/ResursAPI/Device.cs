using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResursAPI
{
	public class Device : ModelBase
	{
		public Device()
		{
			Children = new List<Device>();
		}

		public List<Device> Children { get; set; }

		public DriverType DriverType { get; set; }
		public Driver Driver { get; set; }
	}
}