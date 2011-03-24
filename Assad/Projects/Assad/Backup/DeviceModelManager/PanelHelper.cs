using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeviveModelManager
{
    public static class PanelHelper
    {
        static List<PanelDevice> devices;

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
}
