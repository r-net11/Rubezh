using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecClient;
using GKProcessor;
using FiresecAPI.Journal;

namespace FiresecService.Service
{
	public partial class FiresecService
	{
		string UserName
		{
			get { return CurrentClientCredentials != null ? CurrentClientCredentials.FriendlyUserName : "<Нет>"; }
		}

		public void CancelGKProgress(Guid progressCallbackUID, string userName)
		{
			GKProcessorManager.CancelGKProgress(progressCallbackUID, userName);
			ChinaSKDDriver.Processor.CancelGKProgress(progressCallbackUID, userName);
		}

		public OperationResult<bool> GKWriteConfiguration(Guid deviceUID)
		{
			var device = GKManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				return GKProcessorManager.GKWriteConfiguration(device, UserName);
			}
			else
			{
				return new OperationResult<bool>("Не найдено устройство в конфигурации. Предварительно необходимо применить конфигурацию");
			}
		}

		public OperationResult<GKDeviceConfiguration> GKReadConfiguration(Guid deviceUID)
		{
			var device = GKManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				DescriptorsManager.Create();
				return GKProcessorManager.GKReadConfiguration(device, UserName);
			}
			else
			{
				return new OperationResult<GKDeviceConfiguration>("Не найдено устройство в конфигурации. Предварительно необходимо применить конфигурацию");
			}
		}

		public OperationResult<GKDeviceConfiguration> GKReadConfigurationFromGKFile(Guid deviceUID)
		{
			var device = GKManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				DescriptorsManager.Create();
				return GKProcessorManager.GKReadConfigurationFromGKFile(device, UserName);
			}
			else
			{
				return new OperationResult<GKDeviceConfiguration>("Не найдено устройство в конфигурации. Предварительно необходимо применить конфигурацию");
			}
		}

		public OperationResult<bool> GKUpdateFirmware(Guid deviceUID, string fileName)
		{
			var device = GKManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				return GKProcessorManager.GKUpdateFirmware(device, fileName, UserName);
			}
			else
			{
				return new OperationResult<bool>("Не найдено устройство в конфигурации. Предварительно необходимо применить конфигурацию");
			}
		}

		public OperationResult<bool> GKUpdateFirmwareFSCS(HexFileCollectionInfo hxcFileInfo, string userName, List<Guid> deviceUIDs)
		{
			var devices = new List<GKDevice>();
			foreach (var deviceUID in deviceUIDs)
			{
				var device = GKManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
				if (device == null)
				{
					return new OperationResult<bool>("Не найдено устройство в конфигурации. Предварительно необходимо применить конфигурацию");
				}
				devices.Add(device);
			}
			return GKProcessorManager.GKUpdateFirmwareFSCS(hxcFileInfo, userName, devices);
		}

		public OperationResult<bool> GKSyncronyseTime(Guid deviceUID)
		{
			var device = GKManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				var result = GKProcessorManager.GKSyncronyseTime(device, UserName);
				if (result)
					return new OperationResult<bool>() { Result = true };
				else
					return new OperationResult<bool>("Устройство недоступно") { Result = false };
			}
			else
			{
				return new OperationResult<bool>("Не найдено устройство в конфигурации. Предварительно необходимо применить конфигурацию");
			}
		}

		public OperationResult<string> GKGetDeviceInfo(Guid deviceUID)
		{
			var device = GKManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				return new OperationResult<string>() { Result = GKProcessorManager.GKGetDeviceInfo(device, UserName) };
			}
			else
			{
				return new OperationResult<string>("Не найдено устройство в конфигурации. Предварительно необходимо применить конфигурацию");
			}
		}

		public OperationResult<int> GKGetJournalItemsCount(Guid deviceUID)
		{
			var device = GKManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				return GKProcessorManager.GKGetJournalItemsCount(device);
			}
			else
			{
				return new OperationResult<int>("Не найдено устройство в конфигурации. Предварительно необходимо применить конфигурацию");
			}
		}

		public OperationResult<JournalItem> GKReadJournalItem(Guid deviceUID, int no)
		{
			var device = GKManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				return GKProcessorManager.GKReadJournalItem(device, no);
			}
			else
			{
				return new OperationResult<JournalItem>("Не найдено устройство в конфигурации. Предварительно необходимо применить конфигурацию");
			}
		}

		public OperationResult<bool> GKSetSingleParameter(Guid objectUID, List<byte> parameterBytes)
		{
			GKBase gkBase = null;
			gkBase = GKManager.Devices.FirstOrDefault(x => x.UID == objectUID);
			if (gkBase == null)
			{
				gkBase = GKManager.Directions.FirstOrDefault(x => x.UID == objectUID);
			}
			if (gkBase == null)
			{
				gkBase = GKManager.DeviceConfiguration.Codes.FirstOrDefault(x => x.UID == objectUID);
			}

			if (gkBase != null)
			{
				return GKProcessorManager.GKSetSingleParameter(gkBase, parameterBytes);
			}
			else
			{
				return new OperationResult<bool>("Не найден компонент в конфигурации");
			}
		}

		public OperationResult<List<GKProperty>> GKGetSingleParameter(Guid objectUID)
		{
			GKBase gkBase = null;
			gkBase = GKManager.Devices.FirstOrDefault(x => x.UID == objectUID);
			if (gkBase == null)
			{
				gkBase = GKManager.Directions.FirstOrDefault(x => x.UID == objectUID);
			}
			if (gkBase == null)
			{
				gkBase = GKManager.DeviceConfiguration.Codes.FirstOrDefault(x => x.UID == objectUID);
			}

			if (gkBase != null)
			{
				return GKProcessorManager.GKGetSingleParameter(gkBase);
			}
			else
			{
				return new OperationResult<List<GKProperty>>("Не найден компонент в конфигурации");
			}
		}

		public OperationResult<bool> GKSetSchedule(GKSchedule schedule)
		{
			var gkControllerDevice = GKManager.Devices.FirstOrDefault(x => x.DriverType == GKDriverType.GK);
			if (gkControllerDevice != null)
			{
				return GKProcessorManager.GKSetSchedule(gkControllerDevice, schedule);
			}
			else
			{
				return new OperationResult<bool>("Не найден ГК в конфигурации");
			}
		}

		public OperationResult<List<byte>> GKGKHash(Guid gkDeviceUID)
		{
			var device = GKManager.Devices.FirstOrDefault(x => x.UID == gkDeviceUID);
			if (device != null)
			{
				return GKProcessorManager.GKGKHash(device);
			}
			else
			{
				return new OperationResult<List<byte>>("Не найдено устройство в конфигурации");
			}
		}

		public GKStates GKGetStates()
		{
			return GKProcessorManager.GKGetStates();
		}

		public void GKExecuteDeviceCommand(Guid deviceUID, GKStateBit stateBit)
		{
			var device = GKManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				GKProcessorManager.GKExecuteDeviceCommand(device, stateBit, UserName);
			}
		}

		public void GKReset(Guid uid, GKBaseObjectType objectType)
		{
			var gkBase = GetXBase(uid, objectType);
			if (gkBase != null)
			{
				GKProcessorManager.GKReset(gkBase, UserName);
			}
		}

		public void GKResetFire1(Guid zoneUID)
		{
			var zone = GKManager.Zones.FirstOrDefault(x => x.UID == zoneUID);
			if (zone != null)
			{
				GKProcessorManager.GKResetFire1(zone, UserName);
			}
		}

		public void GKResetFire2(Guid zoneUID)
		{
			var zone = GKManager.Zones.FirstOrDefault(x => x.UID == zoneUID);
			if (zone != null)
			{
				GKProcessorManager.GKResetFire2(zone, UserName);
			}
		}

		public void GKSetAutomaticRegime(Guid uid, GKBaseObjectType objectType)
		{
			var xBase = GetXBase(uid, objectType);
			if (xBase != null)
			{
				GKProcessorManager.GKSetAutomaticRegime(xBase, UserName);
			}
		}

		public void GKSetManualRegime(Guid uid, GKBaseObjectType objectType)
		{
			var xBase = GetXBase(uid, objectType);
			if (xBase != null)
			{
				GKProcessorManager.GKSetManualRegime(xBase, UserName);
			}
		}

		public void GKSetIgnoreRegime(Guid uid, GKBaseObjectType objectType)
		{
			var xBase = GetXBase(uid, objectType);
			if (xBase != null)
			{
				GKProcessorManager.GKSetIgnoreRegime(xBase, UserName);
			}
		}

		public void GKTurnOn(Guid uid, GKBaseObjectType objectType)
		{
			var xBase = GetXBase(uid, objectType);
			if (xBase != null)
			{
				GKProcessorManager.GKTurnOn(xBase, UserName);
			}
		}

		public void GKTurnOnNow(Guid uid, GKBaseObjectType objectType)
		{
			var xBase = GetXBase(uid, objectType);
			if (xBase != null)
			{
				GKProcessorManager.GKTurnOnNow(xBase, UserName);
			}
		}

		public void GKTurnOff(Guid uid, GKBaseObjectType objectType)
		{
			var xBase = GetXBase(uid, objectType);
			if (xBase != null)
			{
				GKProcessorManager.GKTurnOff(xBase, UserName);
			}
		}

		public void GKTurnOffNow(Guid uid, GKBaseObjectType objectType)
		{
			var xBase = GetXBase(uid, objectType);
			if (xBase != null)
			{
				GKProcessorManager.GKTurnOffNow(xBase, UserName);
			}
		}

		public void GKStop(Guid uid, GKBaseObjectType objectType)
		{
			var xBase = GetXBase(uid, objectType);
			if (xBase != null)
			{
				GKProcessorManager.GKStop(xBase, UserName);
			}
		}

		GKBase GetXBase(Guid uid, GKBaseObjectType objectType)
		{
			switch (objectType)
			{
				case GKBaseObjectType.Deivce:
					return GKManager.Devices.FirstOrDefault(x => x.UID == uid);
				case GKBaseObjectType.Direction:
					return GKManager.Directions.FirstOrDefault(x => x.UID == uid);
				case GKBaseObjectType.Zone:
					return GKManager.Zones.FirstOrDefault(x => x.UID == uid);
				case GKBaseObjectType.PumpStation:
					return GKManager.PumpStations.FirstOrDefault(x => x.UID == uid);
				case GKBaseObjectType.MPT:
					return GKManager.MPTs.FirstOrDefault(x => x.UID == uid);
				case GKBaseObjectType.Pim:
					return GKManager.AutoGeneratedPims.FirstOrDefault(x => x.UID == uid);
				case GKBaseObjectType.Delay:
					return GKManager.AutoGeneratedDelays.FirstOrDefault(x => x.UID == uid);
				case GKBaseObjectType.GuardZone:
					return GKManager.GuardZones.FirstOrDefault(x => x.UID == uid);
			}
			return null;
		}

		public void GKStartMeasureMonitoring(Guid deviceUID)
		{
			var device = GKManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				GKProcessorManager.GKStartMeasureMonitoring(device);
			}
		}

		public void GKStopMeasureMonitoring(Guid deviceUID)
		{
			var device = GKManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				GKProcessorManager.GKStopMeasureMonitoring(device);
			}
		}

		#region Users
		public OperationResult<bool> GKAddUser(Guid deviceUID)
		{
			var device = GKManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				var result = GKProcessorManager.GKAddUser(device, UserName);
				if (result)
					return new OperationResult<bool>() { Result = true };
				else
					return new OperationResult<bool>("Устройство недоступно") { Result = false };
			}
			else
			{
				return new OperationResult<bool>("Не найдено устройство в конфигурации");
			}
		}
		#endregion

		#region Journal
		public void AddXJournalItem(JournalItem journalItem)
		{
			GKDBHelper.Add(journalItem);
			AddGKJournalItem(journalItem);
			var gkCallbackResult = new GKCallbackResult();
			gkCallbackResult.JournalItems.Add(journalItem);
			NotifyGKObjectStateChanged(gkCallbackResult);
		}
		#endregion
	}
}