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

        static readonly Guid NsGuid = new Guid("AF05094E-4556-4CEE-A3F3-981149264E89");
        static readonly Guid PduGuid = new Guid("B1DF571E-8786-4987-94B2-EC91F7578D20");
        static readonly Guid IndicatorGuid = new Guid("E486745F-6130-4027-9C01-465DE5415BBF");
        static readonly Guid ZadvizhkaGuid = new Guid("4935848F-0084-4151-A0C8-3A900E3CB5C5");

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
            //foreach (var device in FiresecManager.DeviceConfiguration.Devices.Where(x => x.Driver.DeviceType == DeviceType.Sequrity))
            //{
            //    ++pduCount;
            //    --pduCount;
            //}
            foreach (var device in FiresecManager.DeviceConfiguration.Devices)
            {
                if (device.DriverUID == PduGuid)
                {
                    ++pduCount;
                }
                else if (device.DriverUID == IndicatorGuid)
                {
                    if (device.IndicatorLogic.IndicatorLogicType == IndicatorLogicType.Zone)
                        ValidateDeviceIndicatorOtherNetworkZone(device);
                    else
                        ValidateDeviceIndicatorOtherNetworkDevice(device);
                }

                if (string.IsNullOrWhiteSpace(device.Description) == false)
                {
                    ValidateDeviceComment(device);
                    ValidateDeviceOnInvalidChars(device);
                }
                ValidateDeviceMaximumDeviceOnLine(device);
                ValidateDeviceOwnerZone(device);
                ValidateDeviceAddress(device);
                ValidateDeviceAddressRange(device);
                ValidateDeviceOnEmpty(device);
                ValidateDeviceExtendedZoneLogic(device);
                ValidateDeviceSingleInParent(device);
                ValidateDeviceConflictAddressWithMSChannel(device);
                ValidateDeviceDuplicateSerial(device);
                ValidateDeviceSecAddress(device);
            }

            if (pduCount > 10)
                DeviceErrors.Add(new DeviceError(null, string.Format("Максимальное количество ПДУ - 10, сейчас - {0}", pduCount), ErrorLevel.Warning));
        }

        static void ValidateDeviceIndicatorOtherNetworkDevice(Device device)
        {
            if (device.IndicatorLogic.Device != null && device.IndicatorLogic.Device.AllParents.IsNotNullOrEmpty() && (device.IndicatorLogic.Device.AllParents[1] != device.AllParents[1] || device.IndicatorLogic.Device.AllParents[2] != device.AllParents[2]))
                DeviceErrors.Add(new DeviceError(device, "Для индикатора указано устройство находящееся в другой сети RS-485", ErrorLevel.CannotWrite));
        }

        static void ValidateDeviceIndicatorOtherNetworkZone(Device device)
        {
            var zone = device.IndicatorLogic.Zones.FirstOrDefault(
                zoneNo => GetZoneDevices(zoneNo).Any(x => x.AllParents.IsNotNullOrEmpty() && (x.AllParents[1] != device.AllParents[1] || x.AllParents[2] != device.AllParents[2])));
            if (zone != null)
                DeviceErrors.Add(new DeviceError(device, string.Format("Для индикатора указана зона ({0}) имеющая устройства другой сети RS-485", zone), ErrorLevel.CannotWrite));
        }

        static void ValidateDeviceMaximumDeviceOnLine(Device device)
        {
            if (device.Driver.HasShleif)
            {
                for (int i = 1; i <= device.Driver.ShleifCount; ++i)
                {
                    if (device.Children.Count(x => x.IntAddress >> 8 == i) > 255)
                    {
                        DeviceErrors.Add(new DeviceError(device, "Число устройств на шлейфе не может превышать 255", ErrorLevel.CannotWrite));
                        return;
                    }
                }
            }
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

        static void ValidateDeviceSecAddress(Device device)
        {
            if (device.Driver.DeviceType == DeviceType.Sequrity && (device.IntAddress & 0xff) > 250)
                DeviceErrors.Add(new DeviceError(device, "Не рекомендуется использовать адрес охранного устройства больше 250", ErrorLevel.CannotWrite));
        }

        static void ValidateZones()
        {
            ZoneErrors = new List<ZoneError>();

            int guardZonesCount = 0;
            foreach (var zone in FiresecManager.DeviceConfiguration.Zones)
            {
                List<Device> zoneDevices = GetZoneDevices(zone.No);

                if (zoneDevices.Count == 0)
                {
                    ZoneErrors.Add(new ZoneError(zone, "В зоне отсутствуют устройства", ErrorLevel.Warning));
                }
                else
                {
                    ValidateZoneDetectorCount(zone, zoneDevices);
                    ValidateZoneType(zone, zoneDevices);
                    ValidateZoneOutDevices(zone, zoneDevices);
                    ValidateZoneSingleNS(zone, zoneDevices);
                    ValidateZoneDifferentLine(zone, zoneDevices);
                    ValidateZoneSingleBoltInDirectionZone(zone, zoneDevices);
                }

                ValidateZoneNumber(zone);
                ValidateZoneNameLength(zone);
                ValidateZoneDescriptionLength(zone);
                ValidateZoneName(zone);

                if (zone.ZoneType == ZoneType.Guard)
                    ++guardZonesCount;
            }
            if (guardZonesCount > 64)
                ZoneErrors.Add(new ZoneError(null, string.Format("Превышено максимальное количество охранных зон ({0} из 64 максимально возможных)", guardZonesCount), ErrorLevel.CannotWrite));
        }

        static void ValidateZoneDetectorCount(Zone zone, List<Device> zoneDevices)
        {
            if (zone.DetectorCount > zoneDevices.Where(x => x.Driver.IsZoneDevice).Count())
                ZoneErrors.Add(new ZoneError(zone, "Количество подключенных к зоне датчиков меньше количества датчиков для сработки", ErrorLevel.Warning));
        }

        static void ValidateZoneType(Zone zone, List<Device> zoneDevices)
        {
            switch (zone.ZoneType)
            {
                case ZoneType.Fire:
                    var guardDevice = zoneDevices.FirstOrDefault(x => x.Driver.DeviceType == DeviceType.Sequrity);
                    if (guardDevice != null)
                        ZoneErrors.Add(new ZoneError(zone, string.Format("В зону не может быть помещено охранное устройство ({0})", guardDevice.PresentationAddress), ErrorLevel.CannotSave));
                    break;

                case ZoneType.Guard:
                    var fireDevice = zoneDevices.FirstOrDefault(x => x.Driver.DeviceType == DeviceType.Fire);
                    if (fireDevice != null)
                        ZoneErrors.Add(new ZoneError(zone, string.Format("В зону не может быть помещено пожарное устройство ({0})", fireDevice.PresentationAddress), ErrorLevel.CannotSave));
                    break;
            }
        }

        static void ValidateZoneOutDevices(Zone zone, List<Device> zoneDevices)
        {
            if (zoneDevices.All(x => x.Driver.IsOutDevice))
                ZoneErrors.Add(new ZoneError(zone, "К зоне нельзя отнести только выходные устройства", ErrorLevel.CannotWrite));
        }

        static void ValidateZoneSingleNS(Zone zone, List<Device> zoneDevices)
        {
            if (zoneDevices.Where(x => x.DriverUID == NsGuid).Count() > 1)
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

        static void ValidateZoneSingleBoltInDirectionZone(Zone zone, List<Device> zoneDevices)
        {
            if (zoneDevices.Count(x => x.DriverUID == ZadvizhkaGuid) > 1 && FiresecManager.DeviceConfiguration.Directions.Any(x => x.Zones.Contains(zone.No)))
                ZoneErrors.Add(new ZoneError(zone, "В зоне направления не может быть более одной задвижки", ErrorLevel.CannotWrite));
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
                if (ValidateDirectionZonesContent(direction))
                {
                }
            }
        }

        static bool ValidateDirectionZonesContent(Direction direction)
        {
            if (direction.Zones.IsNotNullOrEmpty() == false)
            {
                DirectionErrors.Add(new DirectionError(direction, "В направлении тушения нет ни одной зоны", ErrorLevel.CannotWrite));
                return false;
            }
            return true;
        }

        static bool ValidateString(string str)
        {
            return str.All(x => FiresecManager.DeviceConfiguration.ValidChars.Contains(x));
        }

        static string InvalidChars(string str)
        {
            return new string(str.Where(x => FiresecManager.DeviceConfiguration.ValidChars.Contains(x) == false).ToArray());
        }

        static List<Device> GetZoneDevices(ulong? zoneNo)
        {
            var zoneDevices = new List<Device>();
            foreach (var device in FiresecManager.DeviceConfiguration.Devices)
            {
                if (device.ZoneNo != null)
                {
                    if (device.ZoneNo == zoneNo)
                        zoneDevices.Add(device);
                }
                else if (device.ZoneLogic != null && device.ZoneLogic.Clauses.IsNotNullOrEmpty())
                {
                    if (device.ZoneLogic.Clauses.Any(x => x.Zones.Contains(zoneNo)))
                        zoneDevices.Add(device);
                }
            }

            return zoneDevices;
        }
    }
}