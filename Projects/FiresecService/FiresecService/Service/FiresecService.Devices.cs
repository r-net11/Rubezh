using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FiresecAPI.Models;
using FiresecService.Converters;
using Firesec;
using FiresecAPI;

namespace FiresecService
{
    public static class ResultDataExtentions
    {
        public static OperationResult<T> ToOperationResult<T>(this FiresecOperationResult<T> resultData)
        {
            var operationResult = new OperationResult<T>()
            {
                Result = resultData.Result,
                HasError = resultData.HasError,
                Error = resultData.Error,
            };
            return operationResult;
        }
    }

    public partial class FiresecService
    {
        void NotifyConfigurationChanged()
        {
            DeviceStatesConverter.Convert();
            CallbackManager.OnConfigurationChanged();
        }

        public OperationResult<bool> SetDeviceConfiguration(DeviceConfiguration deviceConfiguration)
        {
            ConfigurationFileManager.SetDeviceConfiguration(deviceConfiguration);
            FiresecManager.DeviceConfiguration = deviceConfiguration;

            ConfigurationConverter.ConvertBack(deviceConfiguration, true);
            var result = FiresecInternalClient.SetNewConfig(ConfigurationConverter.FiresecConfiguration).ToOperationResult();

            NotifyConfigurationChanged();

            return result;
        }

        public OperationResult<bool> DeviceWriteConfiguration(DeviceConfiguration deviceConfiguration, Guid deviceUID)
        {
            ConfigurationConverter.ConvertBack(deviceConfiguration, false);
            var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
            var result = FiresecInternalClient.DeviceWriteConfig(ConfigurationConverter.FiresecConfiguration, device.PlaceInTree).ToOperationResult();

            NotifyConfigurationChanged();

            return result;
        }

        public OperationResult<bool> DeviceWriteAllConfiguration(DeviceConfiguration deviceConfiguration)
        {
            OperationResult<bool> result = null;
            ConfigurationConverter.ConvertBack(deviceConfiguration, false);
            foreach (var device in deviceConfiguration.Devices)
            {
                if (device.Driver.CanWriteDatabase)
                {
                    result = FiresecInternalClient.DeviceWriteConfig(ConfigurationConverter.FiresecConfiguration, device.PlaceInTree).ToOperationResult();
                    if (result.HasError)
                        return result;
                    System.Threading.Thread.Sleep(1000);
                }
            }

            NotifyConfigurationChanged();

            return result;
        }

        public OperationResult<bool> DeviceSetPassword(DeviceConfiguration deviceConfiguration, Guid deviceUID, DevicePasswordType devicePasswordType, string password)
        {
            ConfigurationConverter.ConvertBack(deviceConfiguration, false);
            var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
            return FiresecInternalClient.DeviceSetPassword(ConfigurationConverter.FiresecConfiguration, device.PlaceInTree, password, (int)devicePasswordType).ToOperationResult();
        }

        public OperationResult<bool> DeviceDatetimeSync(DeviceConfiguration deviceConfiguration, Guid deviceUID)
        {
            ConfigurationConverter.ConvertBack(deviceConfiguration, false);
            var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
            return FiresecInternalClient.DeviceDatetimeSync(ConfigurationConverter.FiresecConfiguration, device.PlaceInTree).ToOperationResult();
        }

        public OperationResult<string> DeviceGetInformation(DeviceConfiguration deviceConfiguration, Guid deviceUID)
        {
            ConfigurationConverter.ConvertBack(deviceConfiguration, false);
            var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
            return FiresecInternalClient.DeviceGetInformation(ConfigurationConverter.FiresecConfiguration, device.PlaceInTree).ToOperationResult();
        }

        public OperationResult<List<string>> DeviceGetSerialList(DeviceConfiguration deviceConfiguration, Guid deviceUID)
        {
            ConfigurationConverter.ConvertBack(deviceConfiguration, false);
            var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
            var result = FiresecInternalClient.DeviceGetSerialList(ConfigurationConverter.FiresecConfiguration, device.PlaceInTree);

            var operationResult = new OperationResult<List<string>>()
            {
                HasError = result.HasError,
                Error = result.Error
            };
            if (result.Result != null)
                operationResult.Result = result.Result.Split(';').ToList();
            return operationResult;
        }

