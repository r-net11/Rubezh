using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GroupControllerModule.Models;
using FiresecAPI.Models;

namespace GroupControllerModule.Converter
{
    public static class DriversHelper
    {
        static DriversHelper()
        {
            Drivers = new List<DriverItem>();
            Drivers.Add(new DriverItem(GCDriverType.RM_1, DriverType.RM_1));
            Drivers.Add(new DriverItem(GCDriverType.RM_2, DriverType.RM_2));
            Drivers.Add(new DriverItem(GCDriverType.RM_3, DriverType.RM_3));
            Drivers.Add(new DriverItem(GCDriverType.RM_4, DriverType.RM_4));
            Drivers.Add(new DriverItem(GCDriverType.RM_5, DriverType.RM_5));
            Drivers.Add(new DriverItem(GCDriverType.MPT, DriverType.MPT));
            Drivers.Add(new DriverItem(GCDriverType.SmokeDetector, DriverType.SmokeDetector));
            Drivers.Add(new DriverItem(GCDriverType.HeatDetector, DriverType.HeatDetector));
            Drivers.Add(new DriverItem(GCDriverType.CombinedDetector, DriverType.CombinedDetector));
            Drivers.Add(new DriverItem(GCDriverType.AM_1, DriverType.AM_1));
            Drivers.Add(new DriverItem(GCDriverType.AM1_O, DriverType.AM1_O));
            Drivers.Add(new DriverItem(GCDriverType.AMP_4, DriverType.AMP_4));
            Drivers.Add(new DriverItem(GCDriverType.AM4, DriverType.AM4));
            Drivers.Add(new DriverItem(GCDriverType.AM4_T, DriverType.AM4_T));
            Drivers.Add(new DriverItem(GCDriverType.AM4_P, DriverType.AM4_P));
            Drivers.Add(new DriverItem(GCDriverType.AM4_O, DriverType.AM4_O));
            Drivers.Add(new DriverItem(GCDriverType.StopButton, DriverType.StopButton));
            Drivers.Add(new DriverItem(GCDriverType.StartButton, DriverType.StartButton));
            Drivers.Add(new DriverItem(GCDriverType.AutomaticButton, DriverType.AutomaticButton));
            Drivers.Add(new DriverItem(GCDriverType.ShuzOnButton, DriverType.ShuzOnButton));
            Drivers.Add(new DriverItem(GCDriverType.ShuzOffButton, DriverType.ShuzOffButton));
            Drivers.Add(new DriverItem(GCDriverType.ShuzUnblockButton, DriverType.ShuzUnblockButton));
            Drivers.Add(new DriverItem(GCDriverType.HandDetector, DriverType.HandDetector));
            Drivers.Add(new DriverItem(GCDriverType.Pump, DriverType.Pump));
            Drivers.Add(new DriverItem(GCDriverType.JokeyPump, DriverType.JokeyPump));
            Drivers.Add(new DriverItem(GCDriverType.Compressor, DriverType.Compressor));
            Drivers.Add(new DriverItem(GCDriverType.DrenazhPump, DriverType.DrenazhPump));
            Drivers.Add(new DriverItem(GCDriverType.CompensationPump, DriverType.CompensationPump));
            Drivers.Add(new DriverItem(GCDriverType.MRO, DriverType.MRO));
            Drivers.Add(new DriverItem(GCDriverType.Valve, DriverType.Valve));
            Drivers.Add(new DriverItem(GCDriverType.AM1_T, DriverType.AM1_T));
            Drivers.Add(new DriverItem(GCDriverType.AMT_4, DriverType.AMT_4));
            Drivers.Add(new DriverItem(GCDriverType.MDU, DriverType.MDU));
            Drivers.Add(new DriverItem(GCDriverType.MRK_30, DriverType.MRK_30));
            Drivers.Add(new DriverItem(GCDriverType.RadioHandDetector, DriverType.RadioHandDetector, false));
            Drivers.Add(new DriverItem(GCDriverType.RadioSmokeDetector, DriverType.RadioSmokeDetector, false));
        }

        public static List<DriverItem> Drivers { get; private set; }

        public static Guid AddressControllerUID = Guid.NewGuid();
        public static Guid GroupControllerUID = Guid.NewGuid();
    }

    public class DriverItem
    {
        public DriverItem(GCDriverType gCDriverType, DriverType driverType, bool connectToAddressController = true)
        {
            GCDriverType = gCDriverType;
            OldDriverType = driverType;
            ConnectToAddressController = connectToAddressController;
        }

        public GCDriverType GCDriverType { get; set; }
        public DriverType OldDriverType { get; set; }
        public Guid UID { get; set; }
        public bool ConnectToAddressController { get; set; }
    }
}