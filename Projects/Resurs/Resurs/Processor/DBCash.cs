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
			RootDevice = new Device(DriverType.System);
			var interface1 = new Device(DriverType.BeregunInterface);
			interface1.AddChild(DriverType.BeregunCounter);
			interface1.AddChild(DriverType.BeregunCounter);
			interface1.AddChild(DriverType.BeregunCounter);
			RootDevice.AddChild(interface1);
			var interface2 = new Device(DriverType.MZEP55Interface);
			interface2.AddChild(DriverType.MZEP55Counter);
			interface2.AddChild(DriverType.MZEP55Counter);
			interface2.AddChild(DriverType.MZEP55Counter);
			RootDevice.AddChild(interface2);
			
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
							new Apartment() {Name = "Квартира 666"},
						}
					}
				}
			};
		}
	}
}