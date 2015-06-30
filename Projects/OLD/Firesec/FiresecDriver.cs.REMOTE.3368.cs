using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Models;
using System.Text;
using Common;
using System.Threading;

namespace Firesec
{
	public partial class FiresecDriver
	{
		NativeFiresecClient NativeFiresecClient { get; set; }
		public FiresecSerializedClient FiresecSerializedClient { get; private set; }
		public ConfigurationConverter ConfigurationConverter { get; private set; }
		public Watcher Watcher { get; private set; }
		public static StringBuilder LoadingErrors { get; private set; }

		public FiresecDriver(int lastJournalNo, string FS_Address, int FS_Port, string FS_Login, string FS_Password, bool isPing)
		{
			LoadingErrors = new StringBuilder();
			try
			{
				FiresecSerializedClient = new FiresecSerializedClient();
				FiresecSerializedClient.Connect(FS_Address, FS_Port, FS_Login, FS_Password, isPing);
				ConfigurationConverter = new ConfigurationConverter()
				{
					FiresecSerializedClient = FiresecSerializedClient
				};
				ConfigurationConverter.ConvertMetadataFromFiresec();
			}
			catch (Exception e)
			{
				Logger.Error(e, "FiresecDriver.Synchronyze");
				FiresecDriver.LoadingErrors.Append(e.Message);
			}
		}

		public void Synchronyze()
		{
			try
			{
				ConfigurationConverter.SynchronyzeConfiguration();
			}
			catch (Exception e)
			{
				Logger.Error(e, "FiresecDriver.Synchronyze");
				FiresecDriver.LoadingErrors.Append(e.Message);
			}
		}

		public void StartWatcher(bool mustMonitorStates, bool mustMonitorJournal)
		{
			try
			{
				Watcher = new Watcher(FiresecSerializedClient, mustMonitorStates, mustMonitorJournal);
				if (mustMonitorStates)
				{
					Watcher.OnStateChanged();
					//Watcher.OnParametersChanged();
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "FiresecDriver.Synchronyze");
				FiresecDriver.LoadingErrors.Append(e.Message);
			}
		}

		public void Convert()
		{
			try
			{
				ConfigurationConverter.Convert();
			}
			catch (Exception e)
			{
				Logger.Error(e, "FiresecDriver.Synchronyze");
				FiresecDriver.LoadingErrors.Append(e.Message);
			}
		}

		public Firesec.Models.CoreConfiguration.config ConvertBack(DeviceConfiguration deviceConfiguration, bool includeSecurity)
		{
			ConfigurationConverter.ConvertBack(deviceConfiguration, includeSecurity);
			return ConfigurationConverter.FiresecConfiguration;
		}

		public OperationResult<bool> SetNewConfig(DeviceConfiguration deviceConfiguration)
		{
			var firesecConfiguration = ConvertBack(deviceConfiguration, true);
			return FiresecSerializedClient.SetNewConfig(firesecConfiguration);
		}

		public OperationResult<bool> DeviceWriteConfiguration(DeviceConfiguration deviceConfiguration, Guid deviceUID)
		{
			var firesecConfiguration = ConvertBack(deviceConfiguration, false);
			var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
			return FiresecSerializedClient.DeviceWriteConfig(firesecConfiguration, device.GetPlaceInTree());
		}

		public OperationResult<bool> DeviceSetPassword(DeviceConfiguration deviceConfiguration, Guid deviceUID, DevicePasswordType devicePasswordType, string password)
		{
			var firesecConfiguration = ConvertBack(deviceConfiguration, false);
			var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
			return FiresecSerializedClient.DeviceSetPassword(firesecConfiguration, device.GetPlaceInTree(), password, (int)devicePasswordType);
		}

		public OperationResult<bool> DeviceDatetimeSync(DeviceConfiguration deviceConfiguration, Guid deviceUID)
		{
			var firesecConfiguration = ConvertBack(deviceConfiguration, false);
			var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
			return FiresecSerializedClient.DeviceDatetimeSync(firesecConfiguration, device.GetPlaceInTree());
		}

