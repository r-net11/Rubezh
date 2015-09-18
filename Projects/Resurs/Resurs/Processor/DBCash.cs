using ResursAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resurs.Processor
{
	public static class DBCash
	{
		static DBCash()
		{
			RootDevice = new Device()
			{
				Name = "Система",
				DriverType = DriverType.System,
				Children = new List<Device>()
				{
					new Device()
					{
						Name = "Сеть 1",
						DriverType = DriverType.Network1,
						Children = new List<Device>()
						{
							new Device() {Name = "Счетчик 1", DriverType = DriverType.Network1Device1},
							new Device() {Name = "Счетчик 2", DriverType = DriverType.Network1Device1},
							new Device() {Name = "Счетчик 3", DriverType = DriverType.Network1Device2},
						}
					},
					new Device()
					{
						Name = "Сеть 2",
						DriverType = DriverType.Network2,
						Children = new List<Device>()
						{
							new Device() {Name = "Счетчик 10", DriverType = DriverType.Network2Device1},
							new Device() {Name = "Счетчик 20", DriverType = DriverType.Network2Device2},
							new Device() {Name = "Счетчик 30", DriverType = DriverType.Network2Device3},
						}
					}
				}
			};
		}

		public static Device RootDevice { get; set; }
	}
}