using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Journal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GKProcessor
{
	public static class GKProcessorManager
	{
		#region Callback
		public static List<GKProgressCallback> GKProgressCallbacks = new List<GKProgressCallback>();

		public static void CancelGKProgress(Guid progressCallbackUID, string userName, Guid? clientUID = null)
		{
			var progressCallback = GKProgressCallbacks.FirstOrDefault(x => x.UID == progressCallbackUID);
			if (progressCallback != null)
			{
				progressCallback.IsCanceled = true;
				progressCallback.CancelizationDateTime = DateTime.Now;
				StopProgress(progressCallback, clientUID);
				AddGKMessage(JournalEventNameType.Отмена_операции, JournalEventDescriptionType.NULL, progressCallback.Title, null, userName);
			}
		}

		public static GKProgressCallback StartProgress(string title, string text, int stepCount, bool canCancel, GKProgressClientType progressClientType, Guid? clientUID = null)
		{
			var gkProgressCallback = new GKProgressCallback
			{
				GKProgressCallbackType = GKProgressCallbackType.Start,
				Title = title,
				Text = text,
				StepCount = stepCount,
				CanCancel = canCancel,
				GKProgressClientType = progressClientType
			};
			GKProgressCallbacks.Add(gkProgressCallback);
			OnGKCallbackResult(gkProgressCallback, clientUID);
			return gkProgressCallback;
		}

		public static void DoProgress(string text, GKProgressCallback progressCallback, Guid? clientUID = null)
		{
			progressCallback.CurrentStep++;
			var gkProgressCallback = new GKProgressCallback
			{
				UID = progressCallback.UID,
				LastActiveDateTime = DateTime.Now,
				GKProgressCallbackType = GKProgressCallbackType.Progress,
				Title = progressCallback.Title,
				Text = text,
				StepCount = progressCallback.StepCount,
				CurrentStep = progressCallback.CurrentStep,
				CanCancel = progressCallback.CanCancel,
				GKProgressClientType = progressCallback.GKProgressClientType
			};
			OnGKCallbackResult(gkProgressCallback, clientUID);
		}

		public static void StopProgress(GKProgressCallback progressCallback, Guid? clientUID = null)
		{
			var gkProgressCallback = new GKProgressCallback
			{
				UID = progressCallback.UID,
				LastActiveDateTime = DateTime.Now,
				GKProgressCallbackType = GKProgressCallbackType.Stop,
				GKProgressClientType = progressCallback.GKProgressClientType
			};
			GKProgressCallbacks.Remove(gkProgressCallback);
			OnGKCallbackResult(gkProgressCallback, clientUID);
		}

		static void OnGKCallbackResult(GKProgressCallback gkProgressCallback, Guid? clientUID)
		{
			GKProgressCallbacks.RemoveAll(x => x.IsCanceled && (DateTime.Now - x.CancelizationDateTime).TotalMinutes > 5);
			if (gkProgressCallback.GKProgressCallbackType == GKProgressCallbackType.Stop || !gkProgressCallback.IsCanceled)
			{
				if (GKProgressCallbackEvent != null)
					GKProgressCallbackEvent(gkProgressCallback, clientUID);
			}
		}
		public static event Action<GKProgressCallback, Guid?> GKProgressCallbackEvent;

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
				gkCallbackResult.GKStates.DoorStates.Count +
				gkCallbackResult.GKStates.SKDZoneStates.Count +
				gkCallbackResult.GKStates.DeviceMeasureParameters.Count > 0)
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

		static void SuspendMonitoring(GKDevice gkControllerDevice)
		{
			gkControllerDevice = GetGKDevice(gkControllerDevice);
			if (WatcherManager.Watchers != null && gkControllerDevice != null)
			{
				var watcher = WatcherManager.Watchers.FirstOrDefault(x => x.GkDatabase.RootDevice.UID == gkControllerDevice.UID);
				if (watcher != null)
					watcher.Suspend();
			}
		}

		static void ResumeMonitoring(GKDevice gkControllerDevice)
		{
			gkControllerDevice = GetGKDevice(gkControllerDevice);
			if (WatcherManager.Watchers != null && gkControllerDevice != null)
			{
				var watcher = WatcherManager.Watchers.FirstOrDefault(x => x.GkDatabase.RootDevice.UID == gkControllerDevice.UID);
				if (watcher != null)
					watcher.Resume();
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
		public static OperationResult<bool> GKWriteConfiguration(GKDevice device, string userName, GKProgressCallback progressCallback, Guid clientUID)
		{
			AddGKMessage(JournalEventNameType.Запись_конфигурации_в_прибор, JournalEventDescriptionType.NULL, "", device, userName);

			Stop();
			GKSyncronyseTime(device, userName);
			var gkDescriptorsWriter = new GkDescriptorsWriter();
			gkDescriptorsWriter.WriteConfig(device, progressCallback, clientUID);
			Start();

			if (gkDescriptorsWriter.Errors.Count > 0)
			{
				var errors = new List<string>();
				foreach (var error in gkDescriptorsWriter.Errors)
				{
					errors.Add(error);
				}
				return OperationResult<bool>.FromError(errors, false);
			}
			return new OperationResult<bool>(true);
		}

		public static OperationResult<GKDeviceConfiguration> GKReadConfiguration(GKDevice device, string userName, Guid clientUID)
		{
			AddGKMessage(JournalEventNameType.Чтение_конфигурации_из_прибора, JournalEventDescriptionType.NULL, "", device, userName);
			Stop();
			DescriptorReaderBase descriptorReader;
			if (device.Driver.IsKau)
			{
				descriptorReader = new KauDescriptorsReaderBase();
				descriptorReader.ReadConfiguration(device, clientUID);
				Start();
				return OperationResult<GKDeviceConfiguration>.FromError(descriptorReader.Error, descriptorReader.DeviceConfiguration);
			}
			if (device.Driver.DriverType == GKDriverType.GK)
			{
				descriptorReader = new GkDescriptorsReaderBase();
				descriptorReader.ReadConfiguration(device, clientUID);
				Start();
				return OperationResult<GKDeviceConfiguration>.FromError(descriptorReader.Error, descriptorReader.DeviceConfiguration);
			}
			if (device.Driver.DriverType == GKDriverType.GKMirror)
			{
				descriptorReader = new MirrorDescriptorsReader();
				descriptorReader.ReadConfiguration(device, clientUID);
				Start();
				return OperationResult<GKDeviceConfiguration>.FromError(descriptorReader.Error, descriptorReader.DeviceConfiguration);
			}
			Start();
			return OperationResult<GKDeviceConfiguration>.FromError("Для данного типа устройства запрещено чтение дескрипторов");
		}

		public static OperationResult<string> GKReadConfigurationFromGKFile(GKDevice device, string userName, GKProgressCallback progressCallback, Guid clientUID)
		{
			AddGKMessage(JournalEventNameType.Чтение_конфигурации_из_прибора, JournalEventDescriptionType.NULL, "", device, userName);
			SuspendMonitoring(device);
			var gkFileReaderWriter = new GKFileReaderWriter();
			var filePath = gkFileReaderWriter.ReadConfigFileFromGK(device, progressCallback, clientUID);
			ResumeMonitoring(device);
			return filePath;
		}

		public static OperationResult<GKDevice> GKAutoSearch(GKDevice device, string userName, Guid clientUID)
		{
			AddGKMessage(JournalEventNameType.Автопоиск, JournalEventDescriptionType.NULL, "", device, userName);
			SuspendMonitoring(device);
			var gkAutoSearchHelper = new GKAutoSearchHelper();
			var newDevice = device.DriverType == GKDriverType.GK ? gkAutoSearchHelper.AutoSearch(device, clientUID) : gkAutoSearchHelper.KauAutoSearch(device, clientUID);
			ResumeMonitoring(device);
			return OperationResult<GKDevice>.FromError(gkAutoSearchHelper.Error, newDevice);
		}

		public static OperationResult<bool> GKUpdateFirmware(GKDevice device, List<byte> firmwareBytes, string userName, Guid clientUID)
		{
			Stop();
			var firmwareUpdateHelper = new FirmwareUpdateHelper();
			string updateResult = firmwareUpdateHelper.Update(device, firmwareBytes, userName, clientUID);
			Start();
			if (updateResult != null)
				return OperationResult<bool>.FromError(updateResult, false);
			return new OperationResult<bool>(true);
		}

		public static bool GKSyncronyseTime(GKDevice device, string userName)
		{
			AddGKMessage(JournalEventNameType.Синхронизация_времени, JournalEventDescriptionType.NULL, "", device, userName);
			return DeviceBytesHelper.WriteDateTime(device);
		}

		public static string GKGetDeviceInfo(GKDevice device, string userName)
		{
			AddGKMessage(JournalEventNameType.Запрос_информации_об_устройстве, JournalEventDescriptionType.NULL, "", device, userName);
			return DeviceBytesHelper.GetDeviceInfo(device) ?? "Устройство недоступно";
		}

		public static OperationResult<int> GKGetJournalItemsCount(GKDevice device)
		{
			var sendResult = SendManager.Send(device, 0, 6, 64);
			if (sendResult.HasError)
			{
				return OperationResult<int>.FromError("Устройство недоступно");
			}
			var journalParser = new JournalParser(device, sendResult.Bytes);
			return new OperationResult<int>(journalParser.GKJournalRecordNo);
		}

		public static OperationResult<JournalItem> GKReadJournalItem(GKDevice device, int no)
		{
			var data = BitConverter.GetBytes(no).ToList();
			var sendResult = SendManager.Send(device, 4, 7, 64, data);
			if (sendResult.HasError)
			{
				return OperationResult<JournalItem>.FromError("Устройство недоступно");
			}
			if (sendResult.Bytes.Count == 64)
			{
				var journalParser = new JournalParser(device, sendResult.Bytes);
				return new OperationResult<JournalItem>(journalParser.JournalItem);
			}
			return OperationResult<JournalItem>.FromError("Ошибка. Недостаточное количество байт в записи журнала");
		}

		public static OperationResult<bool> GKSetSingleParameter(GKBase gkBase, List<byte> parameterBytes)
		{
			var error = ParametersHelper.SetSingleParameter(gkBase, parameterBytes);
			return OperationResult<bool>.FromError(error, true);
		}

		public static OperationResult<List<GKProperty>> GKGetSingleParameter(GKBase gkBase)
		{
			return ParametersHelper.GetSingleParameter(gkBase);
		}

		public static OperationResult<List<byte>> GKGKHash(GKDevice device)
		{
			var gkFileReaderWriter = new GKFileReaderWriter();
			var readInfoBlock = gkFileReaderWriter.ReadInfoBlock(device);
			if (gkFileReaderWriter.Error != null)
				return OperationResult<List<byte>>.FromError(gkFileReaderWriter.Error);
			return new OperationResult<List<byte>>(readInfoBlock.Hash1);
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
			foreach (var pim in GKManager.GlobalPims)
			{
				Watcher.AddObjectStateToGKStates(gkStates, pim);
			}
			foreach (var guardZone in GKManager.GuardZones)
			{
				Watcher.AddObjectStateToGKStates(gkStates, guardZone);
			}
			foreach (var door in GKManager.Doors)
			{
				Watcher.AddObjectStateToGKStates(gkStates, door);
			}
			foreach (var skdZone in GKManager.SKDZones)
			{
				Watcher.AddObjectStateToGKStates(gkStates, skdZone);
				CalculateSKDZone(skdZone);
				var zoneState = gkStates.SKDZoneStates.FirstOrDefault(x => x.UID == skdZone.UID);
				if (zoneState != null)
				{
					zoneState.StateClasses = skdZone.State.StateClasses;
					zoneState.StateClass = GKStatesHelper.GetMinStateClass(skdZone.State.StateClasses);
				}
			}
			return gkStates;
		}

		public static void CalculateSKDZone(GKSKDZone zone)
		{
			var stateClasses = new HashSet<XStateClass>();
			zone.State = new GKState(zone);
			foreach (var door in GKManager.Doors)
			{
				if (door.EnterZoneUID == zone.UID || door.ExitZoneUID == zone.UID)
				{
					foreach (var stateClass in door.State.StateClasses)
					{
						stateClasses.Add(stateClass);
					}
				}
			}
			zone.State.StateClasses = stateClasses.ToList();
			zone.State.StateClass = GKStatesHelper.GetMinStateClass(zone.State.StateClasses);
		}

		public static void GKExecuteDeviceCommand(GKDevice device, GKStateBit stateBit, string userName)
		{
			Watcher.SendControlCommand(device, stateBit, stateBit.ToDescription());
			AddGKMessage(JournalEventNameType.Команда_оператора, JournalEventDescriptionType.NULL, stateBit.ToDescription(), device, userName);
		}

		public static void GKReset(GKBase gkBase, string userName)
		{
			Watcher.SendControlCommand(gkBase, GKStateBit.Reset, "Сброс");
			AddGKMessage(JournalEventNameType.Команда_оператора, JournalEventDescriptionType.Сброс_Команда_оператора, "", gkBase, userName);
		}

		public static void GKResetFire1(GKZone zone, string userName)
		{
			Watcher.SendControlCommand(zone, 0x02, "Сброс Пожар-1");
			AddGKMessage(JournalEventNameType.Команда_оператора, JournalEventDescriptionType.Сброс_Команда_оператора, "", zone, userName);
		}

		public static void GKResetFire2(GKZone zone, string userName)
		{
			Watcher.SendControlCommand(zone, 0x03, "Сброс Пожар-2");
			AddGKMessage(JournalEventNameType.Команда_оператора, JournalEventDescriptionType.Сброс_Команда_оператора, "", zone, userName);
		}

		public static void GKSetAutomaticRegime(GKBase gkBase, string userName)
		{
			Watcher.SendControlCommand(gkBase, GKStateBit.SetRegime_Automatic, "Перевод в автоматический режим");
			AddGKMessage(JournalEventNameType.Команда_оператора, JournalEventDescriptionType.Перевод_в_автоматический_режим_Команда_оператора, "", gkBase, userName);
		}

		public static void GKSetManualRegime(GKBase gkBase, string userName)
		{
			Watcher.SendControlCommand(gkBase, GKStateBit.SetRegime_Manual, "Перевод в ручной режим");
			AddGKMessage(JournalEventNameType.Команда_оператора, JournalEventDescriptionType.Перевод_в_ручной_режим_Команда_оператора, "", gkBase, userName);
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

		public static void GKTurnOnInAutomatic(GKBase gkBase, string userName)
		{
			Watcher.SendControlCommand(gkBase, GKStateBit.TurnOn_InAutomatic, "Включить в автоматике");
			AddGKMessage(JournalEventNameType.Команда_оператора, JournalEventDescriptionType.Включить_в_автоматическом_режиме, "", gkBase, userName);
		}

		public static void GKTurnOnNowInAutomatic(GKBase gkBase, string userName)
		{
			Watcher.SendControlCommand(gkBase, GKStateBit.TurnOnNow_InAutomatic, "Включить немедленно в автоматике");
			AddGKMessage(JournalEventNameType.Команда_оператора, JournalEventDescriptionType.Включить_немедленно_в_автоматическом_режиме, "", gkBase, userName);
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

		public static void GKTurnOffInAutomatic(GKBase gkBase, string userName)
		{
			Watcher.SendControlCommand(gkBase, GKStateBit.TurnOff_InAutomatic, "Выключить в автоматике");
			AddGKMessage(JournalEventNameType.Команда_оператора, JournalEventDescriptionType.Выключить_в_автоматическом_режиме, "", gkBase, userName);
		}

		public static void GKTurnOffNowInAutomatic(GKBase gkBase, string userName)
		{
			Watcher.SendControlCommand(gkBase, GKStateBit.TurnOffNow_InAutomatic, "Выключить немедленно в автоматике");
			AddGKMessage(JournalEventNameType.Команда_оператора, JournalEventDescriptionType.Выключить_немедленно_в_автоматическом_режиме, "", gkBase, userName);
		}

		public static void GKTurnOnNowGlobalPimsInAutomatic(string userName)
		{
			foreach (var globalPim in GKManager.GlobalPims)
			{
				Watcher.SendControlCommand(globalPim, GKStateBit.TurnOnNow_InAutomatic, "Включить немедленно в автоматике");
				AddGKMessage(JournalEventNameType.Команда_оператора, JournalEventDescriptionType.Включить_немедленно_в_автоматическом_режиме, "", globalPim, userName);
			}
		}

		public static void GKTurnOffNowGlobalPimsInAutomatic(string userName)
		{
			foreach (var globalPim in GKManager.GlobalPims)
			{
				Watcher.SendControlCommand(globalPim, GKStateBit.TurnOffNow_InAutomatic, "Выключить немедленно в автоматике");
				AddGKMessage(JournalEventNameType.Команда_оператора, JournalEventDescriptionType.Выключить_немедленно_в_автоматическом_режиме, "", globalPim, userName);
			}
		}

		public static void GKStop(GKBase gkBase, string userName)
		{
			Watcher.SendControlCommand(gkBase, GKStateBit.Stop_InManual, "Остановка пуска");
			AddGKMessage(JournalEventNameType.Команда_оператора, JournalEventDescriptionType.Остановка_пуска, "", gkBase, userName);
		}

		public static void GKStartMeasureMonitoring(GKDevice device)
		{
			if (device == null || device.KAUParent == null || device.KAUParent.GkDatabaseParent == null)
				return;
			var watcher = WatcherManager.Watchers.FirstOrDefault(x => x.GkDatabase.RootDevice.UID == device.KAUParent.GkDatabaseParent.UID);
			if (watcher != null)
			{
				watcher.StartDeviceMeasure(device);
			}
		}

		public static void GKStopMeasureMonitoring(GKDevice device)
		{
			if (device == null || device.Parent == null || device.Parent.GkDatabaseParent == null)
				return;
			var watcher = WatcherManager.Watchers.FirstOrDefault(x => x.GkDatabase.RootDevice.UID == device.Parent.GkDatabaseParent.UID);
			if (watcher != null)
			{
				watcher.StopDeviceMeasure(device);
			}
		}

		public static OperationResult<uint> GKGetReaderCode(GKDevice device)
		{
			var sendResult = SendManager.Send(device.GkDatabaseParent, 2, 12, 68, BytesHelper.ShortToBytes(device.GKDescriptorNo));
			if (!sendResult.HasError)
			{
				var code = BytesHelper.SubstructInt(sendResult.Bytes, 52);
				return new OperationResult<uint>((uint)code);
			}
			else
			{
				return OperationResult<uint>.FromError(sendResult.Error);
			}
		}

		public static void GKOpenSKDZone(GKSKDZone zone)
		{
			foreach (var door in GKManager.Doors)
			{
				if (door.EnterZoneUID == zone.UID)
				{
					AddGKMessage(JournalEventNameType.Открытие_зоны_СКД, JournalEventDescriptionType.NULL, "", zone, null);
					Watcher.SendControlCommand(door, GKStateBit.TurnOn_InAutomatic, "Включить в автоматике");
				}
			}
		}

		public static void GKCloseSKDZone(GKSKDZone zone)
		{
			foreach (var door in GKManager.Doors)
			{
				if (door.EnterZoneUID == zone.UID)
				{
					AddGKMessage(JournalEventNameType.Закрытие_зоны_СКД, JournalEventDescriptionType.NULL, "", zone, null);
					Watcher.SendControlCommand(door, GKStateBit.TurnOff_InAutomatic, "Выключить в автоматике");
				}
			}
		}

		public static OperationResult<CurrentConsumption> GetAlsMeasure(Guid alsUid)
		{
			var alsDevice = GKManager.Devices.FirstOrDefault(x => x.UID == alsUid);
			if (alsDevice == null)
				return null;
			var kauDevice = GKManager.Devices.FirstOrDefault(x => x.Children.Any(y => y.UID == alsDevice.UID));
			if (kauDevice == null)
				return null;
			var alsMeasure = Watcher.GetKAUMeasure(kauDevice, alsDevice.IntAddress).FirstOrDefault();
			if (alsMeasure == null)
				return OperationResult<CurrentConsumption>.FromError("Не удалось получить значение тока");
			return new OperationResult<CurrentConsumption>(alsMeasure);
		}

		public static void GKOpenEnterTurnstile(Guid doorUid, string userName)
		{
			var door = GKManager.Doors.FirstOrDefault(x => x.UID == doorUid);
			if (door == null)
				return;
			var lockDelay = door.LockDelay;
			if (lockDelay == null)
				return;
			Watcher.SendControlCommand(lockDelay, GKStateBit.TurnOnNow_InAutomatic, "Включить для режима автоматика");
			AddGKMessage(JournalEventNameType.Команда_оператора, JournalEventDescriptionType.Открыть_турникет_на_вход, "", door, userName);
		}

		public static void GKOpenExitTurnstile(Guid doorUid, string userName)
		{
			var door = GKManager.Doors.FirstOrDefault(x => x.UID == doorUid);
			if (door == null)
				return;
			var lockDelayExit = door.LockDelayExit;
			if (lockDelayExit == null)
				return;
			Watcher.SendControlCommand(lockDelayExit, GKStateBit.TurnOnNow_InAutomatic, "Включить для режима автоматика");
			AddGKMessage(JournalEventNameType.Команда_оператора, JournalEventDescriptionType.Открыть_турникет_на_выход, "", door, userName);
		}

		public static void GKSetOpenEnterTurnstile(Guid doorUid, string userName)
		{
			var door = GKManager.Doors.FirstOrDefault(x => x.UID == doorUid);
			if (door == null)
				return;
			var lockDelay = door.LockDelay;
			if (lockDelay == null)
				return;
			var resetDelay = door.ResetDelay;
			if (resetDelay == null)
				return;
			Watcher.SendControlCommand(lockDelay, GKStateBit.TurnOnNow_InAutomatic, "Включить для режима автоматика");
			Watcher.SendControlCommand(resetDelay, GKStateBit.TurnOnNow_InAutomatic, "Включить для режима автоматика");
			Watcher.SendControlCommand(door, GKStateBit.SetRegime_Manual, "Перевод в ручной режим");
			Watcher.SendControlCommand(door, GKStateBit.TurnOnNow_InManual, "Включить немедленно");
			AddGKMessage(JournalEventNameType.Команда_оператора, JournalEventDescriptionType.Перевести_турникет_в_режим_всегда_открыто_на_вход, "", door, userName);
		}

		public static void GKSetOpenExitTurnstile(Guid doorUid, string userName)
		{
			var door = GKManager.Doors.FirstOrDefault(x => x.UID == doorUid);
			if (door == null)
				return;
			var lockDelayExit = door.LockDelayExit;
			if (lockDelayExit == null)
				return;
			var resetDelay = door.ResetDelay;
			if (resetDelay == null)
				return;
			Watcher.SendControlCommand(lockDelayExit, GKStateBit.TurnOnNow_InAutomatic, "Включить для режима автоматика");
			Watcher.SendControlCommand(resetDelay, GKStateBit.TurnOnNow_InAutomatic, "Включить для режима автоматика");
			Watcher.SendControlCommand(door, GKStateBit.SetRegime_Manual, "Перевод в ручной режим");
			Watcher.SendControlCommand(door, GKStateBit.TurnOnNow_InManual, "Включить немедленно");
			AddGKMessage(JournalEventNameType.Команда_оператора, JournalEventDescriptionType.Перевести_турникет_в_режим_всегда_открыто_на_выход, "", door, userName);
		}

		public static void GKSetNormTurnstile(Guid doorUid, string userName)
		{
			var door = GKManager.Doors.FirstOrDefault(x => x.UID == doorUid);
			if (door == null)
				return;
			var resetDelay = door.ResetDelay;
			if (resetDelay == null)
				return;
			Watcher.SendControlCommand(resetDelay, GKStateBit.TurnOnNow_InAutomatic, "Включить для режима автоматика");
			Watcher.SendControlCommand(door, GKStateBit.SetRegime_Automatic, "Перевод в автоматический режим");
			Watcher.SendControlCommand(door, GKStateBit.TurnOff_InAutomatic, "Выключить в автоматике");
			AddGKMessage(JournalEventNameType.Команда_оператора, JournalEventDescriptionType.Перевести_турникет_в_норму, "", door, userName);
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
				if (gkBase is GKSKDZone)
				{
					journalObjectType = JournalObjectType.GKSKDZone;
				}
				if (gkBase is GKDoor)
				{
					journalObjectType = JournalObjectType.GKDoor;
				}
				if (gkBase is GKMPT)
				{
					journalObjectType = JournalObjectType.GKMPT;
				}
				if (gkBase is GKPumpStation)
				{
					journalObjectType = JournalObjectType.GKPumpStation;
				}
			}

			var journalItem = new JournalItem
			{
				SystemDateTime = DateTime.Now,
				DeviceDateTime = null,
				JournalObjectType = journalObjectType,
				JournalEventNameType = journalEventNameType,
				JournalEventDescriptionType = journalEventDescriptionType,
				DescriptionText = description,
				ObjectUID = uid,
				UserName = userName,
				JournalSubsystemType = JournalSubsystemType.GK
			};
			if (gkBase != null)
			{
				journalItem.ObjectName = gkBase.PresentationName;
				var gkObjectNo = gkBase.GKDescriptorNo;
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