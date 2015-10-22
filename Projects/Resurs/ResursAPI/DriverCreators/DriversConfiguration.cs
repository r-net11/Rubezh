using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResursAPI
{
	public static class DriversConfiguration
	{
		static DriversConfiguration()
		{
			Drivers = new List<Driver>();
			Drivers.Add(SystemCreator.Create());
			Drivers.Add(BeregunNetworkCreator.Create());
			Drivers.Add(BeregunCounterCreator.Create());
			Drivers.Add(MZEP55CounterCreator.Create());
			Drivers.Add(MZEP55NetworkCreator.Create());
			Drivers.Add(IncotexNetworkCreator.Create());
			Drivers.Add(Mercury203CounterCreator.Create());
			Drivers.Add(VirtualIncotextNetworkCreator.Create());
			Drivers.Add(VirtualMercury203CounterCreator.Create());
			Drivers.Add(VirtualMZEP55CounterCreator.Create());
			Drivers.Add(VirtualMZEP55NetworkCreator.Create());

			CheckUIDs();
		}

		static void CheckUIDs()
		{
			var dictionary = new Dictionary<Guid, string>();
			foreach (var driver in Drivers)
			{
				dictionary.Add(driver.UID, driver.DriverType.ToDescription());
				var parameterDictionary = new Dictionary<Guid, string>();
				foreach (var parameter in driver.DriverParameters)
				{
					parameterDictionary.Add(parameter.UID, parameter.Name);
				}
				var commandDictionary = new Dictionary<Guid, string>();
				foreach (var parameter in driver.Commands)
				{
					commandDictionary.Add(parameter.UID, parameter.Name);
				}
			}
		}

		public static List<Driver> Drivers { get; private set; }

		public static Driver GetDriver(DriverType driverType)
		{
			var driver = Drivers.FirstOrDefault(x => x.DriverType == driverType);
			if (driver == null)
				throw new NullReferenceException();
			return driver;
		}
	}
}