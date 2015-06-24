using System.Collections.Generic;
using System.Linq;
using PowerCalculator.Models;

namespace PowerCalculator.Processor
{
	public static class DriversHelper
	{
		public static List<Driver> Drivers { get; private set; }

        public static Driver GetDriver(DriverType driverType)
        {
            return Drivers.Where(x => x.DriverType == driverType).FirstOrDefault();
        }

		static DriversHelper()
		{
			Drivers = new List<Driver>();

			Drivers.Add(new Driver()
				{
					DriverType = DriverType.RSR2_KAU,
					CanAdd = false,
					Mult = 1,
					R = 0.8,
					I = 0,
					U = 24,
					DeviceType = DeviceType.Supplier,
					Umin = 0,
					Imax = 250
				});

			Drivers.Add(new Driver()
				{
					DriverType = DriverType.RSR2_SmokeDetector,
					Mult = 1,
					R = 0.8,
					I = 0.21,
					U = 0,
					DeviceType = DeviceType.Consumer,
					Umin = 9,
					Imax = 300
				});

			Drivers.Add(new Driver()
				{
					DriverType = DriverType.RSR2_HeatDetector, 
					Mult = 1,
					R = 0.8,
					I = 0.21,
					U = 0,
					DeviceType = DeviceType.Consumer,
					Umin = 9,
					Imax = 300
				});

			Drivers.Add(new Driver()
				{
					DriverType = DriverType.RSR2_CombinedDetector,
					Mult = 1,
					R = 0.8,
					I = 0.21,
					U = 0,
					DeviceType = DeviceType.Consumer,
					Umin = 9,
					Imax = 300
				});

			Drivers.Add(new Driver()
				{
					DriverType = DriverType.RSR2_HandDetector,
					Mult = 1,
					R = 0.8,
					I = 0.21,
					U = 0,
					DeviceType = DeviceType.Consumer,
					Umin = 9,
					Imax = 300
				});

			Drivers.Add(new Driver()
				{
					DriverType = DriverType.RSR2_MDU,
					Mult = 1,
					R = 0.8,
					I = 0.35,
					U = 0,
					DeviceType = DeviceType.Consumer,
					Umin = 12,
					Imax = 300
				});

            Drivers.Add(new Driver()
            {
                DriverType = DriverType.RSR2_MDU24,
                Mult = 1,
                R = 0.8,
                I = 0.35,
                U = 0,
                DeviceType = DeviceType.Consumer,
                Umin = 12,
                Imax = 300
            });

            Drivers.Add(new Driver()
            {
                DriverType = DriverType.RSR2_AM_1,
                Mult = 1,
                R = 0.8,
                I = 0.35,
                U = 0,
                DeviceType = DeviceType.Consumer,
                Umin = 12,
                Imax = 300
            });

            Drivers.Add(new Driver()
            {
                DriverType = DriverType.RSR2_AM_2,
                Mult = 2,
                R = 0.8,
                I = 0.35,
                U = 0,
                DeviceType = DeviceType.Consumer,
                Umin = 12,
                Imax = 300
            });

			Drivers.Add(new Driver()
				{
					DriverType = DriverType.RSR2_AM_4,
					Mult = 4,
					R = 0.8,
					I = 0.35,
					U = 0,
					DeviceType = DeviceType.Consumer,
					Umin = 12,
					Imax = 300
				});

			Drivers.Add(new Driver()
				{
					DriverType = DriverType.RSR2_MAP4,
					Mult = 4,
					R = 0.8,
					I = 0.35,
					U = 0,
					DeviceType = DeviceType.Consumer,
					Umin = 10,
					Imax = 300
				});

			Drivers.Add(new Driver()
				{
					DriverType = DriverType.RSR2_RM_2,
					Mult = 2,
					R = 0.8,
					I = 0.21,
					U = 0,
					DeviceType = DeviceType.Consumer,
					Umin = 12,
					Imax = 300
				});

			Drivers.Add(new Driver()
				{
					DriverType = DriverType.RSR2_RM_4,
					Mult = 4,
					R = 0.8,
					I = 0.21,
					U = 0,
					DeviceType = DeviceType.Consumer,
					Umin = 12,
					Imax = 300
				});

			Drivers.Add(new Driver()
				{
					DriverType = DriverType.RSR2_MVK8,
					Mult = 8,
					R = 0.8,
					I = 0.21,
					U = 0,
					DeviceType = DeviceType.Consumer,
					Umin = 10,
					Imax = 300
				});

			Drivers.Add(new Driver()
				{
					DriverType = DriverType.RSR2_MP,
					Mult = 1,
					R = 0.8,
					I = 0.21,
					U = 24,
					DeviceType = DeviceType.Supplier,
					Umin = 12,
					Imax = 300
				});

			Drivers.Add(new Driver()
				{
					DriverType = DriverType.RSR2_MVP,
					Mult = 1,
					R = 0.8,
					I = 0.21,
					U = 24,
					DeviceType = DeviceType.Supplier,
					Umin = 12,
					Imax = 300
				});

			Drivers.Add(new Driver()
				{
					DriverType = DriverType.RSR2_OPS,
					Mult = 1,
					R = 0.8,
					I = 5,
					U = 0,
					DeviceType = DeviceType.Consumer,
					Umin = 12,
					Imax = 300
				});

			Drivers.Add(new Driver()
				{
					DriverType = DriverType.RSR2_OPZ,
					Mult = 1,
					R = 0.8,
					I = 35,
					U = 0,
					DeviceType = DeviceType.Consumer,
					Umin = 15,
					Imax = 300
				});

			Drivers.Add(new Driver()
				{
					DriverType = DriverType.RSR2_OPK,
					Mult = 1,
					R = 0.8,
					I = 40,
					U = 0,
					DeviceType = DeviceType.Consumer,
					Umin = 15, 
					Imax = 300
				});

            Drivers.Add(new Driver()
            {
                DriverType = DriverType.RSR2_Cardreader,
                Mult = 1,
                R = 0.8,
                I = 1,
                U = 0,
                DeviceType = DeviceType.Consumer,
                Umin = 12,
                Imax = 300
            });

            Drivers.Add(new Driver()
            {
				DriverType = DriverType.RSR2_Bush_Shuv,
                Mult = 1,
                R = 0.8,
                I = 0.3,
                U = 0,
                DeviceType = DeviceType.Consumer,
                Umin = 15,
                Imax = 300
            });

            Drivers.Add(new Driver()
            {
				DriverType = DriverType.RSR2_Valve_DU,
                Mult = 1,
                R = 0.8,
                I = 0.3,
                U = 0,
                DeviceType = DeviceType.Consumer,
                Umin = 15,
                Imax = 300
            });

            Drivers.Add(new Driver()
            {
                DriverType = DriverType.RSR2_GuardDetector,
                Mult = 1,
                R = 0.8,
                I = 0.5,
                U = 0,
                DeviceType = DeviceType.Consumer,
                Umin = 15,
                Imax = 300
            });

            Drivers.Add(new Driver()
            {
                DriverType = DriverType.RSR2_GuardDetectorSound,
                Mult = 1,
                R = 0.8,
                I = 0.5,
                U = 0,
                DeviceType = DeviceType.Consumer,
                Umin = 15,
                Imax = 300
            });
		}
	}
}