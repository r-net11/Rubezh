using System.Collections.Generic;
using System.Linq;
using FiresecClient.Models;

namespace FiresecClient
{
    public class Validator
    {
        CurrentConfiguration configuration;

        public void Validate(CurrentConfiguration currentConfiguration)
        {
            this.configuration = currentConfiguration;
            configuration.Update();
            ClearValidationErrors();
            ValidateZones();
            ValidateDeviceZones();
            ValidateAddresses();
            ValidateAddressUnique();

            //Device rootDevice = currentConfiguration.RootDevice;
            //ValidateChild(rootDevice);
        }

        void ClearValidationErrors()
        {
            foreach (var device in configuration.Devices)
            {
                device.ValidationErrors = new List<ValidationError>();
            }
            foreach (var zone in configuration.Zones)
            {
                zone.ValidationErrors = new List<ValidationError>();
            }
        }

        void ValidateZones()
        {
            foreach (var zone in configuration.Zones)
            {
                if (configuration.Zones.FindAll(x => x.No == zone.No).Count > 1)
                {
                    zone.ValidationErrors.Add(new ValidationError("Зона с таким номером уже существует", Level.Critical));
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

                if (configuration.Devices.Any(x => x.ZoneNo == zone.No) == false)
                {
                    zone.ValidationErrors.Add(new ValidationError("В зоне отсутствуют устройства", Level.Normal));
                }
            }
        }

        void ValidateDeviceZones()
        {
            foreach (var device in configuration.Devices)
            {
                if (device.Driver.IsZoneDevice)
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

                    if (device.Driver.IsZoneLogicDevice)
                    {
                        if ((device.ZoneLogic != null) && (device.ZoneLogic.clause != null))
                        {
                            foreach (var clause in device.ZoneLogic.clause)
                            {
                                if ((clause.state != "0") && (clause.state != "1") && (clause.state != "2") && (clause.state != "5") && (clause.state != "6"))
                                    device.ValidationErrors.Add(new ValidationError("Логика зоны имеет неизвестный тип состояния", Level.Critical));

                                if ((clause.operation != "and") && (clause.operation != "or"))
                                    device.ValidationErrors.Add(new ValidationError("Логика зоны имеет неизвестный тип операции", Level.Critical));

                                foreach (var zonNo in clause.zone)
                                {
                                    if (configuration.Zones.Any(x => x.No == zonNo) == false)
                                        device.ValidationErrors.Add(new ValidationError("Логика зоны имеет неизвестную зону", Level.Critical));
                                }

                                // ПРОВЕРКА ОДНОРОДНОСТИ ОПЕРАТОРОВ ОБЪЕДИНЕНИЯ
                            }
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
            //foreach (var device in configuration.Devices)
            //{
            //    if (device.Driver.HasAddress == false)
            //    {
            //        if (string.IsNullOrEmpty(device.Address) == false)
            //            device.ValidationErrors.Add(new ValidationError("Устройство не может иметь адрес", Level.Critical));

            //        if (device.Driver.AddressMask != null)
            //        {
            //            switch (device.Driver.AddressMask)
            //            {
            //                case "[0(1)-8(8)]":
            //                    int intAddress = 0;
            //                    try
            //                    {
            //                        intAddress = System.Convert.ToInt32(device.Address);
            //                    }
            //                    catch
            //                    {
            //                        device.ValidationErrors.Add(new ValidationError("Адрес должен быть числом", Level.Critical));
            //                        continue;
            //                    }
            //                    if ((intAddress < 1) || (intAddress > 8))
            //                        device.ValidationErrors.Add(new ValidationError("Устройство не может иметь адрес в диапазоне 1 - 8", Level.Critical));
            //                    break;

            //                case "[8(1)-15(2)];[0(1)-7(255)]":
            //                    string[] addresses = device.Address.Split('.');
            //                    if (addresses.Count() != 2)
            //                    {
            //                        device.ValidationErrors.Add(new ValidationError("Устройство не может иметь адрес, разделенный одной точкой", Level.Critical));
            //                        continue;
            //                    }

            //                    int intShleif;
            //                    try
            //                    {
            //                        intShleif = System.Convert.ToInt32(addresses[0]);
            //                    }
            //                    catch
            //                    {
            //                        device.ValidationErrors.Add(new ValidationError("Адрес должен быть числом", Level.Critical));
            //                        continue;
            //                    }
            //                    try
            //                    {
            //                        intAddress = System.Convert.ToInt32(addresses[1]);
            //                    }
            //                    catch
            //                    {
            //                        device.ValidationErrors.Add(new ValidationError("Адрес должен быть числом", Level.Critical));
            //                        continue;
            //                    }
            //                    if ((intAddress < 1) || (intAddress > 255))
            //                        device.ValidationErrors.Add(new ValidationError("Адрес должен быть в диапазоне 1 - 255", Level.Critical));

            //                    int maxShleifAddress = device.Parent.Driver.ShleifCount;
            //                    if (maxShleifAddress == 0)
            //                    {
            //                        maxShleifAddress = 2;
            //                    }

            //                    if ((intShleif < 1) || (intShleif > maxShleifAddress))
            //                    {
            //                        device.ValidationErrors.Add(new ValidationError("Номер шлейфа должен быть в диапазоне 1 - " + maxShleifAddress.ToString(), Level.Critical));
            //                        continue;
            //                    }

            //                    break;

            //                case "[8(1)-15(4)];[0(1)-7(255)]":
            //                    // АСПТ
            //                    break;
            //                default:
            //                    break;
            //            }
            //        }
            //        else
            //        {
            //            int intAddress = 0;
            //            try
            //            {
            //                intAddress = System.Convert.ToInt32(device.Address);
            //            }
            //            catch
            //            {
            //                device.ValidationErrors.Add(new ValidationError("Адрес должен быть числом", Level.Critical));
            //            }

            //            if (device.Driver.IsAutoCreate)
            //            {
            //                if ((intAddress < device.Driver.MinAutoCreateAddress) || (intAddress > device.Driver.MaxAutoCreateAddress))
            //                {
            //                    device.ValidationErrors.Add(new ValidationError("Адрес должен лежать в диапазоне " + device.Driver.MinAutoCreateAddress + " - " + device.Driver.MaxAutoCreateAddress, Level.Critical));
            //                }
            //            }
            //            else
            //            {
            //                if (device.Driver.IsRangeEnabled)
            //                {
            //                    if ((intAddress < device.Driver.MinAddress) || (intAddress > device.Driver.MaxAddress))
            //                    {
            //                        device.ValidationErrors.Add(new ValidationError("Адрес должен лежать в диапазоне " + device.Driver.MinAddress + " - " + device.Driver.MaxAddress, Level.Critical));
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
        }

        void ValidateAddressUnique()
        {
            foreach (var device in configuration.Devices)
            {
                foreach (var childDevice in device.Children)
                {
                    if (childDevice.Driver.HasAddress)
                    {
                        if (device.Children.FindAll(x => x.IntAddress == childDevice.IntAddress).Count > 1)
                        {
                            childDevice.ValidationErrors.Add(new ValidationError("Устройство с таким адресом уже существует", Level.Normal));
                        }
                    }
                    else
                    {
                        string driverName = device.Driver.DriverName;
                        //if ((childDriver.ar_enabled == "1") && (childDriver.ar_from == "0") && (childDriver.ar_to == "0"))
                        if ((driverName == "Насосная Станция") ||
                            (driverName == "Жокей-насос") ||
                            (driverName == "Компрессор") ||
                            (driverName == "Дренажный насос") ||
                            (driverName == "Насос компенсации утечек"))
                        {
                            if (device.Children.FindAll(x => x.Driver.Id == childDevice.Driver.Id).Count > 1)
                            {
                                childDevice.ValidationErrors.Add(new ValidationError("Устройство должно присутствовать в единственном экзэмпляре", Level.Critical));
                            }
                        }
                        if ((childDevice.Driver.Name == "USB преобразователь МС-1") || (childDevice.Driver.Name == "USB преобразователь МС-2"))
                        {
                            // СЕРИЙНЫЙ НЕМЕР, ЕСЛИ ДВА ОДИНАКОВЫХ УСТРОЙСТВА
                        }
                    }
                }
            }
        }
    }
}
