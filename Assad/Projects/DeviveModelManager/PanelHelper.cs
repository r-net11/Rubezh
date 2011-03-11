using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeviveModelManager
{
    public static class PanelHelper
    {
        static List<PanelDevice> devices;
        //static List<string> addressDevices;

        static PanelHelper()
        {
            devices = new List<PanelDevice>();
            devices.Add(new PanelDevice(@"USB Рубеж-2AM", 2));
            devices.Add(new PanelDevice(@"USB Рубеж-4A", 4));
            devices.Add(new PanelDevice(@"USB БУНС", 2));
            devices.Add(new PanelDevice(@"Прибор Рубеж-2AM", 2));
            devices.Add(new PanelDevice(@"Прибор Рубеж-10AM", 10));
            devices.Add(new PanelDevice(@"Прибор Рубеж-4A", 4));
            devices.Add(new PanelDevice(@"БУНС", 2));
            devices.Add(new PanelDevice(@"Насосная Станция", 0));
            devices.Add(new PanelDevice(@"Прибор Рубеж 2A", 2));
            devices.Add(new PanelDevice(@"Прибор Рубеж 10A", 10));
            devices.Add(new PanelDevice(@"Компьютер", 0));
            devices.Add(new PanelDevice(@"COM порт (V1)", 0));
            devices.Add(new PanelDevice(@"COM порт (V2)", 0));
            devices.Add(new PanelDevice(@"Блок индикации", 0));
            devices.Add(new PanelDevice(@"Страница", 0));
            devices.Add(new PanelDevice(@"USB преобразователь МС-2", 0));
            devices.Add(new PanelDevice(@"USB преобразователь МС-3", 0));
            devices.Add(new PanelDevice(@"USB преобразователь МС-4", 0));
            devices.Add(new PanelDevice(@"USB Канал", 0));
            devices.Add(new PanelDevice(@"USB преобразователь МС-1", 0));
            devices.Add(new PanelDevice(@"USB преобразователь МС-2", 0));

            //addressDevices = new List<string>();
            //addressDevices.Add(@"USB Рубеж-2AM");
            //addressDevices.Add(@"USB Рубеж-4A");
            //addressDevices.Add(@"USB БУНС");
            //addressDevices.Add(@"Прибор Рубеж-2AM");
            //addressDevices.Add(@"Прибор Рубеж-10AM");
            //addressDevices.Add(@"Прибор Рубеж-4A");
            //addressDevices.Add(@"БУНС");
            //addressDevices.Add(@"Насосная Станция");
            //addressDevices.Add(@"Прибор Рубеж 2A");
            //addressDevices.Add(@"Прибор Рубеж 10A");
            //addressDevices.Add(@"Компьютер");
            //addressDevices.Add(@"COM порт (V1)");
            //addressDevices.Add(@"COM порт (V2)");
            //addressDevices.Add(@"Блок индикации");
            //addressDevices.Add(@"Страница");
            //addressDevices.Add(@"USB преобразователь МС-2");
            //addressDevices.Add(@"USB преобразователь МС-3");
            //addressDevices.Add(@"USB преобразователь МС-4");
            //addressDevices.Add(@"USB Канал");
            //addressDevices.Add(@"USB преобразователь МС-1");
            //addressDevices.Add(@"USB преобразователь МС-2");

            //addressDevices.Add(@"Пожарный комбинированный извещатель ИП212/101-64-А2R1");
            //addressDevices.Add(@"Пожарный дымовой извещатель ИП 212-64");
            //addressDevices.Add(@"Пожарный тепловой извещатель ИП 101-29-A3R1");
            //addressDevices.Add(@"Ручной извещатель ИПР514-3");
            //addressDevices.Add(@"Пожарная адресная метка АМ1");
            //addressDevices.Add(@"Метка контроля питания");
            //addressDevices.Add(@"Релейный исполнительный модуль РМ-1");
            //addressDevices.Add(@"АСПТ");
            //addressDevices.Add(@"Релейный исполнительный модуль РМ-1");
            //addressDevices.Add(@"Кнопка останова СПТ");
            //addressDevices.Add(@"Кнопка запуска СПТ");
            //addressDevices.Add(@"Кнопка управления автоматикой");
            //addressDevices.Add(@"Ручной извещатель ИПР513-11");
            //addressDevices.Add(@"АМ-4(О)");
            //addressDevices.Add(@"Модуль Управления Клапанами Дымоудаления");
            //addressDevices.Add(@"Модуль Управления Клапанами Огнезащиты");
            //addressDevices.Add(@"Пожарная адресная метка АМП-4");
            //addressDevices.Add(@"Модуль речевого оповещения");
            //addressDevices.Add(@"Задвижка");
            //addressDevices.Add(@"Технологическая адресная метка АМ1-Т");
            //addressDevices.Add(@"Технологическая адресная метка АМТ-4");
            //addressDevices.Add(@"Модуль дымоудаления-1.02/3");
            //addressDevices.Add(@"Модуль пожаротушения");
        }

        public static int GetShleifCount(string deviceName)
        {
            PanelDevice panelDevice;
            try
            {
                panelDevice = devices.FirstOrDefault(x => x.Name == deviceName);
            }
            catch
            {
                panelDevice = null;
            }
            if (panelDevice != null)
            {
                return panelDevice.ShleifCount;
            }
            return 0;
        }

        public static bool IsPanel(string deviceName)
        {
            return devices.Any(x => x.Name == deviceName);
        }

        //public static bool HasAddress(string deviceName)
        //{
        //    return addressDevices.Contains(deviceName);
        //}
    }

    public class PanelDevice
    {
        public PanelDevice(string Name, int ShleifCount)
        {
            this.Name = Name;
            this.ShleifCount = ShleifCount;
        }

        public string Name { get; set; }
        public int ShleifCount { get; set; }
    }

    //string ignore1 = @"8708BDF2-B389-48A9-A1F8-D5F498162033";
    //string ignore2 = @"C4362050-6287-4E35-AA1A-39B42894AB5C";
    //string ignore3 = @"8708BDF2-B389-48A9-A1F8-D5F498162033";
}
