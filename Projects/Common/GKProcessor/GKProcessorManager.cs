using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecClient;

namespace GKProcessor
{
	public static class GKProcessorManager
	{
		#region Callback
		public static List<GKProgressCallback> GKProgressCallbacks = new List<GKProgressCallback>();

		public static void CancelGKProgress(Guid progressCallbackUID, string userName)
		{
			var progressCallback = GKProgressCallbacks.FirstOrDefault(x => x.UID == progressCallbackUID);
			if (progressCallback != null)
			{
				progressCallback.IsCanceled = true;
				progressCallback.CancelizationDateTime = DateTime.Now;
				StopProgress(progressCallback);
				AddGKMessage(EventNameEnum.Отмена_операции, progressCallback.Title, null, userName, true);
			}
		}

		public static GKProgressCallback StartProgress(string title, string text, int stepCount, bool canCancel, GKProgressClientType progressClientType)
		{
			var gkProgressCallback = new GKProgressCallback()
			{
				GKProgressCallbackType = GKProgressCallbackType.Start,
				Title = title,
				Text = text,
				StepCount = stepCount,
				CanCancel = canCancel,
				GKProgressClientType = progressClientType
			};
			GKProgressCallbacks.Add(gkProgressCallback);
			OnGKCallbackResult(gkProgressCallback);
			return gkProgressCallback;
		}

		public static void DoProgress(string text, GKProgressCallback progressCallback)
		{
			var gkProgressCallback = new GKProgressCallback()
			{
				UID = progressCallback.UID,
				LastActiveDateTime = DateTime.Now,
				GKProgressCallbackType = GKProgressCallbackType.Progress,
				Title = progressCallback.Title,
				Text = text,
				StepCount = progressCallback.StepCount,
				CanCancel = progressCallback.CanCancel,
				GKProgressClientType = progressCallback.GKProgressClientType
			};
			OnGKCallbackResult(gkProgressCallback);
		}

		public static void StopProgress(GKProgressCallback progressCallback)
		{
			var gkProgressCallback = new GKProgressCallback()
			{
				UID = progressCallback.UID,
				LastActiveDateTime = DateTime.Now,
				GKProgressCallbackType = GKProgressCallbackType.Stop,
			};
			GKProgressCallbacks.Remove(gkProgressCallback);
			OnGKCallbackResult(gkProgressCallback);
		}

		static void OnGKCallbackResult(GKProgressCallback gkProgressCallback)
		{
			GKProgressCallbacks.RemoveAll(x => x.IsCanceled && (DateTime.Now - x.CancelizationDateTime).TotalMinutes > 5);
			if (gkProgressCallback.GKProgressCallbackType == GKProgressCallbackType.Stop || !gkProgressCallback.IsCanceled)
			{
				if (GKProgressCallbackEvent != null)
					GKProgressCallbackEvent(gkProgressCallback);
			}
		}
		public static event Action<GKProgressCallback> GKProgressCallbackEvent;

		public static void OnGKCallbackResult(GKCallbackResult gkCallbackResult)
		{
			if (gkCallbackResult.JournalItems.Count +
				gkCallbackResult.GKStates.DeviceStates.Count +
				gkCallbackResult.GKStates.ZoneStates.Count +
				gkCallbackResult.GKStates.DirectionStates.Count +
				gkCallbackResult.GKStates.PumpStationStates.Count +
				gkCallbackResult.GKStates.MPTStates.Count +
				gkCallbackResult.GKStates.DelayStates.Count +
				gkCallbackResult.GKStates.PimStates.Count +
				gkCallbackResult .GKStates.DeviceMeasureParameters.Count > 0)
			{
				if (GKCallbackResultEvent != null)
					GKCallbackResultEvent(gkCallbackResult);
			}
		}
		public static event Action<GKCallbackResult> GKCallbackResultEvent;
		#endregion

		#region Main
		public static bool MustMonitor = false;

		public static void Start()
		{
			if (MustMonitor)
			{
				WatcherManager.IsConfigurationReloading = false;
				WatcherManager.Start();
			}
		}

		public static void Stop()
		{
			if (MustMonitor)
			{
				WatcherManager.Stop();
			}
		}

		public static void Suspend()
		{
			WatcherManager.LastConfigurationReloadingTime = DateTime.Now;
			WatcherManager.IsConfigurationReloading = true;
		}

		static void SuspendMonitoring(XDevice gkDevice)
		{
			if (MustMonitor)
			{
				gkDevice = GetGKDevice(gkDevice);
				if (WatcherManager.Watchers != null && gkDevice != null)
				{
					var watcher = WatcherManager.Watchers.FirstOrDefault(x => x.GkDatabase.RootDevice.BaseUID == gkDevice.BaseUID);
					if (watcher != null)
						watcher.Suspend();
				}
			}
		}

