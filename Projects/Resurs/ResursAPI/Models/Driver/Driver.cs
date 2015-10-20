using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResursAPI
{
	public class Driver
	{
		public Driver(Guid uid)
		{
			UID = uid;
			Children = new List<DriverType>();
			DriverParameters = new List<DriverParameter>();
			Commands = new List<DeviceCommand>();
			DefaultTariffType = TariffType.Electricity;
			CanEditTariffType = true;
			MaxTariffParts = 8;
		}
		public Guid UID { get; set; }
		public DriverType DriverType { get; set; }
		public DeviceType DeviceType { get; set; }
		public List<DriverType> Children { get; set; }
		public List<DriverParameter> DriverParameters { get; set; }
		public List<DeviceCommand> Commands { get; set; }
		public TariffType DefaultTariffType { get; set; }
		public bool CanEditTariffType { get; set; }
		public int MaxTariffParts { get; set; }
	}
}