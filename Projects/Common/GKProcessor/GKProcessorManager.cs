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
				AddGKMessage(JournalEventNameType.Отмена_операции, progressCallback.Title, null, userName, true);
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

		static void SuspendMonitoring(XDevice gkDevice)
		{
			if (MustMonitor)
			{
				gkDevice = GetGKDevice(gkDevice);
				if (WatcherManager.Watchers != null && gkDevice != null)
				{
					var watcher = WatcherManager.Watchers.FirstOrDefault(x => x.GkDatabase.RootDevice.UID == gkDevice.UID);
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
					var watcher = WatcherManager.Watchers.FirstOrDefault(x => x.GkDatabase.RootDevice.UID == gkDevice.UID);
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
			AddGKMessage(JournalEventNameType.Запись_конфигурации_в_прибор, "", device, userName, true);
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
			AddGKMessage(JournalEventNameType.Чтение_конфигурации_из_прибора, "", device, userName, true);
			Stop();
			DescriptorsManager.Create();
			var descriptorReader = device.Driver.IsKauOrRSR2Kau ? (DescriptorReaderBase)new KauDescriptorsReaderBase() : new GkDescriptorsReaderBase();
			descriptorReader.ReadConfiguration(device);
			Start();
			return new OperationResult<XDeviceConfiguration> { HasError = descriptorReader.Error != null, Error = descriptorReader.Error, Result = descriptorReader.DeviceConfiguration };
		}

		public static OperationResult<XDeviceConfiguration> GKReadConfigurationFromGKFile(XDevice device, string userName)
		{
			AddGKMessage(JournalEventNameType.Чтение_конфигурации_из_прибора, "", device, userName, true);
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
			AddGKMessage(JournalEventNameType.Синхронизация_времени, "", device, userName, true);
			return DeviceBytesHelper.WriteDateTime(device);
		}

		public static string GKGetDeviceInfo(XDevice device, string userName)
		{
			AddGKMessage(JournalEventNameType.Запрос_информации_об_устройстве, "", device, userName, true);
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

		public static OperationResult<XJournalItem> GKReadJournalItem(XDevice device, int no)
		{
			var data = BitConverter.GetBytes(no).ToList();
			var sendResult = SendManager.Send(device, 4, 7, 64, data);
			if (sendResult.HasError)
			{
				return new OperationResult<XJournalItem>("Устройство недоступно");
			}
			if (sendResult.Bytes.Count == 64)
			{
				var journalParser = new JournalParser(device, sendResult.Bytes);
				return new OperationResult<XJournalItem>() { Result = journalParser.JournalItem };
			}
			else
			{
				return new OperationResult<XJournalItem>("Ошибка. Недостаточное количество байт в записи журнала");
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
			foreach (var guardZone in XManager.GuardZones)
			{
				Watcher.AddObjectStateToGKStates(gkStates, guardZone);
			}
			return gkStates;
		}

		public static void GKExecuteDeviceCommand(XDevice device, XStateBit stateBit, string userName)
		{
			Watcher.SendControlCommand(device, stateBit, stateBit.ToDescription());
			AddGKMessage(JournalEventNameType.Команда_оператора, stateBit.ToDescription(), device, userName);
		}

		public static void GKReset(XBase xBase, string userName)
		{
			Watcher.SendControlCommand(xBase, XStateBit.Reset, "Сброс");
			AddGKMessage(JournalEventNameType.Команда_оператора, JournalEventDescriptionType.Сброс, xBase, userName);
		}

		public static void GKResetFire1(XZone zone, string userName)
		{
			Watcher.SendControlCommand(zone, 0x02, "Сброс Пожар-1");
			AddGKMessage(JournalEventNameType.Команда_оператора, JournalEventDescriptionType.Сброс, zone, userName);
		}

		public static void GKResetFire2(XZone zone, string userName)
		{
			Watcher.SendControlCommand(zone, 0x03, "Сброс Пожар-1");
			AddGKMessage(JournalEventNameType.Команда_оператора, JournalEventDescriptionType.Сброс, zone, userName);
		}

		public static void GKSetAutomaticRegime(XBase xBase, string userName)
		{
			Watcher.SendControlCommand(xBase, XStateBit.SetRegime_Automatic, "Перевод в автоматический режим");
			AddGKMessage(JournalEventNameType.Команда_оператора, JournalEventDescriptionType.Перевод_в_автоматический_режим, xBase, userName);
		}

		public static void GKSetManualRegime(XBase xBase, string userName)
		{
			Watcher.SendControlCommand(xBase, XStateBit.SetRegime_Manual, "Перевод в ручной режим");
			AddGKMessage(JournalEventNameType.Команда_оператора, JournalEventDescriptionType.Перевод_в_ручной_режим, xBase, userName);
		}

		public static void GKSetIgnoreRegime(XBase xBase, string userName)
		{
			Watcher.SendControlCommand(xBase, XStateBit.SetRegime_Off, "Перевод в отключенный режим");
			AddGKMessage(JournalEventNameType.Команда_оператора, JournalEventDescriptionType.Перевод_в_отключенный_режим, xBase, userName);
		}

		public static void GKTurnOn(XBase xBase, string userName)
		{
			Watcher.SendControlCommand(xBase, XStateBit.TurnOn_InManual, "Включить");
			AddGKMessage(JournalEventNameType.Команда_оператора, JournalEventDescriptionType.Включить, xBase, userName);
		}

		public static void GKTurnOnNow(XBase xBase, string userName)
		{
			Watcher.SendControlCommand(xBase, XStateBit.TurnOnNow_InManual, "Включить немедленно");
			AddGKMessage(JournalEventNameType.Команда_оператора, JournalEventDescriptionType.Включить_немедленно, xBase, userName);
		}

		public static void GKTurnOff(XBase xBase, string userName)
		{
			Watcher.SendControlCommand(xBase, XStateBit.TurnOff_InManual, "Выключить");
			AddGKMessage(JournalEventNameType.Команда_оператора, JournalEventDescriptionType.Выключить, xBase, userName);
		}

		public static void GKTurnOffNow(XBase xBase, string userName)
		{
			Watcher.SendControlCommand(xBase, XStateBit.TurnOffNow_InManual, "Выключить немедленно");
			AddGKMessage(JournalEventNameType.Команда_оператора, JournalEventDescriptionType.Выключить_немедленно, xBase, userName);
		}

		public static void GKStop(XBase xBase, string userName)
		{
			Watcher.SendControlCommand(xBase, XStateBit.Stop_InManual, "Остановка пуска");
			AddGKMessage(JournalEventNameType.Команда_оператора, JournalEventDescriptionType.Остановка_пуска, xBase, userName);
		}

		public static void GKStartMeasureMonitoring(XDevice device)
		{
			var watcher = WatcherManager.Watchers.FirstOrDefault(x => x.GkDatabase.RootDevice.UID == device.GkDatabaseParent.UID);
			if (watcher != null)
			{
				watcher.StartDeviceMeasure(device);
			}
		}

		public static void GKStopMeasureMonitoring(XDevice device)
		{
			var watcher = WatcherManager.Watchers.FirstOrDefault(x => x.GkDatabase.RootDevice.UID == device.GkDatabaseParent.UID);
			if (watcher != null)
			{
				watcher.StopDeviceMeasure(device);
			}
		}

		public static bool GKAddUser(XDevice device, string userName)
		{
			//AddGKMessage(JournalEventNameType.Синхронизация_времени, "", device, userName, true);
			return DeviceBytesHelper.AddUser(device);
		}

		#endregion

		#region JournalItem Callback
		public static void AddGKMessage(JournalEventNameType journalEventNameType, JournalEventDescriptionType description, XBase xBase, string userName, bool isAdministrator = false)
		{
			AddGKMessage(journalEventNameType, description.ToDescription(), xBase, userName, isAdministrator);
		}

		public static void AddGKMessage(JournalEventNameType journalEventNameType, string description, XBase xBase, string userName, bool isAdministrator = false)
		{
			Guid uid = Guid.Empty;
			var journalObjectType = XJournalObjectType.System;
			if (xBase != null)
			{
				uid = xBase.UID;
				if (xBase is XDevice)
				{
					journalObjectType = XJournalObjectType.Device;
				}
				if (xBase is XZone)
				{
					journalObjectType = XJournalObjectType.Zone;
				}
				if (xBase is XDirection)
				{
					journalObjectType = XJournalObjectType.Direction;
				}
				if (xBase is XDelay)
				{
					journalObjectType = XJournalObjectType.Delay;
				}
				if (xBase is XPim)
				{
					journalObjectType = XJournalObjectType.Pim;
				}
				if (xBase is XGuardZone)
				{
					journalObjectType = XJournalObjectType.GuardZone;
				}
			}

			var journalItem = new XJournalItem()
			{
				SystemDateTime = DateTime.Now,
				DeviceDateTime = DateTime.Now,
				JournalObjectType = journalObjectType,
				StateClass = EventDescriptionAttributeHelper.ToStateClass(journalEventNameType),
				JournalEventNameType = journalEventNameType,
				Name = EventDescriptionAttributeHelper.ToName(journalEventNameType),
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

		public static event Action<XJournalItem, bool> NewJournalItem;
		static void OnNewJournalItem(XJournalItem journalItem, bool isAdministrator)
		{
			if (NewJournalItem != null)
				NewJournalItem(journalItem, isAdministrator);
		}
		#endregion
	}
}