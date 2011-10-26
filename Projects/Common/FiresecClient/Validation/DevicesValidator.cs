using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using FiresecAPI.Models;

namespace FiresecClient.Validation
{
    public static class DevicesValidator
    {
        public static List<ZoneError> ZoneErrors { get; set; }
        public static List<DeviceError> DeviceErrors { get; set; }
        public static List<DirectionError> DirectionErrors { get; set; }

        public static void Validate()
        {
            ValidateDevices();
            ValidateZones();
            ValidateDirections();
        }

        static void ValidateDevices()
        {
            DeviceErrors = new List<DeviceError>();

            int pduCount = 0;
            foreach (var device in FiresecManager.DeviceConfiguration.Devices)
            {
                if (device.DriverUID == new Guid("B1DF571E-8786-4987-94B2-EC91F7578D20"))
                {
                    ++pduCount;
                }

                if (string.IsNullOrWhiteSpace(device.Description) == false)
                {
                    ValidateDeviceComment(device);
                    ValidateDeviceOnInvalidChars(device);
                }
                ValidateDeviceOwnerZone(device);
                ValidateDeviceAddress(device);
                ValidateDeviceAddressRange(device);
                ValidateDeviceOnEmpty(device);
                ValidateDeviceExtendedZoneLogic(device);
                ValidateDeviceSingleInParent(device);
                ValidateDeviceConflictAddressWithMSChannel(device);
                ValidateDeviceDuplicateSerial(device);
            }

            if (pduCount > 10)
                DeviceErrors.Add(new DeviceError(null, string.Format("Максимальное количество ПДУ - 10, сейчас - {0}", pduCount), ErrorLevel.Warning));
        }

        static void ValidateDeviceComment(Device device)
        {
            if (device.Description.Length > 20)
                DeviceErrors.Add(new DeviceError(device, "Длинное описание - в прибор будет записано описание из первых 20 символов", ErrorLevel.Warning));
        }

        static void ValidateDeviceOnInvalidChars(Device device)
        {
            if (ValidateString(device.Description) == false)
                DeviceErrors.Add(new DeviceError(device, string.Format("Символы \"{0}\" не допустимы для записи в устройства", InvalidChars(device.Description)), ErrorLevel.CannotWrite));
        }

        static void ValidateDeviceOwnerZone(Device device)
        {
            if (device.Driver.IsZoneDevice && device.ZoneNo == null)
                DeviceErrors.Add(new DeviceError(device, "Устройство должно содержать хотя бы одну зону", ErrorLevel.CannotWrite));
        }

        static void ValidateDeviceAddress(Device device)
        {
            if (device.Parent != null && device.IntAddress > 0 && device.Parent.Children.Where(x => x != device).Any(x => x.IntAddress == device.IntAddress))
                DeviceErrors.Add(new DeviceError(device, "Дублируется адрес устройства", ErrorLevel.CannotWrite));
        }

        static void ValidateDeviceAddressRange(Device device)
        {
            if (device.Driver.IsRangeEnabled && (device.IntAddress > device.Driver.MaxAddress || device.IntAddress < device.Driver.MinAddress))
                DeviceErrors.Add(new DeviceError(device, string.Format("Устройство должно иметь адрес в диапазоне {0}-{1}", device.Driver.MinAddress, device.Driver.MaxAddress), ErrorLevel.CannotWrite));
        }

        static void ValidateDeviceOnEmpty(Device device)
        {
            if (device.Driver.CanWriteDatabase && device.Driver.IsNotValidateZoneAndChildren == false && device.Children.Where(x => x.Driver.IsAutoCreate == false).Count() == 0)
                DeviceErrors.Add(new DeviceError(device, "Устройство должно содержать подключенные устройства", ErrorLevel.CannotWrite));
        }

        static void ValidateDeviceExtendedZoneLogic(Device device)
        {
            if (device.Driver.IsZoneLogicDevice && device.ZoneLogic == null)
                DeviceErrors.Add(new DeviceError(device, "Отсутствуют настроенные режимы срабатывания", ErrorLevel.CannotWrite));
        }

        static void ValidateDeviceSingleInParent(Device device)
        {
            if (device.Driver.IsSingleInParent && device.Parent.Children.Count(x => x.DriverUID == device.DriverUID) > 1)
                DeviceErrors.Add(new DeviceError(device, "Устройство должно быть в единственном числе", ErrorLevel.CannotWrite));
        }

        static void ValidateDeviceConflictAddressWithMSChannel(Device device)
        {
            var addressProperty = device.Driver.Properties.FirstOrDefault(x => x.Name == "Address");
            if (addressProperty != null)
            {
                var children = device.Children.FirstOrDefault(x => x.AddressFullPath == addressProperty.Default);
                if (children != null)
                    DeviceErrors.Add(new DeviceError(children, "Конфликт адреса с адресом канала МС", ErrorLevel.CannotWrite));
            }
        }

        static void ValidateDeviceDuplicateSerial(Device device)
        {
            var serialNumberProperty = device.Driver.Properties.FirstOrDefault(x => x.Name == "SerialNo");
            if (serialNumberProperty != null && device.Parent.Children.Where(x => x != device).Any(x => x.DriverUID == device.DriverUID))
                DeviceErrors.Add(new DeviceError(device, "При наличии в конфигурации одинаковых USB устройств, их серийные номера должны быть указаны и отличны", ErrorLevel.CannotWrite));
        }