		static void ResumeMonitoring(XDevice gkDevice)
		{
			if (MustMonitor)
			{
				gkDevice = GetGKDevice(gkDevice);
				if (WatcherManager.Watchers != null && gkDevice != null)
				{
					var watcher = WatcherManager.Watchers.FirstOrDefault(x => x.GkDatabase.RootDevice.BaseUID == gkDevice.BaseUID);
					if (watcher != null)
						watcher.Resume();
				}
			}
		}

		static XDevice GetGKDevice(XDevice device)
		{
			if (device.DriverType == XDriverType.GK)
				return device;
			var gkDevice = device.GkDatabaseParent;
			if (gkDevice.DriverType == XDriverType.GK)
				return gkDevice;
			return null;
		}

		#endregion

		#region Operations
		public static OperationResult<bool> GKWriteConfiguration(XDevice device, string userName)
		{
			AddGKMessage(EventNameEnum.Запись_конфигурации_в_прибор, "", device, userName, true);
			Stop();
			var gkDescriptorsWriter = new GkDescriptorsWriter();
			gkDescriptorsWriter.WriteConfig(device);
			Start();
			if (gkDescriptorsWriter.Errors.Count > 0)
			{
				var errors = new StringBuilder();
				foreach (var error in gkDescriptorsWriter.Errors)
				{
					errors.AppendLine(error);
				}
				return new OperationResult<bool>(errors.ToString()) { Result = false };
			}
			return new OperationResult<bool>() { Result = true };
		}

		public static OperationResult<XDeviceConfiguration> GKReadConfiguration(XDevice device, string userName)
		{
			AddGKMessage(EventNameEnum.Чтение_конфигурации_из_прибора, "", device, userName, true);
			Stop();
			DescriptorsManager.Create();
			var descriptorReader = device.Driver.IsKauOrRSR2Kau ? (DescriptorReaderBase)new KauDescriptorsReaderBase() : new GkDescriptorsReaderBase();
			descriptorReader.ReadConfiguration(device);
			Start();
			return new OperationResult<XDeviceConfiguration> { HasError = descriptorReader.Error != null, Error = descriptorReader.Error, Result = descriptorReader.DeviceConfiguration };
		}

		public static OperationResult<XDeviceConfiguration> GKReadConfigurationFromGKFile(XDevice device, string userName)
		{
			AddGKMessage(EventNameEnum.Чтение_конфигурации_из_прибора, "", device, userName, true);
			SuspendMonitoring(device);
			var gkFileReaderWriter = new GKFileReaderWriter();
			var deviceConfiguration = gkFileReaderWriter.ReadConfigFileFromGK(device);
			ResumeMonitoring(device);
			return new OperationResult<XDeviceConfiguration> { HasError = gkFileReaderWriter.Error != null, Error = gkFileReaderWriter.Error, Result = deviceConfiguration };
		}

		public static OperationResult<bool> GKUpdateFirmware(XDevice device, string fileName, string userName)
		{
			Stop();
			var firmwareUpdateHelper = new FirmwareUpdateHelper();
			firmwareUpdateHelper.Update(device, fileName, userName);
			Start();
			if (firmwareUpdateHelper.ErrorList.Count > 0)
				return new OperationResult<bool>(firmwareUpdateHelper.ErrorList.Aggregate((a,b)=> a + "\n" + b)) { Result = false };
			return new OperationResult<bool> { Result = true };
		}

		public static OperationResult<bool> GKUpdateFirmwareFSCS(HexFileCollectionInfo hxcFileInfo, string userName, List<XDevice> devices)
		{
			Stop();
			var firmwareUpdateHelper = new FirmwareUpdateHelper();
			firmwareUpdateHelper.UpdateFSCS(hxcFileInfo, devices, userName);
			Start();
			if (firmwareUpdateHelper.ErrorList.Count > 0)
				return new OperationResult<bool>(firmwareUpdateHelper.ErrorList.Aggregate((a, b) => a + "\n" + b)) { Result = false };
			return new OperationResult<bool> { Result = true };
		}

		public static bool GKSyncronyseTime(XDevice device, string userName)
		{
			AddGKMessage(EventNameEnum.Синхронизация_времени, "", device, userName, true);
			return DeviceBytesHelper.WriteDateTime(device);
		}

		public static string GKGetDeviceInfo(XDevice device, string userName)
		{
			AddGKMessage(EventNameEnum.Запрос_информации_об_устройстве, "", device, userName, true);
			var result = DeviceBytesHelper.GetDeviceInfo(device);
			if (result == null)
				result = "Устройство недоступно";
			return result;
		}