		public OperationResult<string> DeviceGetInformation(DeviceConfiguration deviceConfiguration, Guid deviceUID)
		{
			var firesecConfiguration = ConvertBack(deviceConfiguration, false);
			var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
			return FiresecSerializedClient.DeviceGetInformation(firesecConfiguration, device.GetPlaceInTree());
		}

		public OperationResult<List<string>> DeviceGetSerialList(DeviceConfiguration deviceConfiguration, Guid deviceUID)
		{
			var firesecConfiguration = ConvertBack(deviceConfiguration, false);
			var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
			var result = FiresecSerializedClient.DeviceGetSerialList(firesecConfiguration, device.GetPlaceInTree());

			var operationResult = new OperationResult<List<string>>()
			{
				HasError = result.HasError,
				Error = result.Error
			};
			if (result.Result != null)
				operationResult.Result = result.Result.Split(';').ToList();
			return operationResult;
		}

		public OperationResult<string> DeviceUpdateFirmware(DeviceConfiguration deviceConfiguration, Guid deviceUID, string fileName)
		{
			var firesecConfiguration = ConvertBack(deviceConfiguration, false);
			var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
			return FiresecSerializedClient.DeviceUpdateFirmware(firesecConfiguration, device.GetPlaceInTree(), fileName);
		}

		public OperationResult<string> DeviceVerifyFirmwareVersion(DeviceConfiguration deviceConfiguration, Guid deviceUID, string fileName)
		{
			var firesecConfiguration = ConvertBack(deviceConfiguration, false);
			var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
			return FiresecSerializedClient.DeviceVerifyFirmwareVersion(firesecConfiguration, device.GetPlaceInTree(), fileName);
		}

		public OperationResult<string> DeviceReadEventLog(DeviceConfiguration deviceConfiguration, Guid deviceUID)
		{
			var firesecConfiguration = ConvertBack(deviceConfiguration, false);
			var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
			return FiresecSerializedClient.DeviceReadEventLog(firesecConfiguration, device.GetPlaceInTree());
		}

		public OperationResult<DeviceConfiguration> DeviceAutoDetectChildren(DeviceConfiguration deviceConfiguration, Guid deviceUID, bool fastSearch)
		{
			var firesecConfiguration = ConvertBack(deviceConfiguration, false);
			var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
			var result = FiresecSerializedClient.DeviceAutoDetectChildren(firesecConfiguration, device.GetPlaceInTree(), fastSearch);

			var operationResult = new OperationResult<DeviceConfiguration>()
			{
				HasError = result.HasError,
				Error = result.Error
			};
			if (operationResult.HasError)
				return operationResult;

			if (result.Result == null)
				return new OperationResult<DeviceConfiguration>("Ошибка. Получена пустая конфигурация");

			var configurationManager = new ConfigurationConverter();
			operationResult.Result = configurationManager.ConvertOnlyDevices(result.Result);
			return operationResult;
		}

		public OperationResult<DeviceConfiguration> DeviceReadConfiguration(DeviceConfiguration deviceConfiguration, Guid deviceUID)
		{
			var firesecConfiguration = ConvertBack(deviceConfiguration, false);
			var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
			var result = FiresecSerializedClient.DeviceReadConfig(firesecConfiguration, device.GetPlaceInTree());

			var operationResult = new OperationResult<DeviceConfiguration>()
			{
				HasError = result.HasError,
				Error = result.Error
			};
			if (operationResult.HasError)
				return operationResult;

			if (result.Result == null)
				return new OperationResult<DeviceConfiguration>("Ошибка. Получена пустая конфигурация");

			var configurationManager = new ConfigurationConverter();
			operationResult.Result = configurationManager.ConvertOnlyDevices(result.Result);
			return operationResult;
		}

		public OperationResult<List<DeviceCustomFunction>> DeviceCustomFunctionList(Guid driverUID)
		{
			var result = FiresecSerializedClient.DeviceCustomFunctionList(driverUID.ToString().ToUpper());

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
			var firesecConfiguration = ConvertBack(deviceConfiguration, false);
			var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
			return FiresecSerializedClient.DeviceCustomFunctionExecute(firesecConfiguration, device.GetPlaceInTree(), functionName);
		}

