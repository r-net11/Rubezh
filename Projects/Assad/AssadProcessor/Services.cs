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
            DeviceModelManager = new DeviceModelManager();
            DeviceManager = new DeviceManager();
        }

        public static Logger.LogEngine LogEngine { get; private set; }
        public static NetManager NetManager { get; private set; }
        public static DeviceModelManager DeviceModelManager { get; private set; }
        public static DeviceManager DeviceManager { get; private set; }
    }
}