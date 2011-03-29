using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceApi;
using FiresecMetadata;

namespace ServiseProcessor
{
    public class Validator
    {
        public static void Validate(StateConfiguration configuration)
        {
            ShortDevice rootShortDevice = configuration.RootShortDevice;
            ValidateChild(rootShortDevice);
        }

        static void ValidateChild(ShortDevice parent)
        {
            List<string> addresses = new List<string>();

            foreach (ShortDevice child in parent.Children)
            {
                string address = child.Address;
                if (addresses.Contains(address))
                    ;// throw new Exception("Адрес дублируется");
                addresses.Add(address);
            }

            foreach (ShortDevice child in parent.Children)
            {
                string error = ValidateDevice(child);
                if (!string.IsNullOrEmpty(error))
                {
                    child.ValidationError = error;
                }
                ValidateChild(child);
            }
        }

        static string ValidateDevice(ShortDevice device)
        {
            int intAddress = 0;
            string driverName = DriversHelper.GetDriverNameById(device.DriverId);
            switch (driverName)
            {
                case "Насосная Станция":
                case "Жокей-насос":
                case "Компрессор":
                case "Дренажный насос":
                case "Насос компенсации утечек":
                    break;
                default:
                    try
                    {
                        intAddress = System.Convert.ToInt32(device.Address);
                    }
                    catch
                    {
                        return "Адрес устройства - не число";
                    }
                    if (intAddress <= 0)
                        return "Адрес не может быть отрицательным";
                    break;
            }

            if (driverName == "Компьютер")
                return "";

            if (string.IsNullOrEmpty(device.Address))
                return "Пустой адрес устройства";

            // проверка спецефичных для устройства параметров
            switch (driverName)
            {
                case "Модуль сопряжения МС-3":
                case "Модуль сопряжения МС-4":
                    if ((device.Address != "124") && (device.Address != "125"))
                        throw new Exception("Устройство должно иметь адрес 124 или 125");
                    break;
                case "Прибор Рубеж-2AM":
                case "Прибор Рубеж-4A":
                case "Прибор Рубеж-10AM":
                case "Блок индикации":
                    if ((intAddress < 1) || (intAddress > 100))
                        return "Устройство должно иметь адрес в диапазоне от 1 до 100";
                    break;
                case "Страница":
                    if ((intAddress < 1) || (intAddress > 5))
                        return "Устройство должно иметь адрес в диапазоне от 1 до 5";
                    break;
                case "Индикатор":
                    if ((intAddress < 1) || (intAddress > 50))
                        return "Устройство должно иметь адрес в диапазоне от 1 до 50";
                    break;
                case "USB преобразователь МС-1":
                case "USB преобразователь МС-2":
                    break;
                case "USB Канал":
                    if (intAddress != 1)
                        return "Устройство должно иметь адрес 1";
                    break;
                case "USB Канал МС-1":
                    if (intAddress != 1)
                        return "Устройство должно иметь адрес 1";
                    break;
                case "USB Канал МС-2":
                    if ((intAddress < 1) || (intAddress > 2))
                        return "Устройство должно иметь адрес в диапазоне от 1 до 2";
                    break;
                default:
                    break;
            }


            // наличие зоны у устройства

            Firesec.Metadata.drvType driver = GetDriverByDriverId(device.DriverId);
            if ((driver.minZoneCardinality == "1") && (driver.maxZoneCardinality == "1"))
            {
                if (device.ZoneNo == null)
                    return "Устройство должно принадлежать к зоне";
            }

            // ПОИСК ЗОНЫ В СПИСКЕ ЗОН

            return "";
        }

        public static Firesec.Metadata.drvType GetDriverByDriverId(string driverId)
        {
            return Services.Configuration.Metadata.drv.FirstOrDefault(x => x.id == driverId);
        }
    }
}
