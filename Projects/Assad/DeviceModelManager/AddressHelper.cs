using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeviveModelManager
{
    public static class AddressHelper
    {
        static List<DeviceAddress> DeviceAddresses;

        static AddressHelper()
        {
            DeviceAddresses = new List<DeviceAddress>();
            DeviceAddresses.Add(new DeviceAddress() { Name = "Страница", Min = 1, Max = 5 });
            DeviceAddresses.Add(new DeviceAddress() { Name = "Индикатор", Min = 1, Max = 50 });
            DeviceAddresses.Add(new DeviceAddress() { Name = "Прибор Рубеж-2AM", Min = 1, Max = 32 });
            DeviceAddresses.Add(new DeviceAddress() { Name = "БУНС", Min = 1, Max = 32 });
            DeviceAddresses.Add(new DeviceAddress() { Name = "Модуль сопряжения МС-3", Min = 124, Max = 125 });
            DeviceAddresses.Add(new DeviceAddress() { Name = "Модуль сопряжения МС-4", Min = 124, Max = 125 });
            DeviceAddresses.Add(new DeviceAddress() { Name = "Блок индикации", Min = 1, Max = 32 });
            DeviceAddresses.Add(new DeviceAddress() { Name = "Прибор Рубеж-10AM", Min = 1, Max = 32 });
            DeviceAddresses.Add(new DeviceAddress() { Name = "Прибор Рубеж-4A", Min = 1, Max = 32 });
            DeviceAddresses.Add(new DeviceAddress() { Name = "Релейный исполнительный модуль РМ-1", Min = 1, Max = 255 });
            DeviceAddresses.Add(new DeviceAddress() { Name = "Модуль пожаротушения", Min = 1, Max = 255 });
            DeviceAddresses.Add(new DeviceAddress() { Name = "Пожарный дымовой извещатель ИП 212-64", Min = 1, Max = 255 });
            DeviceAddresses.Add(new DeviceAddress() { Name = "Пожарный тепловой извещатель ИП 101-29-A3R1", Min = 1, Max = 255 });
            DeviceAddresses.Add(new DeviceAddress() { Name = "Пожарный комбинированный извещатель ИП212/101-64-А2R1", Min = 1, Max = 255 });
            DeviceAddresses.Add(new DeviceAddress() { Name = "Пожарная адресная метка АМ1", Min = 1, Max = 255 });
            DeviceAddresses.Add(new DeviceAddress() { Name = "Кнопка останова СПТ", Min = 1, Max = 255 });
            DeviceAddresses.Add(new DeviceAddress() { Name = "Кнопка запуска СПТ", Min = 1, Max = 255 });
            DeviceAddresses.Add(new DeviceAddress() { Name = "Кнопка управления автоматикой", Min = 1, Max = 255 });
            DeviceAddresses.Add(new DeviceAddress() { Name = "Ручной извещатель ИПР513-11", Min = 1, Max = 255 });
            DeviceAddresses.Add(new DeviceAddress() { Name = "Модуль Управления Клапанами Дымоудаления", Min = 1, Max = 255 });
            DeviceAddresses.Add(new DeviceAddress() { Name = "Модуль Управления Клапанами Огнезащиты", Min = 1, Max = 255 });
            DeviceAddresses.Add(new DeviceAddress() { Name = "Насосная Станция", Min = 0, Max = 0 });
            DeviceAddresses.Add(new DeviceAddress() { Name = "Насос", Min = 1, Max = 10 });
            DeviceAddresses.Add(new DeviceAddress() { Name = "Жокей-насос", Min = 0, Max = 0 });
            DeviceAddresses.Add(new DeviceAddress() { Name = "Компрессор", Min = 0, Max = 0 });
            DeviceAddresses.Add(new DeviceAddress() { Name = "Дренажный насос", Min = 0, Max = 0 });
            DeviceAddresses.Add(new DeviceAddress() { Name = "Насос компенсации утечек", Min = 0, Max = 0 });
            DeviceAddresses.Add(new DeviceAddress() { Name = "Пожарная адресная метка АМП-4", Min = 1, Max = 255 });
            DeviceAddresses.Add(new DeviceAddress() { Name = "Модуль речевого оповещения", Min = 1, Max = 255 });
            DeviceAddresses.Add(new DeviceAddress() { Name = "Задвижка", Min = 1, Max = 255 });
            DeviceAddresses.Add(new DeviceAddress() { Name = "Технологическая адресная метка АМ1-Т", Min = 1, Max = 255 });
            DeviceAddresses.Add(new DeviceAddress() { Name = "АСПТ", Min = 1, Max = 10 });
            DeviceAddresses.Add(new DeviceAddress() { Name = "Модуль дымоудаления-1.02/3", Min = 1, Max = 255 });
            DeviceAddresses.Add(new DeviceAddress() { Name = "USB преобразователь МС-2", Min = 0, Max = 0 });
            DeviceAddresses.Add(new DeviceAddress() { Name = "USB преобразователь МС-1", Min = 0, Max = 0 });
            DeviceAddresses.Add(new DeviceAddress() { Name = "USB Канал", Min = 1, Max = 2 });
            DeviceAddresses.Add(new DeviceAddress() { Name = "USB Канал", Min = 1, Max = 2 });
            DeviceAddresses.Add(new DeviceAddress() { Name = "Канал с резервированием", Min = 0, Max = 0 });
            DeviceAddresses.Add(new DeviceAddress() { Name = "Состав", Min = 0, Max = 0 });
            DeviceAddresses.Add(new DeviceAddress() { Name = "USB Канал", Min = 1, Max = 2 });
            DeviceAddresses.Add(new DeviceAddress() { Name = "Модуль доставки сообщений", Min = 1, Max = 255 });
        }

        public static int GetMinAddress(string DeviceName)
        {
            return DeviceAddresses.FirstOrDefault(x => x.Name == DeviceName).Min;
        }

        public static int GetMaxAddress(string DeviceName)
        {

            return DeviceAddresses.FirstOrDefault(x => x.Name == DeviceName).Max;
        }
    }

    class DeviceAddress
    {
        public string Name { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }
    }
}
