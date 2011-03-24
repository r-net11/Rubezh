using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssadDevices
{
    public class AssadZone : AssadBase
    {
        public string ZoneName { get; set; }
        public string ZoneId { get; set; }
        public string DetectorCount { get; set; }
        public string EvecuationTime { get; set; }

        public override void SetInnerDevice(Assad.MHconfigTypeDevice innerDevice)
        {
            base.SetInnerDevice(innerDevice);

            try
            {
                ZoneName = Properties.First(x => x.Name == "Наименование").Value;
                ZoneId = Properties.First(x => x.Name == "Номер зоны").Value;
                DetectorCount = Properties.First(x => x.Name == "Число датчиков для формирования сигнала Пожар").Value;
                EvecuationTime = Properties.First(x => x.Name == "Время эвакуации").Value;
            }
            catch
            {
                throw new Exception("Неправильный формат зоны при конфигурации из ассада");
            }
        }
    }
}
