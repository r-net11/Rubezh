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

            // только если в конфигурации есть более одного МС нужно указывать серийный номер
            if (string.IsNullOrEmpty(serialNo))
            {
                Address = "0";
            }
            else
            {
                Address = serialNo;
            }
        }
    }
}
