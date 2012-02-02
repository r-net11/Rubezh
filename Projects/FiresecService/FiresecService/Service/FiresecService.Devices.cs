using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FiresecAPI.Models;
using FiresecService.Converters;

namespace FiresecService
{
    public partial class FiresecService
    {
        void NotifyConfigurationChanged()
        {
            DeviceStatesConverter.Convert();
            CallbackManager.OnConfigurationChanged(this.FiresecServiceUID);
        }

        public void SetDeviceConfiguration(DeviceConfiguration deviceConfiguration)
        {
            ConfigurationFileManager.SetDeviceConfiguration(deviceConfiguration);
            FiresecManager.DeviceConfiguration = deviceConfiguration;

            ConfigurationConverter.ConvertBack(deviceConfiguration, true);
            //FiresecInternalClient.SetNewConfig(ConfigurationConverter.FiresecConfiguration);

            NotifyConfigurationChanged();
        }

        public void DeviceWriteConfiguration(DeviceConfiguration deviceConfiguration, Guid deviceUID)
        {
            ConfigurationConverter.ConvertBack(deviceConfiguration, false);
            var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
            FiresecInternalClient.DeviceWriteConfig(ConfigurationConverter.FiresecConfiguration, device.PlaceInTree);

            NotifyConfigurationChanged();
        }

        public void DeviceWriteAllConfiguration(DeviceConfiguration deviceConfiguration)
        {
            ConfigurationConverter.ConvertBack(deviceConfiguration, false);
            foreach (var device in deviceConfiguration.Devices)
            {
                if (device.Driver.CanWriteDatabase)
                    FiresecInternalClient.DeviceWriteConfig(ConfigurationConverter.FiresecConfiguration, device.PlaceInTree);
            }

            NotifyConfigurationChanged();
        }

        public void DeviceSetPassword(DeviceConfiguration deviceConfiguration, Guid deviceUID, DevicePasswordType devicePasswordType, string password)
        {
            ConfigurationConverter.ConvertBack(deviceConfiguration, false);
            var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
            FiresecInternalClient.DeviceSetPassword(ConfigurationConverter.FiresecConfiguration, device.PlaceInTree, password, (int) devicePasswordType);
        }

        public void DeviceDatetimeSync(DeviceConfiguration deviceConfiguration, Guid deviceUID)
        {
            ConfigurationConverter.ConvertBack(deviceConfiguration, false);
            var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
            FiresecInternalClient.DeviceDatetimeSync(ConfigurationConverter.FiresecConfiguration, device.PlaceInTree);
        }

        public void DeviceRestart(DeviceConfiguration deviceConfiguration, Guid deviceUID)
        {
            ConfigurationConverter.ConvertBack(deviceConfiguration, false);
            var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
            FiresecInternalClient.DeviceRestart(ConfigurationConverter.FiresecConfiguration, device.PlaceInTree);
        }

        public string DeviceGetInformation(DeviceConfiguration deviceConfiguration, Guid deviceUID)
        {
            ConfigurationConverter.ConvertBack(deviceConfiguration, false);
            var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
            return FiresecInternalClient.DeviceGetInformation(ConfigurationConverter.FiresecConfiguration, device.PlaceInTree);
        }

        public List<string> DeviceGetSerialList(DeviceConfiguration deviceConfiguration, Guid deviceUID)
        {
            ConfigurationConverter.ConvertBack(deviceConfiguration, false);
            var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
            string serials = FiresecInternalClient.DeviceGetSerialList(ConfigurationConverter.FiresecConfiguration, device.PlaceInTree);
            if (serials == null)
                return new List<string>();
            return serials.Split(';').ToList();
        }

        public string DeviceUpdateFirmware(DeviceConfiguration deviceConfiguration, Guid deviceUID, byte[] bytes, string fileName)
        {
            lock (Locker)
            {
                fileName = Guid.NewGuid().ToString();
                Directory.CreateDirectory("Temp");
                fileName = Directory.GetCurrentDirectory() + "\\Temp\\" + fileName;
                using (var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    stream.Write(bytes, 0, bytes.Length);
                }

                ConfigurationConverter.ConvertBack(deviceConfiguration, false);
                var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
                return FiresecInternalClient.DeviceUpdateFirmware(ConfigurationConverter.FiresecConfiguration, device.PlaceInTree, fileName);
            }
        }

