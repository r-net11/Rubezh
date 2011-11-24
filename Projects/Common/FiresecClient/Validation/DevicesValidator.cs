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

        static readonly DriverType NsDriverType = DriverType.PumpStation;
        static readonly DriverType PduDriverType = DriverType.PDU;
        static readonly DriverType IndicatorDriverType = DriverType.Indicator;
        static readonly DriverType ZadvizhkaDriverType = DriverType.Valve;

        static List<Guid> _validateDevicesWithSerialNumber;

        public static void Validate()
        {
            Action deviceValidator = new Action(ValidateDevices);
            IAsyncResult deviceValidationResult = deviceValidator.BeginInvoke(null, null);

            Action zoneValidator = new Action(ValidateZones);
            IAsyncResult zoneValidationResult = zoneValidator.BeginInvoke(null, null);

            ValidateDirections();

            zoneValidator.EndInvoke(zoneValidationResult);
            deviceValidator.EndInvoke(deviceValidationResult);
        }

        static void ValidateDevices()
        {
            DeviceErrors = new List<DeviceError>();
            _validateDevicesWithSerialNumber = new List<Guid>();
            int pduCount = 0;

            foreach (var device in FiresecManager.DeviceConfiguration.Devices)
            {
                //if (device.DriverUID == new Guid("96CDBD7E-29F6-45D4-9028-CF10332FAB1A"))
                //{
                //    ++pduCount;
                //    --pduCount;
                //}
                if (device.Driver.DriverType == PduDriverType)
                {
                    ++pduCount;
                }
                else if (device.Driver.DriverType == IndicatorDriverType)
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
                ValidateDeviceMaxDeviceOnLine(device);
                ValidateDeviceOwnerZone(device);
                ValidateDeviceAddress(device);
                ValidateDeviceAddressRange(device);
                ValidateDeviceOnEmpty(device);
                ValidateDeviceExtendedZoneLogic(device);
                ValidateDeviceSingleInParent(device);
                ValidateDeviceConflictAddressWithMSChannel(device);
                ValidateDeviceDuplicateSerial(device);
                ValidateDeviceSecurity(device);
                ValidateDeviceEvents(device);
                ValidateDeviceLoopLines(device);
                ValidateDeviceMaxExtCount(device);
                ValidateDeviceSecurityPanel(device);
                ValidateDeviceRangeAddress(device);
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

        static void ValidateDeviceMaxDeviceOnLine(Device device)
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
            var driverAddressProperty = device.Driver.Properties.FirstOrDefault(x => x.Name == "Address");
            if (driverAddressProperty != null)
            {
                var deviceAddressProperty = device.Properties.FirstOrDefault(x => x.Name == driverAddressProperty.Name);
                var address = deviceAddressProperty == null ? driverAddressProperty.Default : deviceAddressProperty.Value;

                var children = device.Children.FirstOrDefault(x => x.AddressFullPath == address);
                if (children != null)
                    DeviceErrors.Add(new DeviceError(children, "Конфликт адреса с адресом канала МС", ErrorLevel.CannotWrite));
            }
        }

        static void ValidateDeviceDuplicateSerial(Device device)
        {
            var driverSerialNumberProperty = device.Driver.Properties.FirstOrDefault(x => x.Name == "SerialNo");
            if (driverSerialNumberProperty == null || _validateDevicesWithSerialNumber.Contains(device.DriverUID))
                return;

            var similarDevices = device.Parent.Children.Where(x => x.DriverUID == device.DriverUID).ToList();
            if (similarDevices.Count > 1)
            {
                _validateDevicesWithSerialNumber.Add(device.DriverUID);
                var serialNumbers = similarDevices.Select(x => GetSerialNumber(x)).ToList();
                for (int i = 0; i < serialNumbers.Count; ++i)
                {
                    if (string.IsNullOrWhiteSpace(serialNumbers[i]) || serialNumbers.Count(x => x == serialNumbers[i]) > 1)
                        DeviceErrors.Add(new DeviceError(similarDevices[i], "При наличии в конфигурации одинаковых USB устройств, их серийные номера должны быть указаны и отличны", ErrorLevel.CannotWrite));
                }
            }
        }

        static string GetSerialNumber(Device device)
        {
            var deviceSerialNumberProperty = device.Properties.FirstOrDefault(x => x.Name == "SerialNo");
            return deviceSerialNumberProperty == null ? device.Driver.Properties.First(x => x.Name == "SerialNo").Default : deviceSerialNumberProperty.Value;
        }

        static void ValidateDeviceSecurity(Device device)
        {
            if (device.Driver.DeviceType == DeviceType.Sequrity)
            {
                if ((device.IntAddress & 0xff) > 250)
                    DeviceErrors.Add(new DeviceError(device, "Не рекомендуется использовать адрес охранного устройства больше 250", ErrorLevel.CannotWrite));
                if (device.Parent.Driver.Properties.Any(x => x.Name == "DeviceCountSecDev") == false)
                    DeviceErrors.Add(new DeviceError(device, "Устройство подключено к недопустимому устройству", ErrorLevel.CannotWrite));
            }
        }

        static void ValidateDeviceEvents(Device device)
        {
            var eventProperty = device.Properties.FirstOrDefault(x => x.Name == "Event1");
            if (eventProperty != null && eventProperty.Value.Length > 20)
            {
                DeviceErrors.Add(new DeviceError(device, "Длинное описание события - в прибор будет записано первые 20 символов", ErrorLevel.Warning));
            }
            else
            {
                eventProperty = device.Properties.FirstOrDefault(x => x.Name == "Event2");
                if (eventProperty != null && eventProperty.Value.Length > 20)
                    DeviceErrors.Add(new DeviceError(device, "Длинное описание события - в прибор будет записано первые 20 символов", ErrorLevel.Warning));
            }
        }

        static void ValidateDeviceLoopLines(Device device)
        {
            var loopLineProperty = device.Properties.FirstOrDefault(x => x.Name == "LoopLine1");
            if (loopLineProperty != null)
            {
                var badChildren = device.Children.Where(x => x.IntAddress >> 8 == 2).ToList();
                badChildren.ForEach(x => DeviceErrors.Add(new DeviceError(x, "Данное устройство находится на четном номере АЛС, что недопустимо для кольцевых АЛС", ErrorLevel.CannotWrite)));
            }

            loopLineProperty = device.Properties.FirstOrDefault(x => x.Name == "LoopLine2");
            if (loopLineProperty != null)
            {
                var badChildren = device.Children.Where(x => x.IntAddress >> 8 == 4).ToList();
                badChildren.ForEach(x => DeviceErrors.Add(new DeviceError(x, "Данное устройство находится на четном номере АЛС, что недопустимо для кольцевых АЛС", ErrorLevel.CannotWrite)));
            }
        }

        static void ValidateDeviceMaxExtCount(Device device)
        {
            if (device.Driver.HasShleif && device.Children.IsNotNullOrEmpty())
            {
                var childrenZones = device.Children.Where(x => x.Driver.IsZoneDevice && x.ZoneNo != null).Select(x => x.ZoneNo).Distinct().ToList();
                if (childrenZones.IsNotNullOrEmpty() == false)
                    return;

                var childrenZonesDevices = new List<Device>();
                childrenZones.ForEach(x => childrenZonesDevices.AddRange(GetZoneDevices(x)));

                int extendedDevicesCount = childrenZonesDevices.Where(x => x.Driver.IsZoneLogicDevice && x.Parent != device).Distinct().Count();
                if (extendedDevicesCount > 250)
                    DeviceErrors.Add(new DeviceError(device, string.Format("В приборе не может быть более 250 внешних устройств. Сейчас : {0}", extendedDevicesCount), ErrorLevel.CannotWrite));
            }
        }

        static void ValidateDeviceSecurityPanel(Device device)
        {
            var driverSecurityDeviceCountProperty = device.Driver.Properties.FirstOrDefault(x => x.Name == "DeviceCountSecDev");
            if (driverSecurityDeviceCountProperty != null)
            {
                var securityDeviceCountPropertyValue = driverSecurityDeviceCountProperty.Parameters.First(x => x.Value == driverSecurityDeviceCountProperty.Default).Name;
                var deviceSecurityDeviceCountProperty = device.Properties.FirstOrDefault(x => x.Name == "DeviceCountSecDev");
                if (deviceSecurityDeviceCountProperty != null)
                    securityDeviceCountPropertyValue = deviceSecurityDeviceCountProperty.Value;

                if (securityDeviceCountPropertyValue == driverSecurityDeviceCountProperty.Parameters[0].Name)
                    ValidateDeviceCountAndOrderOnShlief(device, 64, 0);
                else if (securityDeviceCountPropertyValue == driverSecurityDeviceCountProperty.Parameters[1].Name)
                    ValidateDeviceCountAndOrderOnShlief(device, 48, 16);
                else if (securityDeviceCountPropertyValue == driverSecurityDeviceCountProperty.Parameters[2].Name)
                    ValidateDeviceCountAndOrderOnShlief(device, 32, 32);
                else if (securityDeviceCountPropertyValue == driverSecurityDeviceCountProperty.Parameters[3].Name)
                    ValidateDeviceCountAndOrderOnShlief(device, 16, 48);
                else if (securityDeviceCountPropertyValue == driverSecurityDeviceCountProperty.Parameters[4].Name)
                    ValidateDeviceCountAndOrderOnShlief(device, 0, 64);
            }
        }

        static void ValidateDeviceCountAndOrderOnShlief(Device device, int firstShliefMaxCount, int secondShliefMaxCount)
        {
            int deviceOnFirstShliefCount = 0;
            int deviceOnSecondShliefCount = 0;
            int shliefNumber = 0;
            int firstShliefDeviceNumber = 0;
            int firstShliefDevicePrevNumber = 0;
            int secondShliefDeviceNumber = 0;
            int secondShliefDevicePrevNumber = 0;
            bool isFirstShliefOrederCorrupt = false;
            bool isSecondShliefOrederCorrupt = false;

            foreach (var intAddress in device.Children.Where(x => x.Driver.DeviceType == DeviceType.Sequrity).Select(x => x.IntAddress))
            {
                shliefNumber = intAddress >> 8;
                if (shliefNumber == 1)
                {
                    ++deviceOnFirstShliefCount;
                    firstShliefDevicePrevNumber = firstShliefDeviceNumber;
                    firstShliefDeviceNumber = intAddress & 0xff;
                    if (isFirstShliefOrederCorrupt == false)
                    {
                        if (firstShliefDeviceNumber < 176 || (firstShliefDevicePrevNumber > 0 && (firstShliefDeviceNumber - firstShliefDevicePrevNumber) > 1))
                            isFirstShliefOrederCorrupt = true;
                    }
                }
                else if (shliefNumber == 2)
                {
                    ++deviceOnSecondShliefCount;
                    secondShliefDevicePrevNumber = secondShliefDeviceNumber;
                    secondShliefDeviceNumber = intAddress & 0xff;
                    if (isSecondShliefOrederCorrupt == false)
                    {
                        if (secondShliefDeviceNumber < 176 || (secondShliefDevicePrevNumber > 0 && (secondShliefDeviceNumber - secondShliefDevicePrevNumber) > 1))
                            isSecondShliefOrederCorrupt = true;
                    }
                }
            }
            if (deviceOnFirstShliefCount > firstShliefMaxCount)
                DeviceErrors.Add(new DeviceError(device, "Превышено максимальное количество подключаемых охранных устройств на 1-ом шлейфе", ErrorLevel.CannotWrite));
            if (deviceOnSecondShliefCount > secondShliefMaxCount)
                DeviceErrors.Add(new DeviceError(device, "Превышено максимальное количество подключаемых охранных устройств на 2-ом шлейфе", ErrorLevel.CannotWrite));
            if (isFirstShliefOrederCorrupt)
                DeviceErrors.Add(new DeviceError(device, "Необходима неразрывная последовательность адресов охранных устройств на 1-ом шлейфе начиная  с 176 адреса", ErrorLevel.Warning));
            if (isSecondShliefOrederCorrupt)
                DeviceErrors.Add(new DeviceError(device, "Необходима неразрывная последовательность адресов охранных устройств на 2-ом шлейфе начиная  с 176 адреса", ErrorLevel.Warning));
        }

        static void ValidateDeviceRangeAddress(Device device)
        {
            if (device.Driver.IsChildAddressReservedRange && device.Driver.ChildAddressReserveRangeCount > 0)
            {
                if (device.Children.Any(x => x.IntAddress < device.IntAddress || (x.IntAddress - device.IntAddress) > device.Driver.ChildAddressReserveRangeCount))
                    DeviceErrors.Add(new DeviceError(device, string.Format("Для всех подключенных устройтв необходимо выбрать адрес из диапазона: {0}", device.PresentationAddress), ErrorLevel.Warning));
            }
        }

        static void ValidateZones()
        {
            ZoneErrors = new List<ZoneError>();

            int guardZonesCount = 0;
            foreach (var zone in FiresecManager.DeviceConfiguration.Zones)
            {
                List<Device> zoneDevices = GetZoneDevices(zone.No).ToList();

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

        static void ValidateDirections()
        {
            DirectionErrors = new List<DirectionError>();

            foreach (var direction in FiresecManager.DeviceConfiguration.Directions)
            {
                if (ValidateDirectionZonesContent(direction))
                { }
            }
        }

        static void ValidateZoneOutDevices(Zone zone, List<Device> zoneDevices)
        {
            if (zoneDevices.All(x => x.Driver.IsOutDevice))
                ZoneErrors.Add(new ZoneError(zone, "К зоне нельзя отнести только выходные устройства", ErrorLevel.CannotWrite));
        }

        static void ValidateZoneSingleNS(Zone zone, List<Device> zoneDevices)
        {
            if (zoneDevices.Where(x => x.Driver.DriverType == NsDriverType).Count() > 1)
                ZoneErrors.Add(new ZoneError(zone, "В одной зоне не может быть несколько внешних НС", ErrorLevel.CannotWrite));
        }

        static void ValidateZoneDifferentLine(Zone zone, List<Device> zoneDevices)
        {
            var zoneAutoCreateDevices = zoneDevices.Where(x => x.Driver.IsAutoCreate && x.Driver.IsDeviceOnShleif).ToList();
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
            if (zoneDevices.Count(x => x.Driver.DriverType == ZadvizhkaDriverType) > 1 && FiresecManager.DeviceConfiguration.Directions.Any(x => x.Zones.Contains(zone.No)))
                ZoneErrors.Add(new ZoneError(zone, "В зоне направления не может быть более одной задвижки", ErrorLevel.CannotWrite));
        }

        static void ValidateZoneNameLength(Zone zone)
        {
            if (zone.Name != null && zone.Name.Length > 20)
                ZoneErrors.Add(new ZoneError(zone, "Слишком длинное наименование зоны (более 20 символов)", ErrorLevel.CannotWrite));
        }

        static void ValidateZoneDescriptionLength(Zone zone)
        {
            if (zone.Description != null && zone.Description.Length > 256)
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

        static IEnumerable<Device> GetZoneDevices(ulong? zoneNo)
        {
            var zoneDevices = new List<Device>();
            foreach (var device in FiresecManager.DeviceConfiguration.Devices)
            {
                if (device.ZoneNo != null)
                {
                    if (device.ZoneNo == zoneNo)
                        yield return device;
                }
                else if (device.ZoneLogic != null && device.ZoneLogic.Clauses.IsNotNullOrEmpty())
                {
                    if (device.ZoneLogic.Clauses.Any(x => x.Zones.Contains(zoneNo)))
                        yield return device;
                }
            }
        }
    }
}