        public OperationResult<string> DeviceUpdateFirmware(DeviceConfiguration deviceConfiguration, Guid deviceUID, byte[] bytes, string fileName)
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
                //fileName = "D:/Projects/3rdParty/Firesec/XHC/Рубеж-2АМ/2_25/sborka2_25(161211)_1.HXC";
                return FiresecInternalClient.DeviceUpdateFirmware(ConfigurationConverter.FiresecConfiguration, device.PlaceInTree, fileName).ToOperationResult();
            }
        }

        public OperationResult<string> DeviceVerifyFirmwareVersion(DeviceConfiguration deviceConfiguration, Guid deviceUID, byte[] bytes, string fileName)
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
                //`fileName = "D:/Projects/3rdParty/Firesec/XHC/Рубеж-2АМ/2_25/sborka2_25(161211)_2.HXC";
                return FiresecInternalClient.DeviceVerifyFirmwareVersion(ConfigurationConverter.FiresecConfiguration, device.PlaceInTree, fileName).ToOperationResult();
            }
        }

        public OperationResult<string> DeviceReadEventLog(DeviceConfiguration deviceConfiguration, Guid deviceUID)
        {
            ConfigurationConverter.ConvertBack(deviceConfiguration, false);
            var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
            return FiresecInternalClient.DeviceReadEventLog(ConfigurationConverter.FiresecConfiguration, device.PlaceInTree).ToOperationResult();
        }

        public OperationResult<DeviceConfiguration> DeviceAutoDetectChildren(DeviceConfiguration deviceConfiguration, Guid deviceUID, bool fastSearch)
        {
            ConfigurationConverter.ConvertBack(deviceConfiguration, false);
            var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
            var result = FiresecInternalClient.DeviceAutoDetectChildren(ConfigurationConverter.FiresecConfiguration, device.PlaceInTree, fastSearch);

            var operationResult = new OperationResult<DeviceConfiguration>()
            {
                HasError = result.HasError,
                Error = result.Error
            };

            ConfigurationConverter.DeviceConfiguration = new DeviceConfiguration();
            ConfigurationConverter.FiresecConfiguration = result.Result;
            DeviceConverter.Convert();
            operationResult.Result = ConfigurationConverter.DeviceConfiguration;

            return operationResult;
        }

        public OperationResult<DeviceConfiguration> DeviceReadConfiguration(DeviceConfiguration deviceConfiguration, Guid deviceUID)
        {
            ConfigurationConverter.ConvertBack(deviceConfiguration, false);
            var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
            var result = FiresecInternalClient.DeviceReadConfig(ConfigurationConverter.FiresecConfiguration, device.PlaceInTree);
            
            var operationResult = new OperationResult<DeviceConfiguration>()
            {
                HasError = result.HasError,
                Error = result.Error
            };

            ConfigurationConverter.DeviceConfiguration = new DeviceConfiguration();
            ConfigurationConverter.FiresecConfiguration = result.Result;
            DeviceConverter.Convert();
            operationResult.Result = ConfigurationConverter.DeviceConfiguration;

            return operationResult;
        }

        public OperationResult<List<DeviceCustomFunction>> DeviceCustomFunctionList(Guid driverUID)
        {
            var result = FiresecInternalClient.DeviceCustomFunctionList(driverUID.ToString().ToUpper());

            var operationResult = new OperationResult<List<DeviceCustomFunction>>()
            {
                HasError = result.HasError,
                Error = result.Error
            };

            operationResult.Result = DeviceCustomFunctionConverter.Convert(result.Result);

            return operationResult;
        }

        public OperationResult<string> DeviceCustomFunctionExecute(DeviceConfiguration deviceConfiguration, Guid deviceUID, string functionName)
        {
            ConfigurationConverter.ConvertBack(deviceConfiguration, false);
            var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
            return FiresecInternalClient.DeviceCustomFunctionExecute(ConfigurationConverter.FiresecConfiguration, device.PlaceInTree, functionName).ToOperationResult();
        }

        public OperationResult<string> DeviceGetGuardUsersList(DeviceConfiguration deviceConfiguration, Guid deviceUID)
        {
            ConfigurationConverter.ConvertBack(deviceConfiguration, false);
            var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
            return FiresecInternalClient.DeviceGetGuardUsersList(ConfigurationConverter.FiresecConfiguration, device.PlaceInTree).ToOperationResult();
        }

        public OperationResult<bool> DeviceSetGuardUsersList(DeviceConfiguration deviceConfiguration, Guid deviceUID, string users)
        {
            ConfigurationConverter.ConvertBack(deviceConfiguration, false);
            var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
            return FiresecInternalClient.DeviceSetGuardUsersList(ConfigurationConverter.FiresecConfiguration, device.PlaceInTree, users).ToOperationResult();
        }

        public OperationResult<string> DeviceGetMDS5Data(DeviceConfiguration deviceConfiguration, Guid deviceUID)
        {
            ConfigurationConverter.ConvertBack(deviceConfiguration, false);
            var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
            return FiresecInternalClient.DeviceGetMDS5Data(ConfigurationConverter.FiresecConfiguration, device.PlaceInTree).ToOperationResult();
        }

        public OperationResult<bool> AddToIgnoreList(List<Guid> deviceGuids)
        {
            var devicePaths = new List<string>();
            foreach (var guid in deviceGuids)
            {
                var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == guid);
                devicePaths.Add(device.PlaceInTree);
            }

            return FiresecInternalClient.AddToIgnoreList(devicePaths).ToOperationResult();
        }

        public OperationResult<bool> RemoveFromIgnoreList(List<Guid> deviceGuids)
        {
            var devicePaths = new List<string>();
            foreach (var guid in deviceGuids)
            {
                var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == guid);
                devicePaths.Add(device.PlaceInTree);
            }

            return FiresecInternalClient.RemoveFromIgnoreList(devicePaths).ToOperationResult();
        }

        public OperationResult<bool> ResetStates(List<ResetItem> resetItems)
        {
            return FiresecResetHelper.ResetMany(resetItems).ToOperationResult();
        }

        public OperationResult<bool> AddUserMessage(string message)
        {
            return FiresecInternalClient.AddUserMessage(message).ToOperationResult();
        }

        public OperationResult<bool> ExecuteCommand(Guid deviceUID, string methodName)
        {
            var device = FiresecManager.DeviceConfigurationStates.DeviceStates.FirstOrDefault(x => x.UID == deviceUID);
            if (device != null)
            {
                FiresecInternalClient.ExecuteCommand(device.PlaceInTree, methodName).ToOperationResult();
            }
            var operationResult = new OperationResult<bool>()
            {
                Result = false,
                HasError = true,
                Error = new Exception("Не найдено устройство по идентификатору")
            };
            return operationResult;
        }

        public OperationResult<string> CheckHaspPresence()
        {
            return FiresecInternalClient.CheckHaspPresence().ToOperationResult();
        }
    }
}