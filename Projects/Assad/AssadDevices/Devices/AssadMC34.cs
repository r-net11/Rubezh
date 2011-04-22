using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssadDevices
{
    public class AssadMC34 : AssadBase
    {
        public override void SetInnerDevice(Assad.MHconfigTypeDevice innerDevice)
        {
            base.SetInnerDevice(innerDevice);

            Address = Properties.First(x => x.Name == "Адрес").Value;
            Properties.Remove(Properties.First(x => x.Name == "Адрес"));
        }
    }
}
