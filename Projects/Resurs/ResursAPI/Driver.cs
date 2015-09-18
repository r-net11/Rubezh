using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResursAPI
{
	public class Driver
	{
		public Driver()
		{
			Children = new List<DriverType>();
		}

		public DriverType DriverType { get; set; }
		public List<DriverType> Children { get; set; }
	}
}