        static void ValidateZones()
        {
            ZoneErrors = new List<ZoneError>();

            foreach (var zone in FiresecManager.DeviceConfiguration.Zones)
            {
                List<Device> zoneDevices = new List<Device>();
                foreach (var device in FiresecManager.DeviceConfiguration.Devices)
                {
                    if (device.ZoneNo != null)
                    {
                        if (device.ZoneNo == zone.No)
                            zoneDevices.Add(device);
                    }
                    else if (device.ZoneLogic != null && device.ZoneLogic.Clauses.IsNotNullOrEmpty())
                    {
                        if (device.ZoneLogic.Clauses.Any(x => x.Zones.Contains(zone.No)))
                            zoneDevices.Add(device);
                    }
                }

                if (zoneDevices.Count == 0)
                {
                    ZoneErrors.Add(new ZoneError(zone, "В зоне отсутствуют устройства", ErrorLevel.Warning));
                }
                else
                {
                    ValidateZoneDetectorCount(zone, zoneDevices);
                    ValidateZoneOutDevices(zone, zoneDevices);
                    ValidateZoneSingleNS(zone, zoneDevices);
                    ValidateZoneDifferentLine(zone, zoneDevices);
                }

                ValidateZoneNumber(zone);
                ValidateZoneNameLength(zone);
                ValidateZoneDescriptionLength(zone);
                ValidateZoneName(zone);
            }
        }

        static void ValidateZoneDetectorCount(Zone zone, List<Device> zoneDevices)
        {
            if (zone.DetectorCount > zoneDevices.Where(x => x.Driver.IsZoneDevice).Count())
                ZoneErrors.Add(new ZoneError(zone, "Количество подключенных к зоне датчиков меньше количества датчиков для сработки", ErrorLevel.Warning));
        }

        static void ValidateZoneOutDevices(Zone zone, List<Device> zoneDevices)
        {
            if (zoneDevices.All(x => x.Driver.IsOutDevice))
                ZoneErrors.Add(new ZoneError(zone, "К зоне нельзя отнести только выходные устройства", ErrorLevel.CannotWrite));
        }

        static void ValidateZoneSingleNS(Zone zone, List<Device> zoneDevices)
        {
            if (zoneDevices.Where(x => x.DriverUID == new Guid("AF05094E-4556-4CEE-A3F3-981149264E89")).Count() > 1)
                ZoneErrors.Add(new ZoneError(zone, "В одной зоне не может быть несколько внешних НС", ErrorLevel.CannotWrite));
        }

        static void ValidateZoneDifferentLine(Zone zone, List<Device> zoneDevices)
        {
            var zoneAutoCreateDevices = zoneDevices.Where(x => x.Driver.IsAutoCreate).ToList();
            if (zoneAutoCreateDevices.Count > 0)
            {
                foreach (var device in zoneAutoCreateDevices)
                {
                    var shliefCount = device.AddressFullPath.Substring(0, device.AddressFullPath.IndexOf('.'));
                    if (shliefCount != "0" && zoneDevices.Any(x => x.AddressFullPath.Substring(0, x.AddressFullPath.IndexOf('.')) != shliefCount))
                    {
                        ZoneErrors.Add(new ZoneError(zone, string.Format("Адрес встроенного устройства ({0}) в зоне не соответствует номерам шлейфа прочих устройств", device.PresentationAddress), ErrorLevel.CannotWrite));
                        return;
                    }
                }
            }
        }

        static void ValidateZoneNameLength(Zone zone)
        {
            if (zone.Name != null && zone.Name.Length > 20)
                ZoneErrors.Add(new ZoneError(zone, "Слишком длинное наименование зоны (более 20 символов)", ErrorLevel.CannotWrite));
        }

        static void ValidateZoneDescriptionLength(Zone zone)
        {
            if (zone.Description != null && zone.Description.Length > 20)
                ZoneErrors.Add(new ZoneError(zone, "Слишком длинное примечание в зоне (более 256 символов)", ErrorLevel.CannotSave));
        }

        static void ValidateZoneNumber(Zone zone)
        {
            if (FiresecManager.DeviceConfiguration.Zones.Where(x => x != zone).Any(x => x.No == zone.No))
                ZoneErrors.Add(new ZoneError(zone, "Дублируется номер зоны", ErrorLevel.CannotSave));
        }

        static void ValidateZoneName(Zone zone)
        {
            if (string.IsNullOrWhiteSpace(zone.Name))
                ZoneErrors.Add(new ZoneError(zone, "Не указано наименование зоны", ErrorLevel.CannotWrite));
        }

        static void ValidateDirections()
        {
            DirectionErrors = new List<DirectionError>();

            foreach (var direction in FiresecManager.DeviceConfiguration.Directions)
            {
                ValidateDirectionZonesContent(direction);
            }
        }

        static void ValidateDirectionZonesContent(Direction direction)
        {
            if (direction.Zones.IsNotNullOrEmpty() == false)
                DirectionErrors.Add(new DirectionError(direction, "В направлении тушения нет ни одной зоны", ErrorLevel.CannotWrite));
        }

        static bool ValidateString(string str)
        {
            return str.All(x => FiresecManager.DeviceConfiguration.ValidChars.Contains(x));
        }

        static string InvalidChars(string str)
        {
            return new string(str.Where(x => FiresecManager.DeviceConfiguration.ValidChars.Contains(x) == false).ToArray());
        }
    }
}