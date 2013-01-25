using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerFS2
{
    public static class DriversHelper
    {
        public class DriverData
        {
            public DriverData(string driverId, string name, byte driverType)
            {
                Name = name;
                DriverId = driverId;
                DriverType = driverType;
            }

            public string Name { get; private set; }
            public string DriverId { get; private set; }
            public byte DriverType { get; private set; }
        }

        public static string GetDriverNameByType(byte driverType)
        {
            return DriverDataList.FirstOrDefault(x => x.DriverType == driverType).Name;
        }
        public static List<DriverData> DriverDataList { get; private set; }
        static DriversHelper()
        {
            DriverDataList = new List<DriverData>();
            DriverDataList.Add(new DriverData("B476541B-5298-4B3E-A9BA-605B839B1011", "Прибор Рубеж-2AM", 1));
            DriverDataList.Add(new DriverData("02CE2CC4-D71F-4EAA-ACCC-4F2E870F548C", "БУНС", 2));
            DriverDataList.Add(new DriverData("F966D47B-468D-40A5-ACA7-9BE30D0A3847", "Модуль сопряжения МС-3", 100));
            DriverDataList.Add(new DriverData("F966D47B-468D-40A5-ACA7-9BE30D0A3847", "Модуль сопряжения МС-4", 101));
            DriverDataList.Add(new DriverData("584BC59A-28D5-430B-90BF-592E40E843A6", "Модуль сопряжения МС-ТЛ", 102));
            DriverDataList.Add(new DriverData("28A7487A-BA32-486C-9955-E251AF2E9DD4", "Блок индикации БИ", 3));
            DriverDataList.Add(new DriverData("E750EF8F-54C3-4B00-8C72-C7BEC9E59BFC", "Прибор Рубеж-10AM", 4));
            DriverDataList.Add(new DriverData("F3485243-2F60-493B-8A4E-338C61EF6581", "Рубеж-4А", 5));
            DriverDataList.Add(new DriverData("96CDBD7E-29F6-45D4-9028-CF10332FAB1A", "Прибор РУБЕЖ-2ОП", 6));
            DriverDataList.Add(new DriverData("B1DF571E-8786-4987-94B2-EC91F7578D20", "Пульт дистанционного управления ПДУ", 7));
            DriverDataList.Add(new DriverData("A7BB2FD0-0088-49AE-8C04-7D6FA22C79D6", "БУНС-2", 8));
            DriverDataList.Add(new DriverData("07BEB3DD-7D14-41F8-B8BC-FE8EDD215762", "Пульт дистанционного управления ПДУ-ПТ", 9));
        }
    }
}
