using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common;
using Infrastructure.Common;
using Ionic.Zip;
using FiresecAPI;
using System.Text;
using FiresecAPI.Models;
using XFiresecAPI;
using System;
using GKProcessor;
using FiresecClient;

namespace FiresecService.Service
{
	public partial class FiresecService
	{
		string UserName
		{
			get { return CurrentClientCredentials.FriendlyUserName; }
		}

		public void CancelGKProgress()
		{
			GKProcessorManager.CancelGKProgress();
		}

		public void AddJournalItem(JournalItem journalItem)
		{
			GKDBHelper.Add(journalItem);
			var gkCallbackResult = new GKCallbackResult();
			gkCallbackResult.JournalItems.Add(journalItem);
			NotifyGKObjectStateChanged(gkCallbackResult);
		}

		public OperationResult<bool> GKWriteConfiguration(Guid deviceUID, bool writeFileToGK)
		{
			var device = XManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				return GKProcessorManager.GKWriteConfiguration(device, writeFileToGK, UserName);
			}
			else
			{
				return new OperationResult<bool>("Не найдено устройство в конфигурации");
			}
		}

		public OperationResult<XDeviceConfiguration> GKReadConfiguration(Guid deviceUID)
		{
			var device = XManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				DescriptorsManager.Create();
				return GKProcessorManager.GKReadConfiguration(device, UserName);
			}
			else
			{
				return new OperationResult<XDeviceConfiguration>("Не найдено устройство в конфигурации");
			}
		}

		public OperationResult<XDeviceConfiguration> GKReadConfigurationFromGKFile(Guid deviceUID)
		{
			var device = XManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				DescriptorsManager.Create();
				return GKProcessorManager.GKReadConfigurationFromGKFile(device, UserName);
			}
			else
			{
				return new OperationResult<XDeviceConfiguration>("Не найдено устройство в конфигурации");
			}
		}

		public OperationResult<bool> GKUpdateFirmware(Guid deviceUID, string fileName)
		{
			var device = XManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				return GKProcessorManager.GKUpdateFirmware(device, fileName, UserName);
			}
			else
			{
				return new OperationResult<bool>("Не найдено устройство в конфигурации");
			}
		}

		public OperationResult<bool> GKUpdateFirmwareFSCS(HexFileCollectionInfo hxcFileInfo, string userName, List<Guid> deviceUIDs)
		{
			var devices = new List<XDevice>();
			foreach (var deviceUID in deviceUIDs)
			{
				var device = XManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
				if (device == null)
				{
					return new OperationResult<bool>("Не найдено устройство в конфигурации");
				}
				devices.Add(device);
			}
			return GKProcessorManager.GKUpdateFirmwareFSCS(hxcFileInfo, userName, devices);
		}

		public bool GKSyncronyseTime(Guid deviceUID)
		{
			var device = XManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				return GKProcessorManager.GKSyncronyseTime(device, UserName);
			}
			else
			{
				return false;
			}
		}

		public string GKGetDeviceInfo(Guid deviceUID)
		{
			var device = XManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				return GKProcessorManager.GKGetDeviceInfo(device, UserName);
			}
			else
			{
				return null;
			}
		}

		public OperationResult<int> GKGetJournalItemsCount(Guid deviceUID)
		{
			var device = XManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				return GKProcessorManager.GKGetJournalItemsCount(device);
			}
			else
			{
				return new OperationResult<int>("Не найдено устройство в конфигурации");
			}
		}

		public OperationResult<JournalItem> GKReadJournalItem(Guid deviceUID, int no)
		{
			var device = XManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				return GKProcessorManager.GKReadJournalItem(device, no);
			}
			else
			{
				return new OperationResult<JournalItem>("Не найдено устройство в конфигурации");
			}
		}

		public OperationResult<bool> GKSetSingleParameter(Guid objectUID, List<byte> parameterBytes)
		{
			XBase xBase = null;
			xBase = XManager.Devices.FirstOrDefault(x => x.UID == objectUID);
			if (xBase == null)
			{
				xBase = XManager.Directions.FirstOrDefault(x => x.UID == objectUID);
			}

			if (xBase != null)
			{
				return GKProcessorManager.GKSetSingleParameter(xBase, parameterBytes);
			}
			else
			{
				return new OperationResult<bool>("Не найден компонент в конфигурации");
			}
		}

		public OperationResult<List<XProperty>> GKGetSingleParameter(Guid objectUID)
		{
			XBase xBase = null;
			xBase = XManager.Devices.FirstOrDefault(x => x.UID == objectUID);
			if (xBase == null)
			{
				xBase = XManager.Directions.FirstOrDefault(x => x.UID == objectUID);
			}

			if (xBase != null)
			{
				return GKProcessorManager.GKGetSingleParameter(xBase);
			}
			else
			{
				return new OperationResult<List<XProperty>>("Не найден компонент в конфигурации");
			}
		}

		public OperationResult<List<byte>> GKGKHash(Guid gkDeviceUID)
		{
			var device = XManager.Devices.FirstOrDefault(x => x.UID == gkDeviceUID);
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

		public void GKExecuteDeviceCommand(Guid deviceUID, XStateBit stateBit)
		{
			var device = XManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				GKProcessorManager.GKExecuteDeviceCommand(device, stateBit, UserName);
			}
		}

		public void GKReset(Guid uid, XBaseObjectType objectType)
		{
			var xBase = GetXBase(uid, objectType);
			if (xBase != null)
			{
				GKProcessorManager.GKReset(xBase, UserName);
			}
		}

		public void GKResetFire1(Guid zoneUID)
		{
			var zone = XManager.Zones.FirstOrDefault(x => x.UID == zoneUID);
			if (zone != null)
			{
				GKProcessorManager.GKResetFire1(zone, UserName);
			}
		}

		public void GKResetFire2(Guid zoneUID)
		{
			var zone = XManager.Zones.FirstOrDefault(x => x.UID == zoneUID);
			if (zone != null)
			{
				GKProcessorManager.GKResetFire2(zone, UserName);
			}
		}

		public void GKSetAutomaticRegime(Guid uid, XBaseObjectType objectType)
		{
			var xBase = GetXBase(uid, objectType);
			if (xBase != null)
			{
				GKProcessorManager.GKSetAutomaticRegime(xBase, UserName);
			}
		}

		public void GKSetManualRegime(Guid uid, XBaseObjectType objectType)
		{
			var xBase = GetXBase(uid, objectType);
			if (xBase != null)
			{
				GKProcessorManager.GKSetManualRegime(xBase, UserName);
			}
		}

		public void GKSetIgnoreRegime(Guid uid, XBaseObjectType objectType)
		{
			var xBase = GetXBase(uid, objectType);
			if (xBase != null)
			{
				GKProcessorManager.GKSetIgnoreRegime(xBase, UserName);
			}
		}

		public void GKTurnOn(Guid uid, XBaseObjectType objectType)
		{
			var xBase = GetXBase(uid, objectType);
			if (xBase != null)
			{
				GKProcessorManager.GKTurnOn(xBase, UserName);
			}
		}

		public void GKTurnOnNow(Guid uid, XBaseObjectType objectType)
		{
			var xBase = GetXBase(uid, objectType);
			if (xBase != null)
			{
				GKProcessorManager.GKTurnOnNow(xBase, UserName);
			}
		}

		public void GKTurnOff(Guid uid, XBaseObjectType objectType)
		{
			var xBase = GetXBase(uid, objectType);
			if (xBase != null)
			{
				GKProcessorManager.GKTurnOff(xBase, UserName);
			}
		}

		public void GKTurnOffNow(Guid uid, XBaseObjectType objectType)
		{
			var xBase = GetXBase(uid, objectType);
			if (xBase != null)
			{
				GKProcessorManager.GKTurnOffNow(xBase, UserName);
			}
		}

		public void GKStop(Guid uid, XBaseObjectType objectType)
		{
			var xBase = GetXBase(uid, objectType);
			if (xBase != null)
			{
				GKProcessorManager.GKStop(xBase, UserName);
			}
		}

		XBase GetXBase(Guid uid, XBaseObjectType objectType)
		{
			switch (objectType)
			{
				case XBaseObjectType.Deivce:
					return XManager.Devices.FirstOrDefault(x => x.UID == uid);
				case XBaseObjectType.Direction:
					return XManager.Directions.FirstOrDefault(x => x.UID == uid);
				case XBaseObjectType.Zone:
					return XManager.Zones.FirstOrDefault(x => x.UID == uid);
			}
			return null;
		}

		public void GKStartMeasureMonitoring(Guid deviceUID)
		{
			var device = XManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				GKProcessorManager.GKStartMeasureMonitoring(device);
			}
		}

		public void GKStopMeasureMonitoring(Guid deviceUID)
		{
			var device = XManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				GKProcessorManager.GKStopMeasureMonitoring(device);
			}
		}
	}
}