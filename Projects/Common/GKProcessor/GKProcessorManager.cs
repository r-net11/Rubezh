using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HexManager;
using XFiresecAPI;
using FiresecClient;
using System.Runtime.Serialization;
using FiresecAPI;

namespace GKProcessor
{
	public static class GKProcessorManager
	{
		#region Callback
		public static bool IsProgressCanceled = false;
		public static void CancelGKProgress()
		{
			IsProgressCanceled = true;
		}

		static string CurrentTitle;
		static int CurrentStepCount;
		static bool CurrentCanCancel;
		static GKProgressClientType CurrentGKProgressClientType;

		public static void OnStartProgress(string title, string text, int stepCount, bool canCancel, GKProgressClientType progressClientType)
		{
			IsProgressCanceled = false;
			var gkProgressCallback = new GKProgressCallback();
			gkProgressCallback.GKProgressCallbackType = GKProgressCallbackType.Start;
			CurrentTitle = gkProgressCallback.Title = title;
			gkProgressCallback.Text = text;
			CurrentStepCount = gkProgressCallback.StepCount = stepCount;
			CurrentCanCancel = gkProgressCallback.CanCancel = canCancel;
			CurrentGKProgressClientType = gkProgressCallback.GKProgressClientType = progressClientType;
			OnGKCallbackResult(gkProgressCallback);
		}

		public static void OnDoProgress(string text)
		{
			var gkProgressCallback = new GKProgressCallback();
			gkProgressCallback.GKProgressCallbackType = GKProgressCallbackType.Progress;
			gkProgressCallback.Title = CurrentTitle;
			gkProgressCallback.Text = text;
			gkProgressCallback.StepCount = CurrentStepCount;
			gkProgressCallback.CanCancel = CurrentCanCancel;
			gkProgressCallback.GKProgressClientType = CurrentGKProgressClientType;
			OnGKCallbackResult(gkProgressCallback);
		}

		public static void OnStopProgress()
		{
			var gkProgressCallback = new GKProgressCallback();
			gkProgressCallback.GKProgressCallbackType = GKProgressCallbackType.Stop;
			OnGKCallbackResult(gkProgressCallback);
		}

		public static void OnGKCallbackResult(GKProgressCallback gkProgressCallback)
		{
			if (GKProgressCallbackEvent != null)
				GKProgressCallbackEvent(gkProgressCallback);
		}
		public static event Action<GKProgressCallback> GKProgressCallbackEvent;