		public static OperationResult<int> GKGetJournalItemsCount(XDevice device)
		{
			var sendResult = SendManager.Send(device, 0, 6, 64);
			if (sendResult.HasError)
			{
				return new OperationResult<int>("Устройство недоступно");
			}
			var journalParser = new JournalParser(device, sendResult.Bytes);
			var result = journalParser.JournalItem.GKJournalRecordNo.Value;
			return new OperationResult<int>() { Result = result };
		}

		public static OperationResult<JournalItem> GKReadJournalItem(XDevice device, int no)
		{
			var data = BitConverter.GetBytes(no).ToList();
			var sendResult = SendManager.Send(device, 4, 7, 64, data);
			if (sendResult.HasError)
			{
				return new OperationResult<JournalItem>("Устройство недоступно");
			}
			if (sendResult.Bytes.Count == 64)
			{
				var journalParser = new JournalParser(device, sendResult.Bytes);
				return new OperationResult<JournalItem>() { Result = journalParser.JournalItem };
			}
			else
			{
				return new OperationResult<JournalItem>("Ошибка. Недостаточное количество байт в записи журнала");
			}
		}

		public static OperationResult<bool> GKSetSingleParameter(XBase xBase, List<byte> parameterBytes)
		{
			var error = ParametersHelper.SetSingleParameter(xBase, parameterBytes);
			return new OperationResult<bool>() { HasError = error != null, Error = error, Result = true };
		}

		public static OperationResult<List<XProperty>> GKGetSingleParameter(XBase xBase)
		{
			return ParametersHelper.GetSingleParameter(xBase);
		}

		public static OperationResult<List<byte>> GKGKHash(XDevice device)
		{
			var gkFileReaderWriter = new GKFileReaderWriter();
			var readInfoBlock = gkFileReaderWriter.ReadInfoBlock(device);
			if (gkFileReaderWriter.Error != null)
				return new OperationResult<List<byte>>(gkFileReaderWriter.Error);
			return new OperationResult<List<byte>>() { Result = readInfoBlock.Hash1 };
		}

		public static GKStates GKGetStates()
		{
			var gkStates = new GKStates();
			foreach (var device in XManager.Devices)
			{
				Watcher.AddObjectStateToGKStates(gkStates, device);
			}
			foreach (var zone in XManager.Zones)
			{
				Watcher.AddObjectStateToGKStates(gkStates, zone);
			}
			foreach (var direction in XManager.Directions)
			{
				Watcher.AddObjectStateToGKStates(gkStates, direction);
			}
			foreach (var pumpStation in XManager.PumpStations)
			{
				Watcher.AddObjectStateToGKStates(gkStates, pumpStation);
			}
			foreach (var mpt in XManager.MPTs)
			{
				Watcher.AddObjectStateToGKStates(gkStates, mpt);
			}
			foreach (var delay in XManager.Delays)
			{
				Watcher.AddObjectStateToGKStates(gkStates, delay);
			}
			foreach (var delay in XManager.AutoGeneratedDelays)
			{
				Watcher.AddObjectStateToGKStates(gkStates, delay);
			}
			foreach (var pim in XManager.AutoGeneratedPims)
			{
				Watcher.AddObjectStateToGKStates(gkStates, pim);
			}
			return gkStates;
		}

		public static void GKExecuteDeviceCommand(XDevice device, XStateBit stateBit, string userName)
		{
			Watcher.SendControlCommand(device, stateBit, stateBit.ToDescription());
			AddGKMessage(EventNameEnum.Команда_оператора, stateBit.ToDescription(), device, userName);
		}

		public static void GKReset(XBase xBase, string userName)
		{
			Watcher.SendControlCommand(xBase, XStateBit.Reset, "Сброс");
			AddGKMessage(EventNameEnum.Команда_оператора, EventDescription.Сброс, xBase, userName);
		}

		public static void GKResetFire1(XZone zone, string userName)
		{
			Watcher.SendControlCommand(zone, 0x02, "Сброс Пожар-1");
			AddGKMessage(EventNameEnum.Команда_оператора, EventDescription.Сброс, zone, userName);
		}

		public static void GKResetFire2(XZone zone, string userName)
		{
			Watcher.SendControlCommand(zone, 0x03, "Сброс Пожар-1");
			AddGKMessage(EventNameEnum.Команда_оператора, EventDescription.Сброс, zone, userName);
		}

		public static void GKSetAutomaticRegime(XBase xBase, string userName)
		{
			Watcher.SendControlCommand(xBase, XStateBit.SetRegime_Automatic, "Перевод в автоматический режим");
			AddGKMessage(EventNameEnum.Команда_оператора, EventDescription.Перевод_в_автоматический_режим, xBase, userName);
		}

		public static void GKSetManualRegime(XBase xBase, string userName)
		{
			Watcher.SendControlCommand(xBase, XStateBit.SetRegime_Manual, "Перевод в ручной режим");
			AddGKMessage(EventNameEnum.Команда_оператора, EventDescription.Перевод_в_ручной_режим, xBase, userName);
		}