		public OperationResult<string> DeviceGetGuardUsersList(DeviceConfiguration deviceConfiguration, Guid deviceUID)
		{
			var firesecConfiguration = ConvertBack(deviceConfiguration, false);
			var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
			return FiresecSerializedClient.DeviceGetGuardUsersList(firesecConfiguration, device.GetPlaceInTree());
		}

		public OperationResult<bool> DeviceSetGuardUsersList(DeviceConfiguration deviceConfiguration, Guid deviceUID, string users)
		{
			var firesecConfiguration = ConvertBack(deviceConfiguration, false);
			var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
			return FiresecSerializedClient.DeviceSetGuardUsersList(firesecConfiguration, device.GetPlaceInTree(), users);
		}

		public OperationResult<string> DeviceGetMDS5Data(DeviceConfiguration deviceConfiguration, Guid deviceUID)
		{
			var firesecConfiguration = ConvertBack(deviceConfiguration, false);
			var device = deviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
			return FiresecSerializedClient.DeviceGetMDS5Data(firesecConfiguration, device.GetPlaceInTree());
		}

		public void AddToIgnoreList(List<Device> devices)
		{
			var devicePaths = new List<string>();
			foreach (var device in devices)
			{
				devicePaths.Add(device.PlaceInTree);
			}

            //var thread = new Thread(new ThreadStart(() =>
            //{
				FiresecSerializedClient.AddToIgnoreList(devicePaths);
            //}));
            //thread.Start();
		}

		public void RemoveFromIgnoreList(List<Device> devices)
		{
			var devicePaths = new List<string>();
			foreach (var device in devices)
			{
				devicePaths.Add(device.PlaceInTree);
			}

            //var thread = new Thread(new ThreadStart(() =>
            //{
				FiresecSerializedClient.RemoveFromIgnoreList(devicePaths);
            //}));
            //thread.Start();
		}

		public void SetZoneGuard(Guid secPanelUID, int localZoneNo)
		{
            //var thread = new Thread(new ThreadStart(() =>
            //    {
					var device = ConfigurationCash.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == secPanelUID);
					if (device != null)
					{
                        FiresecSerializedClient.SetZoneGuard(device.PlaceInTree, localZoneNo.ToString());
					}
            //    }));
            //thread.Start();
		}

		public void UnSetZoneGuard(Guid secPanelUID, int localZoneNo)
		{
            //var thread = new Thread(new ThreadStart(() =>
            //{
				var device = ConfigurationCash.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == secPanelUID);
				if (device != null)
				{
                    //int reguestId = 0;
                    //FiresecSerializedClient.ExecuteRuntimeDeviceMethod(device.PlaceInTree, "UnSetZoneFromGuard", localZoneNo.ToString(), ref reguestId);
                    FiresecSerializedClient.UnSetZoneGuard(device.PlaceInTree, localZoneNo.ToString());
				}
            //}));
            //thread.Start();
		}

		public void AddUserMessage(string message)
		{
			FiresecSerializedClient.AddUserMessage(message);
		}

		public OperationResult<bool> ExecuteCommand(Guid deviceUID, string methodName)
		{
			var device = ConfigurationCash.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				FiresecSerializedClient.ExecuteCommand(device.PlaceInTree, methodName);
			}
			var operationResult = new OperationResult<bool>()
			{
				Result = false,
				HasError = true,
				Error = "Не найдено устройство по идентификатору"
			};
			return operationResult;
		}

		public OperationResult<bool> CheckHaspPresence()
		{
			return FiresecSerializedClient.CheckHaspPresence();
		}

		public List<JournalRecord> ConvertJournal()
		{
			var journalRecords = new List<JournalRecord>();

			var document = FiresecSerializedClient.ReadEvents(0, 10000).Result;
			if (document == null || document.Journal == null || document.Journal.Count() == 0)
				return journalRecords;

			foreach (var innerJournalItem in document.Journal)
			{
				var journalRecord = JournalConverter.Convert(innerJournalItem);
				journalRecords.Add(journalRecord);
			}
			return journalRecords;
		}
	}
}