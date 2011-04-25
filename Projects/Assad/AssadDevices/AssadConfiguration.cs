using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssadDevices
{
    // в этом классе хранится точная копия устройств и зон, полученных из ассада

    public static class AssadConfiguration
    {
        public static List<AssadBase> Devices { get; set; }
        public static bool IsValid { get; set; }
        public static List<AssadBase> InvalidDevices { get; set; }
    }
}
