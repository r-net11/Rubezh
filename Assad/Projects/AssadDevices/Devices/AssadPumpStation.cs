using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssadDevices
{
    public class AssadPumpStation : AssadBase
    {
        public override void SetInnerDevice(Assad.MHconfigTypeDevice innerDevice)
        {
            base.SetInnerDevice(innerDevice);

            Address = "Насосная Станция";

            Zones = ExtractZones(Properties.First(x => x.Name == "Зоны").Value);
        }
    }
}
