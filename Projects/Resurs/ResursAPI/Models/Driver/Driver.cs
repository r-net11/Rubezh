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
			DriverParameters = new List<DriverParameter>();
			DefaultTariffType = TariffType.Electricity;
			CanEditTariffType = true;
		}
		public DriverType DriverType { get; set; }
		public DeviceType DeviceType { get; set; }
		public List<DriverType> Children { get; set; }
		public List<DriverParameter> DriverParameters { get; set; }
		public TariffType DefaultTariffType { get; set; }
		public bool CanEditTariffType { get; set; }
	}
}