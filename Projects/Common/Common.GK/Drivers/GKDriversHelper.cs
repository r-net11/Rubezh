using System;
using System.Collections.Generic;
using FiresecAPI.Models;
using XFiresecAPI;

namespace Common.GK
{
	public static class GKDriversHelper
	{
		static GKDriversHelper()
		{
			Drivers = new List<DriverItem>();

			//Drivers.Add(new DriverItem(XDriverType.MRK_30, DriverType.MRK_30, 0x00));

			Drivers.Add(new DriverItem(XDriverType.AMP_4, DriverType.AMP_4, 0x50));

			Drivers.Add(new DriverItem(XDriverType.AM_1, DriverType.AM_1, 0x51));
			//Drivers.Add(new DriverItem(XDriverType.AM1_O, DriverType.AM1_O, 0x51));
			//Drivers.Add(new DriverItem(XDriverType.AM4, DriverType.AM4, 0x51));
			//Drivers.Add(new DriverItem(XDriverType.AM4_T, DriverType.AM4_T, 0x51));
			//Drivers.Add(new DriverItem(XDriverType.AM4_P, DriverType.AM4_P, 0x51));
			//Drivers.Add(new DriverItem(XDriverType.AM4_O, DriverType.AM4_O, 0x51));
			//Drivers.Add(new DriverItem(XDriverType.StopButton, DriverType.StopButton, 0x51));
			//Drivers.Add(new DriverItem(XDriverType.StartButton, DriverType.StartButton, 0x51));
			//Drivers.Add(new DriverItem(XDriverType.AutomaticButton, DriverType.AutomaticButton, 0x51));
			//Drivers.Add(new DriverItem(XDriverType.ShuzOnButton, DriverType.ShuzOnButton, 0x51));
			//Drivers.Add(new DriverItem(XDriverType.ShuzOffButton, DriverType.ShuzOffButton, 0x51));
			//Drivers.Add(new DriverItem(XDriverType.ShuzUnblockButton, DriverType.ShuzUnblockButton, 0x51));
			//Drivers.Add(new DriverItem(XDriverType.AM1_T, DriverType.AM1_T, 0x51));
			//Drivers.Add(new DriverItem(XDriverType.AMT_4, DriverType.AMT_4, 0x51));

			Drivers.Add(new DriverItem(XDriverType.HandDetector, DriverType.HandDetector, 0x55));
			//Drivers.Add(new DriverItem(XDriverType.RadioHandDetector, DriverType.RadioHandDetector, 0x55, false));

			Drivers.Add(new DriverItem(XDriverType.CombinedDetector, DriverType.CombinedDetector, 0x60));

			Drivers.Add(new DriverItem(XDriverType.SmokeDetector, DriverType.SmokeDetector, 0x61));
			//Drivers.Add(new DriverItem(XDriverType.RadioSmokeDetector, DriverType.RadioSmokeDetector, 0x61, false));

			Drivers.Add(new DriverItem(XDriverType.HeatDetector, DriverType.HeatDetector, 0x62));

			Drivers.Add(new DriverItem(XDriverType.Pump, DriverType.Pump, 0x70));
			//Drivers.Add(new DriverItem(XDriverType.JokeyPump, DriverType.JokeyPump, 0x70));
			//Drivers.Add(new DriverItem(XDriverType.Compressor, DriverType.Compressor, 0x70));
			//Drivers.Add(new DriverItem(XDriverType.DrenazhPump, DriverType.DrenazhPump, 0x70));
			//Drivers.Add(new DriverItem(XDriverType.CompensationPump, DriverType.CompensationPump, 0x70));

			Drivers.Add(new DriverItem(XDriverType.Valve, DriverType.Valve, 0x71));

			Drivers.Add(new DriverItem(XDriverType.MRO, DriverType.MRO, 0x74));

			Drivers.Add(new DriverItem(XDriverType.RM_1, DriverType.RM_1, 0x75));
			Drivers.Add(new DriverItem(XDriverType.RM_2, DriverType.RM_2, 0x75));
			Drivers.Add(new DriverItem(XDriverType.RM_3, DriverType.RM_3, 0x75));
			Drivers.Add(new DriverItem(XDriverType.RM_4, DriverType.RM_4, 0x75));
			Drivers.Add(new DriverItem(XDriverType.RM_5, DriverType.RM_5, 0x75));

			Drivers.Add(new DriverItem(XDriverType.MPT, DriverType.MPT, 0x76));

			Drivers.Add(new DriverItem(XDriverType.MDU, DriverType.MDU, 0x7E));
		}

		public static List<DriverItem> Drivers { get; private set; }

		public static Guid System_UID = new Guid("938947C5-4624-4A1A-939C-60AEEBF7B65C");
		public static Guid GK_UID = new Guid("C052395D-043F-4590-A0B8-BC49867ADC6A");
		public static Guid KAU_UID = new Guid("4993E06C-85D1-4F20-9887-4C5F67C450E8");
		public static Guid KAUIndicator_UID = new Guid("17A2B7D1-CB62-4AF7-940E-BC30B004B0D0");
		public static Guid GKIndicator_UID = new Guid("200EED4B-3402-45B4-8122-AE51A4841E18");
		public static Guid GKLine_UID = new Guid("DEAA33C2-0EAA-4D4D-BA31-FCDBE0AD149A");
		public static Guid GKRele_UID = new Guid("1AC85436-61BC-441B-B6BF-C6A0FA62748B");
		public static Guid KAUExit_UID = new Guid("FD3E3C36-B036-470F-BAC8-6D49007FBFD3");
	}

	public class DriverItem
	{
		public DriverItem(XDriverType xDriverType, DriverType oldDriverType, ushort driverTypeNo, bool connectToAddressController = true)
		{
			XDriverType = xDriverType;
			OldDriverType = oldDriverType;
			DriverTypeNo = driverTypeNo;
			ConnectToAddressController = connectToAddressController;
		}

		public XDriverType XDriverType { get; set; }
		public DriverType OldDriverType { get; set; }
		public ushort DriverTypeNo { get; set; }
		public bool ConnectToAddressController { get; set; }
	}
}