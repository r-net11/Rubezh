using ResursAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resurs.Processor
{
	public static class DBCash
	{

		public static Device RootDevice { get; set; }
		public static Apartment RootApartment { get; set; }

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

			RootApartment = new Apartment()
			{
				Name = "Жилой комплекс",
				Children = new List<Apartment>()
				{
					new Apartment()
					{
						Name = "Дом 1",
						Children = new List<Apartment>()
						{
							new Apartment() {Name = "Квартира 1"},
							new Apartment() {Name = "Квартира 2"},
							new Apartment() {Name = "Квартира 3"},
							new Apartment() {Name = "Квартира 4"},
							new Apartment() {Name = "Квартира 5"},
							new Apartment() {Name = "Квартира 6"},
							new Apartment() {Name = "Квартира 7"},
							new Apartment() {Name = "Квартира 8"},
						}
					},
					new Apartment()
					{
						Name = "Дом 2",
						Children = new List<Apartment>()
						{
							new Apartment() {Name = "Квартира 1"},
							new Apartment() {Name = "Квартира 2"},
							new Apartment() {Name = "Квартира 3"},
							new Apartment() {Name = "Квартира 4"},
							new Apartment() {Name = "Квартира 5"},
							new Apartment() {Name = "Квартира 6"},
							new Apartment() {Name = "Квартира 7"},
							new Apartment() {Name = "Квартира 8"},
							new Apartment() {Name = "Квартира 9"},
							new Apartment() {Name = "Квартира 10"},
							new Apartment() {Name = "Квартира 11"},
							new Apartment() {Name = "Квартира 12"},
							new Apartment() {Name = "Квартира 13"},
							new Apartment() {Name = "Квартира 14"},
							new Apartment() {Name = "Квартира 15"},
							new Apartment() {Name = "Квартира 66"},
						}
					}
				}
			};
		}
	}
}