		public static void GKSetIgnoreRegime(XBase xBase, string userName)
		{
			Watcher.SendControlCommand(xBase, XStateBit.SetRegime_Off, "Перевод в отключенный режим");
			AddGKMessage(EventNameEnum.Команда_оператора, EventDescription.Перевод_в_отключенный_режим, xBase, userName);
		}

		public static void GKTurnOn(XBase xBase, string userName)
		{
			Watcher.SendControlCommand(xBase, XStateBit.TurnOn_InManual, "Включить");
			AddGKMessage(EventNameEnum.Команда_оператора, EventDescription.Включить, xBase, userName);
		}

		public static void GKTurnOnNow(XBase xBase, string userName)
		{
			Watcher.SendControlCommand(xBase, XStateBit.TurnOnNow_InManual, "Включить немедленно");
			AddGKMessage(EventNameEnum.Команда_оператора, EventDescription.Включить_немедленно, xBase, userName);
		}

		public static void GKTurnOff(XBase xBase, string userName)
		{
			Watcher.SendControlCommand(xBase, XStateBit.TurnOff_InManual, "Выключить");
			AddGKMessage(EventNameEnum.Команда_оператора, EventDescription.Выключить, xBase, userName);
		}

		public static void GKTurnOffNow(XBase xBase, string userName)
		{
			Watcher.SendControlCommand(xBase, XStateBit.TurnOffNow_InManual, "Выключить немедленно");
			AddGKMessage(EventNameEnum.Команда_оператора, EventDescription.Выключить_немедленно, xBase, userName);
		}

		public static void GKStop(XBase xBase, string userName)
		{
			Watcher.SendControlCommand(xBase, XStateBit.Stop_InManual, "Остановка пуска");
			AddGKMessage(EventNameEnum.Команда_оператора, EventDescription.Остановка_пуска, xBase, userName);
		}

		public static void GKStartMeasureMonitoring(XDevice device)
		{
			var watcher = WatcherManager.Watchers.FirstOrDefault(x => x.GkDatabase.RootDevice.BaseUID == device.GkDatabaseParent.BaseUID);
			if (watcher != null)
			{
				watcher.StartDeviceMeasure(device);
			}
		}

		public static void GKStopMeasureMonitoring(XDevice device)
		{
			var watcher = WatcherManager.Watchers.FirstOrDefault(x => x.GkDatabase.RootDevice.BaseUID == device.GkDatabaseParent.BaseUID);
			if (watcher != null)
			{
				watcher.StopDeviceMeasure(device);
			}
		}

		#endregion

		#region JournalItem Callback
		public static void AddGKMessage(EventNameEnum message, EventDescription description, XBase xBase, string userName, bool isAdministrator = false)
		{
			AddGKMessage(message, description.ToDescription(), xBase, userName, isAdministrator);
		}

		public static void AddGKMessage(EventNameEnum message, string description, XBase xBase, string userName, bool isAdministrator = false)
		{
			Guid uid = Guid.Empty;
			var journalItemType = JournalItemType.System;
			if (xBase != null)
			{
				uid = xBase.BaseUID;
				if (xBase is XDevice)
				{
					journalItemType = JournalItemType.Device;
				}
				if (xBase is XZone)
				{
					journalItemType = JournalItemType.Zone;
				}
				if (xBase is XDirection)
				{
					journalItemType = JournalItemType.Direction;
				}
				if (xBase is XDelay)
				{
					journalItemType = JournalItemType.Delay;
				}
				if (xBase is XPim)
				{
					journalItemType = JournalItemType.Pim;
				}
			}

			var journalItem = new JournalItem()
			{
				SystemDateTime = DateTime.Now,
				DeviceDateTime = DateTime.Now,
				JournalItemType = journalItemType,
				StateClass = EventNamesHelper.GetStateClass(message),
				Name = message.ToDescription(),
				Description = description,
				ObjectUID = uid,
				ObjectStateClass = XStateClass.Norm,
				UserName = userName,
				SubsystemType = XSubsystemType.System
			};
			if (xBase != null)
			{
				journalItem.ObjectName = xBase.PresentationName;
				journalItem.GKObjectNo = (ushort)xBase.GKDescriptorNo;
			}

			GKDBHelper.Add(journalItem);
			OnNewJournalItem(journalItem, isAdministrator);
			var gkCallbackResult = new GKCallbackResult();
			gkCallbackResult.JournalItems.Add(journalItem);
			OnGKCallbackResult(gkCallbackResult);
		}

		public static event Action<JournalItem, bool> NewJournalItem;
		static void OnNewJournalItem(JournalItem journalItem, bool isAdministrator)
		{
			if (NewJournalItem != null)
				NewJournalItem(journalItem, isAdministrator);
		}
		#endregion
	}
}