        public string DeviceVerifyFirmwareVersion(DeviceConfiguration deviceConfiguration, Guid deviceUID, byte[] bytes, string fileName)
        {
            lock (Locker)
            {
                fileName = Guid.NewGuid().ToString();
                Directory.CreateDirectory("Temp");
                fileName = Directory.GetCurrentDirectory() + "\\Temp\\" + fileName;
                using (var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    stream.Write(bytes, 0, bytes.Length);
                }

                ConfigurationConverter.ConvertBack(deviceConfiguration, false);
                var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
                return FiresecInternalClient.DeviceVerifyFirmwareVersion(ConfigurationConverter.FiresecConfiguration, device.PlaceInTree, fileName);
            }
        }

        public string DeviceReadEventLog(DeviceConfiguration deviceConfiguration, Guid deviceUID)
        {
            ConfigurationConverter.ConvertBack(deviceConfiguration, false);
            var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
            return FiresecInternalClient.DeviceReadEventLog(ConfigurationConverter.FiresecConfiguration, device.PlaceInTree);
        }

        public DeviceConfiguration DeviceAutoDetectChildren(DeviceConfiguration deviceConfiguration, Guid deviceUID, bool fastSearch)
        {
            ConfigurationConverter.ConvertBack(deviceConfiguration, false);
            var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
            var config = FiresecInternalClient.DeviceAutoDetectChildren(ConfigurationConverter.FiresecConfiguration, device.PlaceInTree, fastSearch);
            if (config == null)
                return null;

            ConfigurationConverter.DeviceConfiguration = new DeviceConfiguration();
            ConfigurationConverter.FiresecConfiguration = config;
            DeviceConverter.Convert();
            return ConfigurationConverter.DeviceConfiguration;
        }

        public DeviceConfiguration DeviceReadConfiguration(DeviceConfiguration deviceConfiguration, Guid deviceUID)
        {
            ConfigurationConverter.ConvertBack(deviceConfiguration, false);
            var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
            var config = FiresecInternalClient.DeviceReadConfig(ConfigurationConverter.FiresecConfiguration, device.PlaceInTree);
            if (config == null)
                return null;

            ConfigurationConverter.DeviceConfiguration = new DeviceConfiguration();
            ConfigurationConverter.FiresecConfiguration = config;
            DeviceConverter.Convert();
            return ConfigurationConverter.DeviceConfiguration;
        }

        public List<DeviceCustomFunction> DeviceCustomFunctionList(Guid driverUID)
        {
            var functions = FiresecInternalClient.DeviceCustomFunctionList(driverUID.ToString().ToUpper());
            return DeviceCustomFunctionConverter.Convert(functions);
        }

        public string DeviceCustomFunctionExecute(DeviceConfiguration deviceConfiguration, Guid deviceUID, string functionName)
        {
            ConfigurationConverter.ConvertBack(deviceConfiguration, false);
            var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
            return FiresecInternalClient.DeviceCustomFunctionExecute(ConfigurationConverter.FiresecConfiguration, device.PlaceInTree, functionName);
        }

        public string DeviceGetGuardUsersList(DeviceConfiguration deviceConfiguration, Guid deviceUID)
        {
            ConfigurationConverter.ConvertBack(deviceConfiguration, false);
            var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
            return FiresecInternalClient.DeviceGetGuardUsersList(ConfigurationConverter.FiresecConfiguration, device.PlaceInTree);
        }

        public string DeviceGetMDS5Data(DeviceConfiguration deviceConfiguration, Guid deviceUID)
        {
            ConfigurationConverter.ConvertBack(deviceConfiguration, false);
            var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
            return FiresecInternalClient.DeviceGetMDS5Data(ConfigurationConverter.FiresecConfiguration, device.PlaceInTree);
        }

        public void AddToIgnoreList(List<Guid> deviceGuids)
        {
            var devicePaths = new List<string>();
            foreach (var guid in deviceGuids)
            {
                var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == guid);
                devicePaths.Add(device.PlaceInTree);
            }

            FiresecInternalClient.AddToIgnoreList(devicePaths);
        }

        public void RemoveFromIgnoreList(List<Guid> deviceGuids)
        {
            var devicePaths = new List<string>();
            foreach (var guid in deviceGuids)
            {
                var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == guid);
                devicePaths.Add(device.PlaceInTree);
            }

            FiresecInternalClient.RemoveFromIgnoreList(devicePaths);
        }

        public void ResetStates(List<ResetItem> resetItems)
        {
            FiresecResetHelper.ResetMany(resetItems);
        }

        public void AddUserMessage(string message)
        {
            FiresecInternalClient.AddUserMessage(message);
        }

        public void ExecuteCommand(Guid deviceUID, string methodName)
        {
            var device = FiresecManager.DeviceConfigurationStates.DeviceStates.FirstOrDefault(x => x.UID == deviceUID);
            if (device != null)
            {
                FiresecInternalClient.ExecuteCommand(device.PlaceInTree, methodName);
            }
        }
    }
}