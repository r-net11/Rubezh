using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssadDevices
{
    public static class AssadServices
    {
        static AssadServices()
        {
            AssadDeviceTypesManager = new AssadDeviceTypesManager();
            AssadDeviceManager = new AssadDeviceManager();
        }
        public static AssadDeviceTypesManager AssadDeviceTypesManager { get; private set; }
        public static AssadDeviceManager AssadDeviceManager { get; private set; }
    }
}
