using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssadDevices
{
    public class AssadChannel : AssadBase
    {
        public override void SetInnerDevice(Assad.MHconfigTypeDevice innerDevice)
        {
            base.SetInnerDevice(innerDevice);

            Address = Properties.FirstOrDefault(x => x.Name == "Адрес").Value;
            Properties.Remove(Properties.First(x => x.Name == "Адрес"));
            Properties.FirstOrDefault(x => x.Name == "Адрес USB устройства в сети RS-485").Name = "Адрес";
        }
    }
}
