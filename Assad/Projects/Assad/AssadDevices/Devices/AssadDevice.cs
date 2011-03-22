using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecMetadata;

namespace AssadDevices
{
    public class AssadDevice : AssadBase
    {
        public override void SetInnerDevice(Assad.MHconfigTypeDevice innerDevice)
        {
            base.SetInnerDevice(innerDevice);

            string stringAddress = Properties.First(x => x.Name == "Адрес").Value;
            string shleifNo = Properties.First(x => x.Name == "Шлейф").Value;

            Properties.Remove(Properties.First(x => x.Name == "Адрес"));
            Properties.Remove(Properties.First(x => x.Name == "Шлейф"));

            int intAddress;
            try
            {
                intAddress = Convert.ToInt32(stringAddress);
            }
            catch
            {
                SetValidationError("Адрес должен иметь целочисленное значение");
                return;
            }
            int intShleif = Convert.ToInt32(shleifNo);
            intAddress = intShleif * 256 + intAddress;

            Address = intAddress.ToString();

            string driverName = DriversHelper.GetDriverNameById(DriverId);
            switch (driverName)
            {
                case "Модуль пожаротушения":
                case "Пожарный дымовой извещатель ИП 212-64":
                case "Пожарный тепловой извещатель ИП 101-29-A3R1":
                case "Пожарный комбинированный извещатель ИП212//101-64-А2R1":
                case "Пожарная адресная метка АМ1":
                case "Кнопка останова СПТ":
                case "Кнопка запуска СПТ":
                case "Кнопка управления автоматикой":
                case "Ручной извещатель ИПР513-11":
                case "Пожарная адресная метка АМП-4":
                    Zone = Properties.First(x => x.Name == "Зона").Value;
                    break;

                case "Модуль Управления Клапанами Дымоудаления":
                case "Модуль Управления Клапанами Огнезащиты":
                case "Задвижка":
                case "Модуль речевого оповещения":
                case "Модуль дымоудаления-1.02//3":
                    //Zone = Properties.First(x => x.Name == "Настройка включения по состоянию зон").Value;
                    // РАСШИРЕННАЯ ЛОГИКА ЗОН
                    break;

                case "Релейный исполнительный модуль РМ-1":
                case "Технологическая адресная метка АМ1-Т":
                    break;

                default:
                    throw new Exception("Неизвестное устройство");
            }
        }

        public void Validate(string address)
        {
            List<string> addresses = address.Split(new char[] { '.' }, StringSplitOptions.None).ToList();
            if (addresses.Count != 2)
            {
                SetValidationError("Адрес должен быть разделен точкой");
                return;
            }
            int intShleifAddress = 0;
            try
            {
                intShleifAddress = Convert.ToInt32(addresses[0]);
            }
            catch
            {
                SetValidationError("Адрес шлейфа не является целочичленным значением");
                return;
            }
            int intAddress = 0;
            try
            {
                intAddress = Convert.ToInt32(addresses[0]);
            }
            catch
            {
                SetValidationError("Адрес является целочичленным значением");
                return;
            }
            if ((intAddress < 1) && (intAddress > 255))
            {
                SetValidationError("Адрес должен быть в диапазоне 1 - 255");
                return;
            }
            // НУЖНА ПРОВЕРКА ТИПА РОДИТЕЛЬСКОГО УСТРОЙСТВА ЧТОБЫ ПРОВЕРИТЬ ВАЛИДНОСТЬ ДИАПАЗОНА
            // ЭТО НУЖНО ДЕЛАТЬ ПОСЛЕ ПОЛНОГО ЗАПОЛНЕНИЯ КОНФИГУРАЦИИ, Т.К. РОДИТЕЛЬСКОЕ УСТРОЙСТВО МОЖЕТ БЫТЬ ЕЩЕ НЕДОСТУПНО

            if ((intShleifAddress < 1) && (intShleifAddress > 2))
            {
                SetValidationError("Адрес шлейфа быть в диапазоне 1 - 2");
                return;
            }
        }
    }
}
