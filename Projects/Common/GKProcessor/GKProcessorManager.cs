using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecClient;
using System.Reflection;
using FiresecAPI.Journal;

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
				AddGKMessage(JournalEventNameType.Отмена_операции, JournalEventDescriptionType.NULL, progressCallback.Title, null, userName);
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
				gkCallbackResult.GKStates.GuardZoneStates.Count +
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

		static void SuspendMonitoring(GKDevice gkControllerDevice)
		{
			if (MustMonitor)
			{
				gkControllerDevice = GetGKDevice(gkControllerDevice);
				if (WatcherManager.Watchers != null && gkControllerDevice != null)
				{
					var watcher = WatcherManager.Watchers.FirstOrDefault(x => x.GkDatabase.RootDevice.UID == gkControllerDevice.UID);
					if (watcher != null)
						watcher.Suspend();
				}
			}
		}

		static void ResumeMonitoring(GKDevice gkControllerDevice)
		{
			if (MustMonitor)
			{
				gkControllerDevice = GetGKDevice(gkControllerDevice);
				if (WatcherManager.Watchers != null && gkControllerDevice != null)
				{
					var watcher = WatcherManager.Watchers.FirstOrDefault(x => x.GkDatabase.RootDevice.UID == gkControllerDevice.UID);
					if (watcher != null)
						watcher.Resume();
				}
			}
		}

		static GKDevice GetGKDevice(GKDevice device)
		{
			if (device.DriverType == GKDriverType.GK)
				return device;
			var gkControllerDevice = device.GkDatabaseParent;
			if (gkControllerDevice.DriverType == GKDriverType.GK)
				return gkControllerDevice;
			return null;
		}

		#endregion

		#region Operations
		public static OperationResult<bool> GKWriteConfiguration(GKDevice device, string userName)
		{
			AddGKMessage(JournalEventNameType.Запись_конфигурации_в_прибор, JournalEventDescriptionType.NULL, "", device, userName);
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

		public static OperationResult<GKDeviceConfiguration> GKReadConfiguration(GKDevice device, string userName)
		{
			AddGKMessage(JournalEventNameType.Чтение_конфигурации_из_прибора, JournalEventDescriptionType.NULL, "", device, userName);
			Stop();
			DescriptorsManager.Create();
			var descriptorReader = device.Driver.IsKauOrRSR2Kau ? (DescriptorReaderBase)new KauDescriptorsReaderBase() : new GkDescriptorsReaderBase();
			descriptorReader.ReadConfiguration(device);
			Start();
			return new OperationResult<GKDeviceConfiguration> { HasError = descriptorReader.Error != null, Error = descriptorReader.Error, Result = descriptorReader.DeviceConfiguration };
		}

		public static OperationResult<GKDeviceConfiguration> GKReadConfigurationFromGKFile(GKDevice device, string userName)
		{
			AddGKMessage(JournalEventNameType.Чтение_конфигурации_из_прибора, JournalEventDescriptionType.NULL, "", device, userName);
			SuspendMonitoring(device);
			var gkFileReaderWriter = new GKFileReaderWriter();
			var deviceConfiguration = gkFileReaderWriter.ReadConfigFileFromGK(device);
			ResumeMonitoring(device);
			return new OperationResult<GKDeviceConfiguration> { HasError = gkFileReaderWriter.Error != null, Error = gkFileReaderWriter.Error, Result = deviceConfiguration };
		}

		public static OperationResult<bool> GKUpdateFirmware(GKDevice device, string fileName, string userName)
		{
			Stop();
			var firmwareUpdateHelper = new FirmwareUpdateHelper();
			firmwareUpdateHelper.Update(device, fileName, userName);
			Start();
			if (firmwareUpdateHelper.ErrorList.Count > 0)
				return new OperationResult<bool>(firmwareUpdateHelper.ErrorList.Aggregate((a,b)=> a + "\n" + b)) { Result = false };
			return new OperationResult<bool> { Result = true };
		}

		public static OperationResult<bool> GKUpdateFirmwareFSCS(HexFileCollectionInfo hxcFileInfo, string userName, List<GKDevice> devices)
		{
			Stop();
			var firmwareUpdateHelper = new FirmwareUpdateHelper();
			firmwareUpdateHelper.UpdateFSCS(hxcFileInfo, devices, userName);
			Start();
			if (firmwareUpdateHelper.ErrorList.Count > 0)
				return new OperationResult<bool>(firmwareUpdateHelper.ErrorList.Aggregate((a, b) => a + "\n" + b)) { Result = false };
			return new OperationResult<bool> { Result = true };
		}

		public static bool GKSyncronyseTime(GKDevice device, string userName)
		{
			AddGKMessage(JournalEventNameType.Синхронизация_времени, JournalEventDescriptionType.NULL, "", device, userName);
			return DeviceBytesHelper.WriteDateTime(device);
		}

		public static string GKGetDeviceInfo(GKDevice device, string userName)
		{
			AddGKMessage(JournalEventNameType.Запрос_информации_об_устройстве, JournalEventDescriptionType.NULL, "", device, userName);
			var result = DeviceBytesHelper.GetDeviceInfo(device);
			if (result == null)
				result = "Устройство недоступно";
			return result;
		}

		public static OperationResult<int> GKGetJournalItemsCount(GKDevice device)
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

		public static OperationResult<GKJournalItem> GKReadJournalItem(GKDevice device, int no)
		{
			var data = BitConverter.GetBytes(no).ToList();
			var sendResult = SendManager.Send(device, 4, 7, 64, data);
			if (sendResult.HasError)
			{
				return new OperationResult<GKJournalItem>("Устройство недоступно");
			}
			if (sendResult.Bytes.Count == 64)
			{
				var journalParser = new JournalParser(device, sendResult.Bytes);
				return new OperationResult<GKJournalItem>() { Result = journalParser.JournalItem };
			}
			else
			{
				return new OperationResult<GKJournalItem>("Ошибка. Недостаточное количество байт в записи журнала");
			}
		}

		public static OperationResult<bool> GKSetSingleParameter(GKBase xBase, List<byte> parameterBytes)
		{
			var error = ParametersHelper.SetSingleParameter(xBase, parameterBytes);
			return new OperationResult<bool>() { HasError = error != null, Error = error, Result = true };
		}

		public static OperationResult<List<GKProperty>> GKGetSingleParameter(GKBase xBase)
		{
			return ParametersHelper.GetSingleParameter(xBase);
		}

		public static OperationResult<bool> GKSetSchedule(GKDevice device, GKSchedule schedule)
		{
			var bytes = new List<byte>();
			var sendResult = SendManager.Send(device, (ushort)bytes.Count, 0, 0, bytes);
			return new OperationResult<bool>() { HasError = sendResult.HasError, Error = sendResult.Error, Result = !sendResult.HasError };
		}

		public static OperationResult<GKSchedule> GKGetSchedule(GKDevice device, int no)
		{
			var bytes = new List<byte>();
			var sendResult = SendManager.Send(device, (ushort)bytes.Count, 0, 0, bytes);
			if (!sendResult.HasError)
			{
				var schedule = new GKSchedule();
				return new OperationResult<GKSchedule>() { Result = schedule };
			}
			return new OperationResult<GKSchedule>("Ошибка");
		}

		public static OperationResult<List<byte>> GKGKHash(GKDevice device)
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
			foreach (var device in GKManager.Devices)
			{
				Watcher.AddObjectStateToGKStates(gkStates, device);
			}
			foreach (var zone in GKManager.Zones)
			{
				Watcher.AddObjectStateToGKStates(gkStates, zone);
			}
			foreach (var direction in GKManager.Directions)
			{
				Watcher.AddObjectStateToGKStates(gkStates, direction);
			}
			foreach (var pumpStation in GKManager.PumpStations)
			{
				Watcher.AddObjectStateToGKStates(gkStates, pumpStation);
			}
			foreach (var mpt in GKManager.MPTs)
			{
				Watcher.AddObjectStateToGKStates(gkStates, mpt);
			}
			foreach (var delay in GKManager.Delays)
			{
				Watcher.AddObjectStateToGKStates(gkStates, delay);
			}
			foreach (var delay in GKManager.AutoGeneratedDelays)
			{
				Watcher.AddObjectStateToGKStates(gkStates, delay);
			}
			foreach (var pim in GKManager.AutoGeneratedPims)
			{
				Watcher.AddObjectStateToGKStates(gkStates, pim);
			}
			foreach (var guardZone in GKManager.GuardZones)
			{
				Watcher.AddObjectStateToGKStates(gkStates, guardZone);
			}
			return gkStates;
		}

		public static void GKExecuteDeviceCommand(GKDevice device, GKStateBit stateBit, string userName)
		{
			Watcher.SendControlCommand(device, stateBit, stateBit.ToDescription());
			AddGKMessage(JournalEventNameType.Команда_оператора, JournalEventDescriptionType.NULL, stateBit.ToDescription(), device, userName);
		}

		public static void GKReset(GKBase xBase, string userName)
		{
			Watcher.SendControlCommand(xBase, GKStateBit.Reset, "Сброс");
			AddGKMessage(JournalEventNameType.Команда_оператора, JournalEventDescriptionType.Сброс, "", xBase, userName);
		}

		public static void GKResetFire1(GKZone zone, string userName)
		{
			Watcher.SendControlCommand(zone, 0x02, "Сброс Пожар-1");
			AddGKMessage(JournalEventNameType.Команда_оператора, JournalEventDescriptionType.Сброс, "", zone, userName);
		}

		public static void GKResetFire2(GKZone zone, string userName)
		{
			Watcher.SendControlCommand(zone, 0x03, "Сброс Пожар-1");
			AddGKMessage(JournalEventNameType.Команда_оператора, JournalEventDescriptionType.Сброс, "", zone, userName);
		}

		public static void GKSetAutomaticRegime(GKBase xBase, string userName)
		{
			Watcher.SendControlCommand(xBase, GKStateBit.SetRegime_Automatic, "Перевод в автоматический режим");
			AddGKMessage(JournalEventNameType.Команда_оператора, JournalEventDescriptionType.Перевод_в_автоматический_режим, "", xBase, userName);
		}

		public static void GKSetManualRegime(GKBase xBase, string userName)
		{
			Watcher.SendControlCommand(xBase, GKStateBit.SetRegime_Manual, "Перевод в ручной режим");
			AddGKMessage(JournalEventNameType.Команда_оператора, JournalEventDescriptionType.Перевод_в_ручной_режим, "", xBase, userName);
		}

		public static void GKSetIgnoreRegime(GKBase xBase, string userName)
		{
			Watcher.SendControlCommand(xBase, GKStateBit.SetRegime_Off, "Перевод в отключенный режим");
			AddGKMessage(JournalEventNameType.Команда_оператора, JournalEventDescriptionType.Перевод_в_отключенный_режим, "", xBase, userName);
		}

		public static void GKTurnOn(GKBase xBase, string userName)
		{
			Watcher.SendControlCommand(xBase, GKStateBit.TurnOn_InManual, "Включить");
			AddGKMessage(JournalEventNameType.Команда_оператора, JournalEventDescriptionType.Включить, "", xBase, userName);
		}

		public static void GKTurnOnNow(GKBase xBase, string userName)
		{
			Watcher.SendControlCommand(xBase, GKStateBit.TurnOnNow_InManual, "Включить немедленно");
			AddGKMessage(JournalEventNameType.Команда_оператора, JournalEventDescriptionType.Включить_немедленно, "", xBase, userName);
		}

		public static void GKTurnOff(GKBase xBase, string userName)
		{
			Watcher.SendControlCommand(xBase, GKStateBit.TurnOff_InManual, "Выключить");
			AddGKMessage(JournalEventNameType.Команда_оператора, JournalEventDescriptionType.Выключить, "", xBase, userName);
		}

		public static void GKTurnOffNow(GKBase xBase, string userName)
		{
			Watcher.SendControlCommand(xBase, GKStateBit.TurnOffNow_InManual, "Выключить немедленно");
			AddGKMessage(JournalEventNameType.Команда_оператора, JournalEventDescriptionType.Выключить_немедленно, "", xBase, userName);
		}

		public static void GKStop(GKBase xBase, string userName)
		{
			Watcher.SendControlCommand(xBase, GKStateBit.Stop_InManual, "Остановка пуска");
			AddGKMessage(JournalEventNameType.Команда_оператора, JournalEventDescriptionType.Остановка_пуска, "", xBase, userName);
		}

		public static void GKStartMeasureMonitoring(GKDevice device)
		{
			var watcher = WatcherManager.Watchers.FirstOrDefault(x => x.GkDatabase.RootDevice.UID == device.GkDatabaseParent.UID);
			if (watcher != null)
			{
				watcher.StartDeviceMeasure(device);
			}
		}

		public static void GKStopMeasureMonitoring(GKDevice device)
		{
			var watcher = WatcherManager.Watchers.FirstOrDefault(x => x.GkDatabase.RootDevice.UID == device.GkDatabaseParent.UID);
			if (watcher != null)
			{
				watcher.StopDeviceMeasure(device);
			}
		}

		public static bool GKAddUser(GKDevice device, string userName)
		{
			//AddGKMessage(JournalEventNameType.Синхронизация_времени, "", device, userName, true);
			return DeviceBytesHelper.AddUser(device);
		}

		#endregion

		#region JournalItem Callback
		public static void AddGKMessage(JournalEventNameType journalEventNameType, JournalEventDescriptionType journalEventDescriptionType, string description, GKBase xBase, string userName)
		{
			Guid uid = Guid.Empty;
			var journalObjectType = GKJournalObjectType.System;
			if (xBase != null)
			{
				uid = xBase.UID;
				if (xBase is GKDevice)
				{
					journalObjectType = GKJournalObjectType.Device;
				}
				if (xBase is GKZone)
				{
					journalObjectType = GKJournalObjectType.Zone;
				}
				if (xBase is GKDirection)
				{
					journalObjectType = GKJournalObjectType.Direction;
				}
				if (xBase is GKDelay)
				{
					journalObjectType = GKJournalObjectType.Delay;
				}
				if (xBase is GKPim)
				{
					journalObjectType = GKJournalObjectType.Pim;
				}
				if (xBase is GKGuardZone)
				{
					journalObjectType = GKJournalObjectType.GuardZone;
				}
			}

			var journalItem = new GKJournalItem()
			{
				SystemDateTime = DateTime.Now,
				DeviceDateTime = DateTime.Now,
				JournalObjectType = journalObjectType,
				StateClass = EventDescriptionAttributeHelper.ToStateClass(journalEventNameType),
				JournalEventNameType = journalEventNameType,
				JournalEventDescriptionType = journalEventDescriptionType,
				Name = EventDescriptionAttributeHelper.ToName(journalEventNameType),
				Description = description,
				ObjectUID = uid,
				ObjectStateClass = XStateClass.Norm,
				UserName = userName,
				SubsystemType = GKSubsystemType.System
			};
			if (xBase != null)
			{
				journalItem.ObjectName = xBase.PresentationName;
				journalItem.GKObjectNo = (ushort)xBase.GKDescriptorNo;
			}

			GKDBHelper.Add(journalItem);
			var gkCallbackResult = new GKCallbackResult();
			gkCallbackResult.JournalItems.Add(journalItem);
			OnGKCallbackResult(gkCallbackResult);
		}
		#endregion
	}
}