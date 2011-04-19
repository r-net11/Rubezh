using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecMetadata;

namespace AssadDevices
{
    public static class AssadDeviceFactory
    {
        public static AssadBase Create(Assad.MHconfigTypeDevice innerDevice)
        {
            AssadBase assadBase;
            string driverId = GetDriverId(innerDevice);
            string driverName = DriversHelper.GetDriverNameById(driverId);
            switch (driverName)
            {
                case "Компьютер":
                    assadBase = new AssadComputer();
                    break;
                case "zone":
                    assadBase = new AssadZone();
                    break;
                case "monitor":

                case "USB преобразователь МС-1":
                case "USB преобразователь МС-2":
                    assadBase = new AssadMC();
                    break;

                case "Модуль сопряжения МС-3":
                case "Модуль сопряжения МС-4":
                    assadBase = new AssadMC34();
                    break;

                case "Страница":
                    assadBase = new AssadPage();
                    break;

                case "Индикатор":
                    assadBase = new AssadIndicator();
                    break;

                case "USB Канал МС-1":
                case "USB Канал МС-2":
                case "USB Канал":
                    assadBase = new AssadChannel();
                    break;

                case "Релейный исполнительный модуль РМ-1":
                case "Модуль пожаротушения":
                case "Пожарный дымовой извещатель ИП 212-64":
                case "Пожарный тепловой извещатель ИП 101-29-A3R1":
                case "Пожарный комбинированный извещатель ИП212//101-64-А2R1":
                case "Пожарная адресная метка АМ1":
                case "Кнопка останова СПТ":
                case "Кнопка запуска СПТ":
                case "Кнопка управления автоматикой":
                case "Ручной извещатель ИПР513-11":
                case "Модуль Управления Клапанами Дымоудаления":
                case "Модуль Управления Клапанами Огнезащиты":
                case "Пожарная адресная метка АМП-4":
                case "Модуль речевого оповещения":
                case "Задвижка":
                case "Технологическая адресная метка АМ1-Т":
                case "Модуль дымоудаления-1.02//3":
                    assadBase = new AssadDevice();
                    break;

                case "Прибор Рубеж-2AM":
                case "Прибор Рубеж-4A":
                case "Прибор Рубеж-10AM":
                case "БУНС":
                case "Блок индикации":
                    assadBase = new AssadPanel();
                    break;

                case "АСПТ":
                    assadBase = new AssadASPT();
                    break;

                case "Насосная Станция":
                    assadBase = new AssadPumpStation();
                    break;

                case "Насос":
                case "Жокей-насос":
                case "Компрессор":
                case "Дренажный насос":
                case "Насос компенсации утечек":
                    assadBase = new AssadPump();
                    break;

                default:
                    throw new Exception("Фабрика устройств не может распознать устройство");
            }
            return assadBase;
        }

        // свойство innerDevice.type содержит закодированный тип устройства
        // метод возвращает идентификатор драйвкра устройства или строку zone

        public static string GetDriverId(Assad.MHconfigTypeDevice innerDevice)
        {
            string[] separators = new string[1];
            separators[0] = ".";
            string[] separatedTypes = innerDevice.type.Split(separators, StringSplitOptions.None);
            string driverId = separatedTypes[2];
            return driverId;
        }
    }
}
