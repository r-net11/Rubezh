using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssadProcessor
{
    public static class Services
    {
        static Services()
        {
            LogEngine = new Logger.LogEngine();
            NetManager = new NetManager();
            AssadDeviceTypesManager = new AssadDeviceTypesManager();
            AssadDeviceManager = new AssadDeviceManager();
        }

        public static Logger.LogEngine LogEngine { get; private set; }
        public static NetManager NetManager { get; private set; }
        public static AssadDeviceTypesManager AssadDeviceTypesManager { get; private set; }
        public static AssadDeviceManager AssadDeviceManager { get; private set; }
    }
}
