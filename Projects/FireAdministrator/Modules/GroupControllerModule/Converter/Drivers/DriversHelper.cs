using System;
using System.Collections.Generic;
using FiresecAPI.Models;
using GKModule.Models;
using XFiresecAPI;

namespace GKModule.Converter
{
    public static class DriversHelper
    {
        static DriversHelper()
        {
            Drivers = new List<DriverItem>();
            Drivers.Add(new DriverItem(XDriverType.RM_1, DriverType.RM_1));
            Drivers.Add(new DriverItem(XDriverType.RM_2, DriverType.RM_2));
            Drivers.Add(new DriverItem(XDriverType.RM_3, DriverType.RM_3));
            Drivers.Add(new DriverItem(XDriverType.RM_4, DriverType.RM_4));
            Drivers.Add(new DriverItem(XDriverType.RM_5, DriverType.RM_5));
            Drivers.Add(new DriverItem(XDriverType.MPT, DriverType.MPT));
            Drivers.Add(new DriverItem(XDriverType.SmokeDetector, DriverType.SmokeDetector));
            Drivers.Add(new DriverItem(XDriverType.HeatDetector, DriverType.HeatDetector));
            Drivers.Add(new DriverItem(XDriverType.CombinedDetector, DriverType.CombinedDetector));
            Drivers.Add(new DriverItem(XDriverType.AM_1, DriverType.AM_1));
            Drivers.Add(new DriverItem(XDriverType.AM1_O, DriverType.AM1_O));
            Drivers.Add(new DriverItem(XDriverType.AMP_4, DriverType.AMP_4));
            Drivers.Add(new DriverItem(XDriverType.AM4, DriverType.AM4));
            Drivers.Add(new DriverItem(XDriverType.AM4_T, DriverType.AM4_T));
            Drivers.Add(new DriverItem(XDriverType.AM4_P, DriverType.AM4_P));
            Drivers.Add(new DriverItem(XDriverType.AM4_O, DriverType.AM4_O));
            Drivers.Add(new DriverItem(XDriverType.StopButton, DriverType.StopButton));
            Drivers.Add(new DriverItem(XDriverType.StartButton, DriverType.StartButton));
            Drivers.Add(new DriverItem(XDriverType.AutomaticButton, DriverType.AutomaticButton));
            Drivers.Add(new DriverItem(XDriverType.ShuzOnButton, DriverType.ShuzOnButton));
            Drivers.Add(new DriverItem(XDriverType.ShuzOffButton, DriverType.ShuzOffButton));
            Drivers.Add(new DriverItem(XDriverType.ShuzUnblockButton, DriverType.ShuzUnblockButton));
            Drivers.Add(new DriverItem(XDriverType.HandDetector, DriverType.HandDetector));
            Drivers.Add(new DriverItem(XDriverType.Pump, DriverType.Pump));
            Drivers.Add(new DriverItem(XDriverType.JokeyPump, DriverType.JokeyPump));
            Drivers.Add(new DriverItem(XDriverType.Compressor, DriverType.Compressor));
            Drivers.Add(new DriverItem(XDriverType.DrenazhPump, DriverType.DrenazhPump));
            Drivers.Add(new DriverItem(XDriverType.CompensationPump, DriverType.CompensationPump));
            Drivers.Add(new DriverItem(XDriverType.MRO, DriverType.MRO));
            Drivers.Add(new DriverItem(XDriverType.Valve, DriverType.Valve));
            Drivers.Add(new DriverItem(XDriverType.AM1_T, DriverType.AM1_T));
            Drivers.Add(new DriverItem(XDriverType.AMT_4, DriverType.AMT_4));
            Drivers.Add(new DriverItem(XDriverType.MDU, DriverType.MDU));
            Drivers.Add(new DriverItem(XDriverType.MRK_30, DriverType.MRK_30));
            Drivers.Add(new DriverItem(XDriverType.RadioHandDetector, DriverType.RadioHandDetector, false));
            Drivers.Add(new DriverItem(XDriverType.RadioSmokeDetector, DriverType.RadioSmokeDetector, false));
        }

        public static List<DriverItem>  Drivers { get; private set; }

        public static Guid System_UID = new Guid("938947C5-4624-4A1A-939C-60AEEBF7B65C");
        public static Guid GK_UID = new Guid("C052395D-043F-4590-A0B8-BC49867ADC6A");
        public static Guid KAU_UID = new Guid("4993E06C-85D1-4F20-9887-4C5F67C450E8");
        public static Guid KAUIndicator_UID = new Guid("17A2B7D1-CB62-4AF7-940E-BC30B004B0D0");
		public static Guid GKIndicator_UID = new Guid("200EED4B-3402-45B4-8122-AE51A4841E18");
        public static Guid KAUExit_UID = new Guid("FD3E3C36-B036-470F-BAC8-6D49007FBFD3");
    }

    public class DriverItem
    {
        public DriverItem(XDriverType xDriverType, DriverType oldDriverType, bool connectToAddressController = true)
        {
            XDriverType = xDriverType;
            OldDriverType = oldDriverType;
            ConnectToAddressController = connectToAddressController;
        }

        public XDriverType XDriverType { get; set; }
        public DriverType OldDriverType { get; set; }
        public bool ConnectToAddressController { get; set; }
    }
}