using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssadDevices
{
    public class AssadMC : AssadBase
    {
        public override void SetInnerDevice(Assad.MHconfigTypeDevice innerDevice)
        {
            base.SetInnerDevice(innerDevice);

            string serialNo = Properties.First(x => x.Name == "Серийный номер").Value;
            Address = serialNo;
        }
    }
}