		public static void OnGKCallbackResult(GKCallbackResult gkCallbackResult)
		{
			if (gkCallbackResult.JournalItems.Count +
				gkCallbackResult.GKStates.DeviceStates.Count +
				gkCallbackResult.GKStates.ZoneStates.Count +
				gkCallbackResult.GKStates.DirectionStates.Count +
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
		public static void Start()
		{
			WatcherManager.IsConfigurationReloading = false;
			WatcherManager.Start();
		}

		public static void Stop()
		{
			WatcherManager.Stop();
		}

		public static void Suspend()
		{
			WatcherManager.LastConfigurationReloadingTime = DateTime.Now;
			WatcherManager.IsConfigurationReloading = true;
		}

		static void SuspendMonitoring(XDevice gkDevice)
		{
			if (WatcherManager.Watchers != null)
			{
				var watcher = WatcherManager.Watchers.FirstOrDefault(x => x.GkDatabase.RootDevice.UID == gkDevice.UID);
				if (watcher != null)
					watcher.Suspend();
			}
		}

		static void ResumeMonitoring(XDevice gkDevice)
		{
			if (WatcherManager.Watchers != null)
			{
				var watcher = WatcherManager.Watchers.FirstOrDefault(x => x.GkDatabase.RootDevice.UID == gkDevice.UID);
				if (watcher != null)
					watcher.Resume();
			}
		}
		#endregion

		#region Operations
		public static OperationResult<bool> GKWriteConfiguration(XDevice device, string userName)
		{
			AddGKMessage("Запись конфигурации в прибор", "", XStateClass.Info, device, userName, true);
			Stop();
			var gkDescriptorsWriter = new GkDescriptorsWriter();
			gkDescriptorsWriter.WriteConfig(device);
			Start();
			if (gkDescriptorsWriter.Error != null)
				return new OperationResult<bool>(gkDescriptorsWriter.Error) { Result = false };
			return new OperationResult<bool>() { Result = true };
		}

		public static OperationResult<XDeviceConfiguration> GKReadConfiguration(XDevice device, string userName)
		{
			AddGKMessage("Чтение конфигурации из прибора", "", XStateClass.Info, device, userName, true);
			SuspendMonitoring(device);
			DescriptorsManager.Create();
			var descriptorReader = device.Driver.IsKauOrRSR2Kau ? (DescriptorReaderBase)new KauDescriptorsReaderBase() : new GkDescriptorsReaderBase();
			descriptorReader.ReadConfiguration(device);
			ResumeMonitoring(device);
			return new OperationResult<XDeviceConfiguration> { HasError = descriptorReader.Error != null, Error = descriptorReader.Error, Result = descriptorReader.DeviceConfiguration };
		}

		public static OperationResult<XDeviceConfiguration> GKReadConfigurationFromGKFile(XDevice device, string userName)
		{
			AddGKMessage("Чтение конфигурации из прибора", "", XStateClass.Info, device, userName, true);
			SuspendMonitoring(device);
			var gkFileReaderWriter = new GKFileReaderWriter();
			var deviceConfiguration = gkFileReaderWriter.ReadConfigFileFromGK(device);
			ResumeMonitoring(device);
			return new OperationResult<XDeviceConfiguration> { HasError = gkFileReaderWriter.Error != null, Error = gkFileReaderWriter.Error, Result = deviceConfiguration };
		}

		public static OperationResult<bool> GKUpdateFirmware(XDevice device, string fileName, string userName)
		{
			AddGKMessage("Обновление ПО прибора", "", XStateClass.Info, device, userName, true);
			Stop();
			var firmwareUpdateHelper = new FirmwareUpdateHelper();
			firmwareUpdateHelper.Update(device, fileName);
			Start();
			if (firmwareUpdateHelper.ErrorList != null)
				return new OperationResult<bool>(firmwareUpdateHelper.ErrorList.Aggregate((a,b)=> a + "\n" + b)) { Result = false };
			return new OperationResult<bool> { Result = true };
		}

		public static OperationResult<bool> GKUpdateFirmwareFSCS(HexFileCollectionInfo hxcFileInfo, string userName, List<XDevice> devices)
		{
			Stop();
			var firmwareUpdateHelper = new FirmwareUpdateHelper();
			firmwareUpdateHelper.UpdateFSCS(hxcFileInfo, userName, devices);
			Start();
			if (firmwareUpdateHelper.ErrorList != null)
                return new OperationResult<bool>(firmwareUpdateHelper.ErrorList.Aggregate((a, b) => a + "\n" + b)) { Result = false };
			return new OperationResult<bool> { Result = true };
		}

		public static bool GKSyncronyseTime(XDevice device, string userName)
		{
			AddGKMessage("Синхронизация времени", "", XStateClass.Info, device, userName, true);
			return DeviceBytesHelper.WriteDateTime(device);
		}

		public static string GKGetDeviceInfo(XDevice device, string userName)
		{
			AddGKMessage("Запрос информации об устройсве", "", XStateClass.Info, device, userName, true);
			return DeviceBytesHelper.GetDeviceInfo(device);
		}

		public static OperationResult<int> GKGetJournalItemsCount(XDevice device)
		{
			var sendResult = SendManager.Send(device, 0, 6, 64);
			if (sendResult.HasError)
			{
				return new OperationResult<int>("Ошибка связи с устройством");
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
				return new OperationResult<JournalItem>("Ошибка связи с устройством");
			}
			var journalParser = new JournalParser(device, sendResult.Bytes);
			return new OperationResult<JournalItem>() { Result = journalParser.JournalItem };
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
				device.InternalState.CopyToXState(device.State);
				gkStates.DeviceStates.Add(device.State);
			}
			foreach (var zone in XManager.Zones)
			{
				zone.InternalState.CopyToXState(zone.State);
				gkStates.ZoneStates.Add(zone.State);
			}
			foreach (var direction in XManager.Directions)
			{
				direction.InternalState.CopyToXState(direction.State);
				gkStates.DirectionStates.Add(direction.State);
			}
			foreach (var pumpStation in XManager.PumpStations)
			{
				pumpStation.InternalState.CopyToXState(pumpStation.State);
				gkStates.PumpStationStates.Add(pumpStation.State);
			}
			foreach (var delay in XManager.Delays)
			{
				delay.InternalState.CopyToXState(delay.State);
				delay.State.PresentationName = delay.PresentationName;
				gkStates.DelayStates.Add(delay.State);
			}
			foreach (var pim in XManager.Pims)
			{
				pim.InternalState.CopyToXState(pim.State);
				pim.State.PresentationName = pim.PresentationName;
				gkStates.PumpStationStates.Add(pim.State);
			}
			return gkStates;
		}

		public static void GKExecuteDeviceCommand(XDevice device, XStateBit stateBit, string userName)
		{
			Watcher.SendControlCommand(device, stateBit);
			AddGKMessage("Команда оператора", stateBit.ToDescription(), XStateClass.Info, device, userName);
		}

		public static void GKReset(XBase xBase, string userName)
		{
			Watcher.SendControlCommand(xBase, XStateBit.Reset);
			AddGKMessage("Команда оператора", "Сброс", XStateClass.Info, xBase, userName);
		}

		public static void GKResetFire1(XZone zone, string userName)
		{
			Watcher.SendControlCommand(zone, 0x02);
			AddGKMessage("Команда оператора", "Сброс", XStateClass.Info, zone, userName);
		}

		public static void GKResetFire2(XZone zone, string userName)
		{
			Watcher.SendControlCommand(zone, 0x03);
			AddGKMessage("Команда оператора", "Сброс", XStateClass.Info, zone, userName);
		}

		public static void GKSetAutomaticRegime(XBase xBase, string userName)
		{
			Watcher.SendControlCommand(xBase, XStateBit.SetRegime_Automatic);
			AddGKMessage("Команда оператора", "Перевод в автоматический режим", XStateClass.Info, xBase, userName);
		}

		public static void GKSetManualRegime(XBase xBase, string userName)
		{
			Watcher.SendControlCommand(xBase, XStateBit.SetRegime_Manual);
			AddGKMessage("Команда оператора", "Перевод в ручной режим", XStateClass.Info, xBase, userName);
		}

		public static void GKSetIgnoreRegime(XBase xBase, string userName)
		{
			Watcher.SendControlCommand(xBase, XStateBit.SetRegime_Off);
			AddGKMessage("Команда оператора", "Перевод в ручной режим", XStateClass.Info, xBase, userName);
		}

		public static void GKTurnOn(XBase xBase, string userName)
		{
			Watcher.SendControlCommand(xBase, XStateBit.TurnOn_InManual);
			AddGKMessage("Команда оператора", "Включить", XStateClass.Info, xBase, userName);
		}

		public static void GKTurnOnNow(XBase xBase, string userName)
		{
			Watcher.SendControlCommand(xBase, XStateBit.TurnOnNow_InManual);
			AddGKMessage("Команда оператора", "Включить немедленно", XStateClass.Info, xBase, userName);
		}

		public static void GKTurnOff(XBase xBase, string userName)
		{
			Watcher.SendControlCommand(xBase, XStateBit.TurnOff_InManual);
			AddGKMessage("Команда оператора", "Выключить", XStateClass.Info, xBase, userName);
		}

		public static void GKTurnOffNow(XBase xBase, string userName)
		{
			Watcher.SendControlCommand(xBase, XStateBit.TurnOffNow_InManual);
			AddGKMessage("Команда оператора", "Выключить немедленно", XStateClass.Info, xBase, userName);
		}

		public static void GKStop(XBase xBase, string userName)
		{
			Watcher.SendControlCommand(xBase, XStateBit.Stop_InManual);
			AddGKMessage("Команда оператора", "Остановка пуска", XStateClass.Info, xBase, userName);
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

		#endregion

		#region JournalItem Callback
		public static void AddGKMessage(string message, string description, XStateClass stateClass, XBase xBase, string userName, bool isAdministrator = false)
		{
			Guid uid = Guid.Empty;
			var journalItemType = JournalItemType.System;
			if (xBase != null)
			{
				if (xBase is XDevice)
				{
					uid = (xBase as XDevice).UID;
					journalItemType = JournalItemType.Device;
				}
				if (xBase is XZone)
				{
					uid = (xBase as XZone).UID;
					journalItemType = JournalItemType.Zone;
				}
				if (xBase is XDirection)
				{
					uid = (xBase as XDirection).UID;
					journalItemType = JournalItemType.Direction;
				}
				if (xBase is XDelay)
				{
					uid = (xBase as XDelay).UID;
					journalItemType = JournalItemType.Delay;
				}
				if (xBase is XPim)
				{
					uid = (xBase as XPim).UID;
					journalItemType = JournalItemType.Pim;
				}
			}

			var journalItem = new JournalItem()
			{
				SystemDateTime = DateTime.Now,
				DeviceDateTime = DateTime.Now,
				JournalItemType = journalItemType,
				StateClass = stateClass,
				Name = message,
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