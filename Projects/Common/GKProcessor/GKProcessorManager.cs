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

		public static OperationResult<GKDeviceConfiguration> GKAutoSearch(GKDevice device, string userName)
		{
			AddGKMessage(JournalEventNameType.Автопоиск, JournalEventDescriptionType.NULL, "", device, userName);
			SuspendMonitoring(device);
			var gkAutoSearchHelper = new GKAutoSearchHelper();
			var deviceConfiguration = gkAutoSearchHelper.AutoSearch(device);
			ResumeMonitoring(device);
			return new OperationResult<GKDeviceConfiguration> { HasError = gkAutoSearchHelper.Error != null, Error = gkAutoSearchHelper.Error, Result = deviceConfiguration };
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
			var result = journalParser.GKJournalRecordNo;
			return new OperationResult<int>() { Result = result };
		}

		public static OperationResult<JournalItem> GKReadJournalItem(GKDevice device, int no)
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

		public static OperationResult<bool> GKSetSingleParameter(GKBase gkBase, List<byte> parameterBytes)
		{
			var error = ParametersHelper.SetSingleParameter(gkBase, parameterBytes);
			return new OperationResult<bool>() { HasError = error != null, Error = error, Result = true };
		}

		public static OperationResult<List<GKProperty>> GKGetSingleParameter(GKBase gkBase)
		{
			return ParametersHelper.GetSingleParameter(gkBase);
		}

		public static OperationResult<bool> GKSetSchedule(GKDevice device, GKSchedule schedule)
		{
			var count = 0;
			foreach (var dayScheduleUID in schedule.DayScheduleUIDs)
			{
				var daySchedule = GKManager.DeviceConfiguration.DaySchedules.FirstOrDefault(x => x.UID == dayScheduleUID);
				if (daySchedule != null)
				{
					count += daySchedule.DayScheduleParts.Count;
				}
			}
			var secondsPeriod = schedule.DayScheduleUIDs.Count * 60 * 60 * 24;
			if (schedule.SchedulePeriodType == GKSchedulePeriodType.Custom)
				secondsPeriod = schedule.HoursPeriod * 60 * 60;
			if (schedule.SchedulePeriodType == GKSchedulePeriodType.NonPeriodic)
				secondsPeriod = 0;

			var bytes = new List<byte>();
			bytes.Add((byte)schedule.No);
			var nameBytes = BytesHelper.StringDescriptionToBytes(schedule.Name);
			bytes.AddRange(nameBytes);
			bytes.Add(0);
			bytes.AddRange(BytesHelper.ShortToBytes((ushort)(count * 2)));
			bytes.AddRange(BytesHelper.IntToBytes(secondsPeriod));
			bytes.Add(0);
			bytes.Add(0);
			bytes.Add(0);
			bytes.Add(0);
			bytes.Add(0);
			bytes.Add(0);
			bytes.Add(0);
			bytes.Add(0);

			var startDateTime = schedule.StartDateTime;
			if (schedule.SchedulePeriodType == GKSchedulePeriodType.Weekly)
			{
				if (startDateTime.DayOfWeek == DayOfWeek.Monday)
					startDateTime.AddDays(0);
				if (startDateTime.DayOfWeek == DayOfWeek.Tuesday)
					startDateTime.AddDays(-1);
				if (startDateTime.DayOfWeek == DayOfWeek.Wednesday)
					startDateTime.AddDays(-2);
				if (startDateTime.DayOfWeek == DayOfWeek.Thursday)
					startDateTime.AddDays(-3);
				if (startDateTime.DayOfWeek == DayOfWeek.Friday)
					startDateTime.AddDays(-4);
				if (startDateTime.DayOfWeek == DayOfWeek.Saturday)
					startDateTime.AddDays(-5);
				if (startDateTime.DayOfWeek == DayOfWeek.Sunday)
					startDateTime.AddDays(-6);
			}
			var timeSpan = startDateTime - new DateTime(2000, 1, 1);
			var scheduleStartSeconds = timeSpan.TotalSeconds;

			for (int i = 0; i < schedule.DayScheduleUIDs.Count; i++)
			{
				var dayScheduleUID = schedule.DayScheduleUIDs[i];
				var daySchedule = GKManager.DeviceConfiguration.DaySchedules.FirstOrDefault(x => x.UID == dayScheduleUID);
				if (daySchedule != null)
				{
					foreach (var daySchedulePart in daySchedule.DayScheduleParts)
					{
						bytes.AddRange(BytesHelper.ShortToBytes((ushort)(daySchedulePart.StartMilliseconds / 1000)));
						bytes.AddRange(BytesHelper.ShortToBytes((ushort)(daySchedulePart.EndMilliseconds / 1000)));
					}
				}
			}

			var packs = new List<List<byte>>();
			for (int packNo = 0; packNo <= bytes.Count / 256; packNo++)
			{
				int packLenght = Math.Min(256, bytes.Count - packNo * 256);
				var packBytes = bytes.Skip(packNo * 256).Take(packLenght).ToList();

				if (packBytes.Count > 0)
				{
					var resultBytes = new List<byte>();
					resultBytes.Add((byte)(packNo));
					resultBytes.AddRange(packBytes);
					packs.Add(resultBytes);
				}
			}

			foreach (var pack in packs)
			{
				var sendResult = SendManager.Send(device, (ushort)(pack.Count), 28, 0, pack);
				if (sendResult.HasError)
				{
					return new OperationResult<bool>(sendResult.Error);
				}
			}

			return new OperationResult<bool>() { Result = true };
		}

		//public static OperationResult<GKSchedule> GKGetSchedule(GKDevice device, int no)
		//{
		//    var resultBytes = new List<byte>();

		//    var bytes = new List<byte>();
		//    bytes.Add(0);
		//    bytes.Add(1);
		//    var sendResult = SendManager.Send(device, (ushort)bytes.Count, 27, 0, bytes);
		//    if (!sendResult.HasError)
		//    {
		//        if (sendResult.Bytes.Count > 0)
		//        {
		//            sendResult.Bytes.RemoveAt(0);
		//            resultBytes.AddRange(sendResult.Bytes);
		//        }
		//    }

		//    if (bytes.Count > 0)
		//    {
		//        var schedule = new GKSchedule();
		//        schedule.No = bytes[0];
		//        schedule.Name = BytesHelper.BytesToString(bytes.Skip(1).Take(32).ToList());
		//        var holidayScheduleNo = bytes[33];
		//        var partsCount = BytesHelper.SubstructShort(bytes, 34) / 2;
		//        var duration = BytesHelper.SubstructInt(bytes, 36);
		//        var shortScheduleNo = bytes[40];

		//        var dayScheduleParts = new List<GKDaySchedulePart>();
		//        for (int i = 48; i < bytes.Count; i+=4)
		//        {
		//            var startSeconds = BytesHelper.SubstructShort(bytes, i);
		//            var endSeconds = BytesHelper.SubstructShort(bytes, i + 2);
		//            var daySchedulePart = new GKDaySchedulePart();
		//            daySchedulePart.StartMilliseconds = startSeconds * 1000;
		//            daySchedulePart.EndMilliseconds = endSeconds * 1000;
		//            dayScheduleParts.Add(daySchedulePart);
		//        }
		//        return new OperationResult<GKSchedule>() { Result = schedule };
		//    }

		//    return new OperationResult<GKSchedule>("Ошибка");
		//}

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

		public static void GKReset(GKBase gkBase, string userName)
		{
			Watcher.SendControlCommand(gkBase, GKStateBit.Reset, "Сброс");
			AddGKMessage(JournalEventNameType.Команда_оператора, JournalEventDescriptionType.Сброс, "", gkBase, userName);
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

		public static void GKSetAutomaticRegime(GKBase gkBase, string userName)
		{
			Watcher.SendControlCommand(gkBase, GKStateBit.SetRegime_Automatic, "Перевод в автоматический режим");
			AddGKMessage(JournalEventNameType.Команда_оператора, JournalEventDescriptionType.Перевод_в_автоматический_режим, "", gkBase, userName);
		}

		public static void GKSetManualRegime(GKBase gkBase, string userName)
		{
			Watcher.SendControlCommand(gkBase, GKStateBit.SetRegime_Manual, "Перевод в ручной режим");
			AddGKMessage(JournalEventNameType.Команда_оператора, JournalEventDescriptionType.Перевод_в_ручной_режим, "", gkBase, userName);
		}

		public static void GKSetIgnoreRegime(GKBase gkBase, string userName)
		{
			Watcher.SendControlCommand(gkBase, GKStateBit.SetRegime_Off, "Перевод в отключенный режим");
			AddGKMessage(JournalEventNameType.Команда_оператора, JournalEventDescriptionType.Перевод_в_отключенный_режим, "", gkBase, userName);
		}

		public static void GKTurnOn(GKBase gkBase, string userName)
		{
			Watcher.SendControlCommand(gkBase, GKStateBit.TurnOn_InManual, "Включить");
			AddGKMessage(JournalEventNameType.Команда_оператора, JournalEventDescriptionType.Включить, "", gkBase, userName);
		}

		public static void GKTurnOnNow(GKBase gkBase, string userName)
		{
			Watcher.SendControlCommand(gkBase, GKStateBit.TurnOnNow_InManual, "Включить немедленно");
			AddGKMessage(JournalEventNameType.Команда_оператора, JournalEventDescriptionType.Включить_немедленно, "", gkBase, userName);
		}

		public static void GKTurnOff(GKBase gkBase, string userName)
		{
			Watcher.SendControlCommand(gkBase, GKStateBit.TurnOff_InManual, "Выключить");
			AddGKMessage(JournalEventNameType.Команда_оператора, JournalEventDescriptionType.Выключить, "", gkBase, userName);
		}

		public static void GKTurnOffNow(GKBase gkBase, string userName)
		{
			Watcher.SendControlCommand(gkBase, GKStateBit.TurnOffNow_InManual, "Выключить немедленно");
			AddGKMessage(JournalEventNameType.Команда_оператора, JournalEventDescriptionType.Выключить_немедленно, "", gkBase, userName);
		}

		public static void GKStop(GKBase gkBase, string userName)
		{
			Watcher.SendControlCommand(gkBase, GKStateBit.Stop_InManual, "Остановка пуска");
			AddGKMessage(JournalEventNameType.Команда_оператора, JournalEventDescriptionType.Остановка_пуска, "", gkBase, userName);
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
		public static void AddGKMessage(JournalEventNameType journalEventNameType, JournalEventDescriptionType journalEventDescriptionType, string description, GKBase gkBase, string userName)
		{
			Guid uid = Guid.Empty;
			var journalObjectType = JournalObjectType.None;
			if (gkBase != null)
			{
				uid = gkBase.UID;
				if (gkBase is GKDevice)
				{
					journalObjectType = JournalObjectType.GKDevice;
				}
				if (gkBase is GKZone)
				{
					journalObjectType = JournalObjectType.GKZone;
				}
				if (gkBase is GKDirection)
				{
					journalObjectType = JournalObjectType.GKDirection;
				}
				if (gkBase is GKDelay)
				{
					journalObjectType = JournalObjectType.GKDelay;
				}
				if (gkBase is GKPim)
				{
					journalObjectType = JournalObjectType.GKPim;
				}
				if (gkBase is GKGuardZone)
				{
					journalObjectType = JournalObjectType.GKGuardZone;
				}
			}

			var journalItem = new JournalItem()
			{
				SystemDateTime = DateTime.Now,
				DeviceDateTime = DateTime.Now,
				JournalObjectType = journalObjectType,
				JournalEventNameType = journalEventNameType,
				JournalEventDescriptionType = journalEventDescriptionType,
				NameText = EventDescriptionAttributeHelper.ToName(journalEventNameType),
				DescriptionText = description,
				ObjectUID = uid,
				UserName = userName,
				JournalSubsystemType = JournalSubsystemType.System
			};
			if (gkBase != null)
			{
				journalItem.ObjectName = gkBase.PresentationName;
				var gkObjectNo = (ushort)gkBase.GKDescriptorNo;
				if (gkObjectNo > 0)
					journalItem.JournalDetalisationItems.Add(new JournalDetalisationItem("Компонент ГК", gkObjectNo.ToString()));
			}

			var gkCallbackResult = new GKCallbackResult();
			gkCallbackResult.JournalItems.Add(journalItem);
			OnGKCallbackResult(gkCallbackResult);
		}
		#endregion
	}
}