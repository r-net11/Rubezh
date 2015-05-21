﻿using System.Collections.Generic;
using PowerCalculator.Models;

namespace PowerCalculator.Processor
{
	public class DriversHelper
	{
		public static List<Driver> Drivers { get; private set; }

		static DriversHelper()
		{
			Drivers = new List<Driver>();

			//Drivers.Add(new Driver()
			//    {
			//        DriverType = DriverType.Unknown,
			//        Mult = 1,
			//        R = 0.8,
			//        I = 0.21,
			//        U = 0,
			//        DeviceType = AlsDeviceType.Consumer,
			//        Umin = 12,
			//        Imax = 250
			//    });

			Drivers.Add(new Driver()
				{
					DriverType = DriverType.KAU,
					Mult = 1,
					R = 0.8,
					I = 0,
					U = 24,
					DeviceType = AlsDeviceType.Supplier,
					Umin = 0,
					Imax = 250
				});

			Drivers.Add(new Driver()
				{
					DriverType = DriverType.IPD,
					Mult = 1,
					R = 0.8,
					I = 0.21,
					U = 0,
					DeviceType = AlsDeviceType.Consumer,
					Umin = 9,
					Imax = 300
				});

			Drivers.Add(new Driver()
				{
					DriverType = DriverType.IPT,
					Mult = 1,
					R = 0.8,
					I = 0.21,
					U = 0,
					DeviceType = AlsDeviceType.Consumer,
					Umin = 9,
					Imax = 300
				});

			Drivers.Add(new Driver()
				{
					DriverType = DriverType.IPK,
					Mult = 1,
					R = 0.8,
					I = 0.21,
					U = 0,
					DeviceType = AlsDeviceType.Consumer,
					Umin = 9,
					Imax = 300
				});

			Drivers.Add(new Driver()
				{
					DriverType = DriverType.IPR,
					Mult = 1,
					R = 0.8,
					I = 0.21,
					U = 0,
					DeviceType = AlsDeviceType.Consumer,
					Umin = 9,
					Imax = 300
				});

			Drivers.Add(new Driver()
				{
					DriverType = DriverType.MDU,
					Mult = 1,
					R = 0.8,
					I = 0.21,
					U = 0,
					DeviceType = AlsDeviceType.Consumer,
					Umin = 12,
					Imax = 300
				});

			Drivers.Add(new Driver()
				{
					DriverType = DriverType.MA4,
					Mult = 4,
					R = 0.8,
					I = 0.21,
					U = 0,
					DeviceType = AlsDeviceType.Consumer,
					Umin = 12,
					Imax = 300
				});

			Drivers.Add(new Driver()
				{
					DriverType = DriverType.MAP4,
					Mult = 4,
					R = 0.8,
					I = 0.21,
					U = 0,
					DeviceType = AlsDeviceType.Consumer,
					Umin = 10,
					Imax = 300
				});

			Drivers.Add(new Driver()
				{
					DriverType = DriverType.MR2,
					Mult = 2,
					R = 0.8,
					I = 0.21,
					U = 0,
					DeviceType = AlsDeviceType.Consumer,
					Umin = 12,
					Imax = 300
				});

			Drivers.Add(new Driver()
				{
					DriverType = DriverType.MR4,
					Mult = 4,
					R = 0.8,
					I = 0.21,
					U = 0,
					DeviceType = AlsDeviceType.Consumer,
					Umin = 12,
					Imax = 300
				});

			Drivers.Add(new Driver()
				{
					DriverType = DriverType.MVK8,
					Mult = 8,
					R = 0.8,
					I = 0.21,
					U = 0,
					DeviceType = AlsDeviceType.Consumer,
					Umin = 10,
					Imax = 300
				});

			Drivers.Add(new Driver()
				{
					DriverType = DriverType.MP,
					Mult = 1,
					R = 0.8,
					I = 0.21,
					U = 24,
					DeviceType = AlsDeviceType.Supplier,
					Umin = 12,
					Imax = 300
				});

			Drivers.Add(new Driver()
				{
					DriverType = DriverType.MVP,
					Mult = 1,
					R = 0.8,
					I = 0.21,
					U = 24,
					DeviceType = AlsDeviceType.Supplier,
					Umin = 12,
					Imax = 300
				});

			Drivers.Add(new Driver()
				{
					DriverType = DriverType.OPS,
					Mult = 1,
					R = 0.8,
					I = 5,
					U = 0,
					DeviceType = AlsDeviceType.Consumer,
					Umin = 12,
					Imax = 300
				});

			Drivers.Add(new Driver()
				{
					DriverType = DriverType.OPZ,
					Mult = 1,
					R = 0.8,
					I = 30,
					U = 0,
					DeviceType = AlsDeviceType.Consumer,
					Umin = 15,
					Imax = 300
				});

			Drivers.Add(new Driver()
				{
					DriverType = DriverType.OPK,
					Mult = 1,
					R = 0.8,
					I = 35,
					U = 0,
					DeviceType = AlsDeviceType.Consumer,
					Umin = 15,
					Imax = 300
				});
		}
	}
}