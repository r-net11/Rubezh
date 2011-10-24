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
            foreach (var device in FiresecManager.DeviceConfiguration.Devices)
            {
                ValidateDeviceComment(device);
                ValidateDeviceOwnerZone(device);
                ValidateDeviceAddressRange(device);
                ValidateEmptyDevice(device);
                ValidateOnValidChars(device);
            }
        }

        static void ValidateDeviceOwnerZone(Device device)
        {
            if (device.Driver.IsZoneDevice && device.ZoneNo == null)
                DeviceErrors.Add(new DeviceError(device, "Устройство должно содержать хотя бы одну зону", ErrorLevel.CannotWrite));
        }

        static void ValidateDeviceComment(Device device)
        {
            if (string.IsNullOrWhiteSpace(device.Description) == false && device.Description.Length > 20)
                DeviceErrors.Add(new DeviceError(device, "Длинное описание - в прибор будет записано описание из первых 20 символов", ErrorLevel.Warning));
        }

        static void ValidateDeviceAddress(Device device)
        {
            if (device.Parent.Children.Where(x => x != device).Any(x => x.IntAddress == device.IntAddress))
                DeviceErrors.Add(new DeviceError(device, "Дублируется адрес устройства", ErrorLevel.CannotWrite));
        }

        static void ValidateDeviceAddressRange(Device device)
        {
            if (device.Driver.IsRangeEnabled && (device.IntAddress > device.Driver.MaxAddress || device.IntAddress < device.Driver.MinAddress))
                DeviceErrors.Add(new DeviceError(device, string.Format("Устройство должно иметь адрес в диапазоне {0}-{1}", device.Driver.MinAddress, device.Driver.MaxAddress), ErrorLevel.CannotWrite));
        }

        static void ValidateEmptyDevice(Device device)
        {
            if (device.Driver.CanWriteDatabase && device.Driver.IsNotValidateZoneAndChildren == false && device.Children.IsNotNullOrEmpty() == false)
                DeviceErrors.Add(new DeviceError(device, "Устройство должно содержать подключенные устройства", ErrorLevel.CannotWrite));
        }

        static void ValidateOnValidChars(Device device)
        {
            if (string.IsNullOrWhiteSpace(device.Description) == false && ValidateString(device.Description) == false)
                DeviceErrors.Add(new DeviceError(device, string.Format("Символы \"{0}\" не допустимы для записи в устройства", InvalidChars(device.Description)), ErrorLevel.CannotWrite));
        }

        static void ValidateZones()
        {
            ZoneErrors = new List<ZoneError>();

            foreach (var zone in FiresecManager.DeviceConfiguration.Zones)
            {
                ValidateZoneNumber(zone);
                ValidateZoneOwnedDevices(zone);
                ValidateZoneDetectorCount(zone);
            }
        }

        static void ValidateZoneNumber(Zone zone)
        {
            if (FiresecManager.DeviceConfiguration.Zones.Where(x => x != zone).Any(x => x.No == zone.No))
                ZoneErrors.Add(new ZoneError(zone, "Дублируется номер зоны", ErrorLevel.CannotSave));
        }

        static void ValidateZoneOwnedDevices(Zone zone)
        {
            if (FiresecManager.DeviceConfiguration.Devices.Any(x => x.Driver.IsZoneDevice && x.ZoneNo == zone.No) == false)
                ZoneErrors.Add(new ZoneError(zone, "В зоне отсутствуют устройства", ErrorLevel.Warning));
        }

        static void ValidateZoneDetectorCount(Zone zone)
        {
            if (zone.DetectorCount < FiresecManager.DeviceConfiguration.Devices.Count(x => x.ZoneNo == zone.No))
                ZoneErrors.Add(new ZoneError(zone, "Количество подключенных к зоне датчиков меньше количества датчиков для сработки", ErrorLevel.Warning));
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