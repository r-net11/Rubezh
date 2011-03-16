using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecMetadata;

namespace AssadDevices
{
    public class AssadPump : AssadBase
    {
        public override void SetInnerDevice(Assad.MHconfigTypeDevice innerDevice)
        {
            base.SetInnerDevice(innerDevice);

            string driverName = DriversHelper.GetDriverNameById(DriverId);
            switch (driverName)
            {
                case "Насос":
                    Address = Properties.First(x => x.Name == "Адрес").Value;
                    Properties.Remove(Properties.First(x => x.Name == "Адрес"));
                    break;

                case "Жокей-насос":
                    Address = "Жокей-насос";
                    break;

                case "Компрессор":
                    Address = "Компрессор";
                    break;

                case "Дренажный насос":
                    Address = "Дренажный насос";
                    break;

                case "Насос компенсации утечек":
                    Address = "Насос компенсации утечек";
                    break;
            }
        }
    }
}
