using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecAPI.SKD;
using FiresecClient;
using FiresecAPI.Journal;
using System.IO;

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
			OnGKCallbackResult(gkProgressCallback);
			return gkProgressCallback;
		}

		public static void DoProgress(string text, GKProgressCallback progressCallback)
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
			OnGKCallbackResult(gkProgressCallback);
		}

		public static void StopProgress(GKProgressCallback progressCallback)
		{
			var gkProgressCallback = new GKProgressCallback
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
		public static bool MustMonitor = false;

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

			return new OperationResult<bool>(true);
		}

		public static Stream GKReadConfigurationFromGKFile(GKDevice device, string userName)
		{
			AddGKMessage(JournalEventNameType.Чтение_конфигурации_из_прибора, JournalEventDescriptionType.NULL, "", device, userName);
			var gkFileReaderWriter = new GKFileReaderWriter();
			var filePath = gkFileReaderWriter.ReadConfigFileFromGK(device);
			if (filePath != null)
			{
				return new FileStream(filePath, FileMode.Open, FileAccess.Read);
			}
			return Stream.Null;
			//return OperationResult<Stream>.FromError { gkFileReaderWriter.Error, new FileStream(filePath, FileMode.Open, FileAccess.Read));
		}

		public static OperationResult<GKDeviceConfiguration> GKAutoSearch(GKDevice device, string userName)
		{
			AddGKMessage(JournalEventNameType.Автопоиск, JournalEventDescriptionType.NULL, "", device, userName);
			var gkAutoSearchHelper = new GKAutoSearchHelper();
			var deviceConfiguration = gkAutoSearchHelper.AutoSearch(device);
			return OperationResult<GKDeviceConfiguration>.FromError(gkAutoSearchHelper.Error, deviceConfiguration);
		}

		public static OperationResult<bool> GKUpdateFirmware(GKDevice device, string fileName, string userName)
		{
			var firmwareUpdateHelper = new FirmwareUpdateHelper();
			firmwareUpdateHelper.Update(device, fileName, userName);
			if (firmwareUpdateHelper.ErrorList.Count > 0)
				return OperationResult<bool>.FromError(firmwareUpdateHelper.ErrorList, false);
			return new OperationResult<bool>(true);
		}

		//public static void GKOpenSKDZone(GKSKDZone zone) TODO: Change to SKD
		//{
		//	foreach (var door in GKManager.Doors)
		//	{
		//		if (door.EnterZoneUID == zone.UID)
		//		{
		//			GKProcessorManager.AddGKMessage(JournalEventNameType.Открытие_зоны_СКД, JournalEventDescriptionType.NULL, "", zone, null);
		//		}
		//	}
		//}

		//public static void GKCloseSKDZone(GKSKDZone zone)
		//{
		//	foreach (var door in GKManager.Doors)
		//	{
		//		if (door.EnterZoneUID == zone.UID)
		//		{
		//			GKProcessorManager.AddGKMessage(JournalEventNameType.Закрытие_зоны_СКД, JournalEventDescriptionType.NULL, "", zone, null);
		//		}
		//	}
		//}

		public static OperationResult<bool> GKUpdateFirmwareFSCS(HexFileCollectionInfo hxcFileInfo, string userName, List<GKDevice> devices)
		{
			var firmwareUpdateHelper = new FirmwareUpdateHelper();
			firmwareUpdateHelper.UpdateFSCS(hxcFileInfo, devices, userName);
			if (firmwareUpdateHelper.ErrorList.Count > 0)
				return OperationResult<bool>.FromError(firmwareUpdateHelper.ErrorList, false);
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

			foreach (var skdZone in GKManager.SKDZones)
			{
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
			zone.State.StateClasses = stateClasses.ToList();
			zone.State.StateClass = GKStatesHelper.GetMinStateClass(zone.State.StateClasses);
		}

		public static void GKExecuteDeviceCommand(GKDevice device, GKStateBit stateBit, string userName)
		{
			AddGKMessage(JournalEventNameType.Команда_оператора, JournalEventDescriptionType.NULL, stateBit.ToDescription(), device, userName);
		}

		public static void GKReset(GKBase gkBase, string userName)
		{
			AddGKMessage(JournalEventNameType.Команда_оператора, JournalEventDescriptionType.Сброс, "", gkBase, userName);
		}

		public static void GKSetAutomaticRegime(GKBase gkBase, string userName)
		{
			AddGKMessage(JournalEventNameType.Команда_оператора, JournalEventDescriptionType.Перевод_в_автоматический_режим, "", gkBase, userName);
		}

		public static void GKSetManualRegime(GKBase gkBase, string userName)
		{
			AddGKMessage(JournalEventNameType.Команда_оператора, JournalEventDescriptionType.Перевод_в_ручной_режим, "", gkBase, userName);
		}

		public static void GKSetIgnoreRegime(GKBase gkBase, string userName)
		{
			AddGKMessage(JournalEventNameType.Команда_оператора, JournalEventDescriptionType.Перевод_в_отключенный_режим, "", gkBase, userName);
		}

		public static void GKTurnOn(GKBase gkBase, string userName)
		{
			AddGKMessage(JournalEventNameType.Команда_оператора, JournalEventDescriptionType.Включить, "", gkBase, userName);
		}

		public static void GKTurnOnNow(GKBase gkBase, string userName)
		{
			AddGKMessage(JournalEventNameType.Команда_оператора, JournalEventDescriptionType.Включить_немедленно, "", gkBase, userName);
		}

		public static void GKTurnOnInAutomatic(GKBase gkBase, string userName)
		{
			AddGKMessage(JournalEventNameType.Команда_оператора, JournalEventDescriptionType.Включить_в_автоматическом_режиме, "", gkBase, userName);
		}

		public static void GKTurnOnNowInAutomatic(GKBase gkBase, string userName)
		{
			AddGKMessage(JournalEventNameType.Команда_оператора, JournalEventDescriptionType.Включить_немедленно_в_автоматическом_режиме, "", gkBase, userName);
		}

		public static void GKTurnOff(GKBase gkBase, string userName)
		{
			AddGKMessage(JournalEventNameType.Команда_оператора, JournalEventDescriptionType.Выключить, "", gkBase, userName);
		}

		public static void GKTurnOffNow(GKBase gkBase, string userName)
		{
			AddGKMessage(JournalEventNameType.Команда_оператора, JournalEventDescriptionType.Выключить_немедленно, "", gkBase, userName);
		}

		public static void GKTurnOffInAutomatic(GKBase gkBase, string userName)
		{
			AddGKMessage(JournalEventNameType.Команда_оператора, JournalEventDescriptionType.Выключить_в_автоматическом_режиме, "", gkBase, userName);
		}

		public static void GKTurnOffNowInAutomatic(GKBase gkBase, string userName)
		{
			AddGKMessage(JournalEventNameType.Команда_оператора, JournalEventDescriptionType.Выключить_немедленно_в_автоматическом_режиме, "", gkBase, userName);
		}

		public static void GKStop(GKBase gkBase, string userName)
		{
			AddGKMessage(JournalEventNameType.Команда_оператора, JournalEventDescriptionType.Остановка_пуска, "", gkBase, userName);
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
				if (gkBase is GKSKDZone)
				{
					journalObjectType = JournalObjectType.GKSKDZone;
				}
			}

			var journalItem = new JournalItem
			{
				SystemDateTime = DateTime.Now,
				DeviceDateTime = DateTime.Now,
				JournalObjectType = journalObjectType,
				JournalEventNameType = journalEventNameType,
				JournalEventDescriptionType = journalEventDescriptionType,
				DescriptionText = description,
				ObjectUID = uid,
				UserName = userName,
				JournalSubsystemType = JournalSubsystemType.System
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