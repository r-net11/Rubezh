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
        public void Validate(CurrentConfiguration currentConfiguration)
        {
            this.configuration = currentConfiguration;
            configuration.FillAllDevices();
            ClearValidationErrors();
            ValidateZones();
            ValidateDeviceZones();
            ValidateAddresses();

            //Device rootDevice = currentConfiguration.RootDevice;
            //ValidateChild(rootDevice);
        }

        CurrentConfiguration configuration;

        void ClearValidationErrors()
        {
            foreach (Device device in configuration.AllDevices)
            {
                device.ValidationErrors = new List<ValidationError>();
            }
            foreach (Zone zone in configuration.Zones)
            {
                zone.ValidationErrors = new List<ValidationError>();
            }
        }

        void ValidateZones()
        {
            List<string> uniqueZoneIds = new List<string>();
            foreach (Zone zone in configuration.Zones)
            {
                if (uniqueZoneIds.Contains(zone.No))
                {
                    zone.ValidationErrors.Add(new ValidationError("Адрес зоны повторяется", Level.Critical));
                }
                else
                {
                    uniqueZoneIds.Add(zone.No);
                }

                int intDetectorCount = 0;
                try
                {
                    intDetectorCount = System.Convert.ToInt32(zone.DetectorCount);
                }
                catch
                {
                    zone.ValidationErrors.Add(new ValidationError("Параметр количество устройств не является числом", Level.Critical));
                }
                if (intDetectorCount < 1)
                {
                    zone.ValidationErrors.Add(new ValidationError("Параметр количество устройств должно быть в диапазоне 1-255", Level.Normal));
                    zone.DetectorCount = "1";
                }
                if (intDetectorCount < 255)
                {
                    zone.ValidationErrors.Add(new ValidationError("Параметр количество устройств должно быть в диапазоне 1-255", Level.Normal));
                    zone.DetectorCount = "255";
                }

                int intEvacuationTime = 0;
                try
                {
                    intEvacuationTime = System.Convert.ToInt32(zone.EvacuationTime);
                }
                catch
                {
                    zone.ValidationErrors.Add(new ValidationError("Параметр время эвакуации не является числом", Level.Critical));
                }
                if (intEvacuationTime < 0)
                {
                    zone.ValidationErrors.Add(new ValidationError("Параметр время эвакуации должн быть в диапазоне 0-32000", Level.Normal));
                    //zone.EvacuationTime = "0";
                }
                if (intEvacuationTime > 32000)
                {
                    zone.ValidationErrors.Add(new ValidationError("Параметр время эвакуации должн быть в диапазоне 0-32000", Level.Normal));
                    //zone.EvacuationTime = "32000";
                }

                if (configuration.AllDevices.Any(x => x.ZoneNo == zone.No) == false)
                {
                    zone.ValidationErrors.Add(new ValidationError("В зоне отсутствуют устройства", Level.Normal));
                }
            }
        }

        void ValidateDeviceZones()
        {
            foreach (Device device in configuration.AllDevices)
            {
                Firesec.Metadata.drvType driver = GetDriverByDriverId(device.DriverId);
                if ((driver.minZoneCardinality == "1") && (driver.maxZoneCardinality == "1"))
                {
                    if (device.ZoneLogic != null)
                        device.ValidationErrors.Add(new ValidationError("Устройство не может иметь логику срабатывания", Level.Critical));

                    if (string.IsNullOrEmpty(device.ZoneNo))
                        device.ValidationErrors.Add(new ValidationError("Устройство должно иметь зону", Level.Normal));
                    else
                    {
                        if (configuration.Zones.Any(x => x.No == device.ZoneNo) == false)
                        {
                            device.ValidationErrors.Add(new ValidationError("Устройство принадлежит к несуществующей зоне", Level.Critical));
                            //device.ZoneNo = null;
                        }
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(device.ZoneNo))
                        device.ValidationErrors.Add(new ValidationError("Устройство не может принадлежать к зоне", Level.Critical));

                    if (driver.options.Contains("ExtendedZoneLogic"))
                    {
                        foreach (Firesec.ZoneLogic.clauseType clause in device.ZoneLogic.clause)
                        {
                            if ((clause.state != "0") && (clause.state != "1") && (clause.state != "2") && (clause.state != "5") && (clause.state != "6"))
                                device.ValidationErrors.Add(new ValidationError("Логика зоны имеет неизвестный тип состояния", Level.Critical));

                            if ((clause.operation != "and") && (clause.operation != "or"))
                                device.ValidationErrors.Add(new ValidationError("Логика зоны имеет неизвестный тип операции", Level.Critical));

                            foreach (string zonNo in clause.zone)
                            {
                                if (configuration.Zones.Any(x=>x.No == zonNo) == false)
                                    device.ValidationErrors.Add(new ValidationError("Логика зоны имеет неизвестную зону", Level.Critical));
                            }

                            // ПРОВЕРКА ОДНОРОДНОСТИ ОПЕРАТОРОВ ОБЪЕДИНЕНИЯ
                        }
                    }
                    else
                    {
                        if (device.ZoneLogic != null)
                            device.ValidationErrors.Add(new ValidationError("Устройство не может иметь логику срабатывания", Level.Critical));
                    }
                }
            }
        }

        void ValidateAddresses()
        {
            foreach (Device device in configuration.AllDevices)
            {
                Firesec.Metadata.drvType driver = GetDriverByDriverId(device.DriverId);
                if (driver.ar_no_addr == "1")
                {
                    if (string.IsNullOrEmpty(device.Address) == false)
                        device.ValidationErrors.Add(new ValidationError("Устройство не может иметь адрес", Level.Critical));

                    if (driver.addrMask != null)
                    {
                        // ВАЛИДАЦИЯ АДРЕСА
                        ;
                    }
                    else
                    {
                        int intAddress = 0;
                        try
                        {
                            intAddress = System.Convert.ToInt32(device.Address);
                        }
                        catch
                        {
                            device.ValidationErrors.Add(new ValidationError("Адрес должен быть числом", Level.Critical));
                        }

                        if (driver.acr_enabled == "1")
                        {
                            int minAddress = System.Convert.ToInt32(driver.acr_from);
                            int maxAddress = System.Convert.ToInt32(driver.acr_to);
                            if ((intAddress < minAddress) || (intAddress > maxAddress))
                            {
                                device.ValidationErrors.Add(new ValidationError("Адрес должен лежать в диапазоне " + driver.acr_from + " - " + driver.acr_to, Level.Critical));
                            }
                        }
                        else
                        {
                            if (driver.ar_enabled == "1")
                            {
                                int minAddress = System.Convert.ToInt32(driver.ar_from);
                                int maxAddress = System.Convert.ToInt32(driver.ar_to);
                                if ((intAddress < minAddress) || (intAddress > maxAddress))
                                {
                                    device.ValidationErrors.Add(new ValidationError("Адрес должен лежать в диапазоне " + driver.ar_from + " - " + driver.ar_to, Level.Critical));
                                }
                            }
                        }
                    }
                }
            }
        }

        void ValidateChild(Device parent)
        {
            List<string> addresses = new List<string>();

            foreach (Device child in parent.Children)
            {
                string address = child.Address;
                if (addresses.Contains(address))
                    ;// throw new Exception("Адрес дублируется");
                addresses.Add(address);
            }

            foreach (Device child in parent.Children)
            {
                ValidationError error = ValidateDevice(child);
                if (error != null)
                {
                    child.ValidationErrors.Add(error);
                }
                ValidateChild(child);
            }
        }

        ValidationError ValidateDevice(Device device)
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
                        return new ValidationError("Адрес устройства - не число", Level.Critical);
                    }
                    if (intAddress <= 0)
                        return new ValidationError("Адрес не может быть отрицательным", Level.Critical);
                    break;
            }

            if (driverName == "Компьютер")
                return null;

            if (string.IsNullOrEmpty(device.Address))
                return new ValidationError("Пустой адрес устройства", Level.Critical);

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
                        return new ValidationError("Устройство должно иметь адрес в диапазоне от 1 до 100", Level.Critical);
                    break;
                case "Страница":
                    if ((intAddress < 1) || (intAddress > 5))
                        return new ValidationError("Устройство должно иметь адрес в диапазоне от 1 до 5", Level.Critical);
                    break;
                case "Индикатор":
                    if ((intAddress < 1) || (intAddress > 50))
                        return new ValidationError("Устройство должно иметь адрес в диапазоне от 1 до 50", Level.Critical);
                    break;
                case "USB преобразователь МС-1":
                case "USB преобразователь МС-2":
                    break;
                case "USB Канал":
                    if (intAddress != 1)
                        return new ValidationError("Устройство должно иметь адрес 1", Level.Critical);
                    break;
                case "USB Канал МС-1":
                    if (intAddress != 1)
                        return new ValidationError("Устройство должно иметь адрес 1", Level.Critical);
                    break;
                case "USB Канал МС-2":
                    if ((intAddress < 1) || (intAddress > 2))
                        return new ValidationError("Устройство должно иметь адрес в диапазоне от 1 до 2", Level.Critical);
                    break;
                default:
                    break;
            }


            // наличие зоны у устройства

            Firesec.Metadata.drvType driver = GetDriverByDriverId(device.DriverId);
            if ((driver.minZoneCardinality == "1") && (driver.maxZoneCardinality == "1"))
            {
                if (device.ZoneNo == null)
                    return new ValidationError("Устройство должно принадлежать к зоне", Level.Critical);
            }

            // ПОИСК ЗОНЫ В СПИСКЕ ЗОН

            return null;
        }

        public Firesec.Metadata.drvType GetDriverByDriverId(string driverId)
        {
            return Services.CurrentConfiguration.Metadata.drv.FirstOrDefault(x => x.id == driverId);
        }
    }
}
