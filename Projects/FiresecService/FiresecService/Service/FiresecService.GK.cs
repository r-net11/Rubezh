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
		public void AddJournalItem(JournalItem journalItem)
		{
			GKDBHelper.Add(journalItem);
			NotifyNewGKJournal(new List<JournalItem>() { journalItem });
		}

		public void GKWriteConfiguration(Guid deviceUID, bool writeFileToGK = false)
		{
			var device = XManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				GkDescriptorsWriter.WriteConfig(device);
				AddMessage("Запись конфигурации в прибор", "", XStateClass.Info, device);
			}
		}

		public OperationResult<XDeviceConfiguration> GKReadConfiguration(Guid deviceUID)
		{
			var device = XManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				return GKProcessorManager.GKReadConfiguration(device);
			}
			else
			{
				return new OperationResult<XDeviceConfiguration>("Не найдено устройство в конфигурации");
			}
		}

		public void GKUpdateFirmware(Guid deviceUID, string fileName)
		{
			var device = XManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				GKProcessorManager.GKUpdateFirmware(device, fileName);
			}
		}

		public bool GKSyncronyseTime(Guid deviceUID)
		{
			var device = XManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				return GKProcessorManager.GKSyncronyseTime(device);
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
				return GKProcessorManager.GKGetDeviceInfo(device);
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

		public OperationResult<bool> GKSetSingleParameter(Guid deviceUID)
		{
			var device = XManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				return GKProcessorManager.GKSetSingleParameter(device);
			}
			else
			{
				return new OperationResult<bool>("Не найдено устройство в конфигурации");
			}
		}

		public OperationResult<bool> GKGetSingleParameter(Guid deviceUID)
		{
			var device = XManager.Devices.FirstOrDefault(x => x.UID == deviceUID);
			if (device != null)
			{
				return GKProcessorManager.GKGetSingleParameter(device);
			}
			else
			{
				return new OperationResult<bool>("Не найдено устройство в конфигурации");
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
				GKProcessorManager.GKExecuteDeviceCommand(device, stateBit);
			}
		}

		public void GKReset(Guid uid, XBaseObjectType objectType)
		{
			var xBase = GetXBase(uid, objectType);
			if (xBase != null)
			{
				GKProcessorManager.GKReset(xBase);
			}
		}

		public void GKResetFire1(Guid zoneUID)
		{
			var zone = XManager.Zones.FirstOrDefault(x => x.UID == zoneUID);
			if (zone != null)
			{
				GKProcessorManager.GKResetFire1(zone);
			}
		}

		public void GKResetFire2(Guid zoneUID)
		{
			var zone = XManager.Zones.FirstOrDefault(x => x.UID == zoneUID);
			if (zone != null)
			{
				GKProcessorManager.GKResetFire2(zone);
			}
		}

		public void GKSetAutomaticRegime(Guid uid, XBaseObjectType objectType)
		{
			var xBase = GetXBase(uid, objectType);
			if (xBase != null)
			{
				GKProcessorManager.GKSetAutomaticRegime(xBase);
			}
		}

		public void GKSetManualRegime(Guid uid, XBaseObjectType objectType)
		{
			var xBase = GetXBase(uid, objectType);
			if (xBase != null)
			{
				GKProcessorManager.GKSetManualRegime(xBase);
			}
		}

		public void GKSetIgnoreRegime(Guid uid, XBaseObjectType objectType)
		{
			var xBase = GetXBase(uid, objectType);
			if (xBase != null)
			{
				GKProcessorManager.GKSetIgnoreRegime(xBase);
			}
		}

		public void GKTurnOn(Guid uid, XBaseObjectType objectType)
		{
			var xBase = GetXBase(uid, objectType);
			if (xBase != null)
			{
				GKProcessorManager.GKTurnOn(xBase);
			}
		}

		public void GKTurnOnNow(Guid uid, XBaseObjectType objectType)
		{
			var xBase = GetXBase(uid, objectType);
			if (xBase != null)
			{
				GKProcessorManager.GKTurnOnNow(xBase);
			}
		}

		public void GKTurnOff(Guid uid, XBaseObjectType objectType)
		{
			var xBase = GetXBase(uid, objectType);
			if (xBase != null)
			{
				GKProcessorManager.GKTurnOff(xBase);
			}
		}

		public void GKTurnOffNow(Guid uid, XBaseObjectType objectType)
		{
			var xBase = GetXBase(uid, objectType);
			if (xBase != null)
			{
				GKProcessorManager.GKTurnOffNow(xBase);
			}
		}

		public void GKStop(Guid uid, XBaseObjectType objectType)
		{
			var xBase = GetXBase(uid, objectType);
			if (xBase != null)
			{
				GKProcessorManager.GKStop(xBase);
			}
		}

		public void AddMessage(string message, string description, XStateClass stateClass, XBase xBase)
		{
			Guid uid = Guid.Empty;
			JournalItemType journalItemType = JournalItemType.System;
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

			var journalItem = new JournalItem()
			{
				SystemDateTime = DateTime.Now,
				DeviceDateTime = DateTime.Now,
				JournalItemType = journalItemType,
				StateClass = stateClass,
				Name = message,
				Description = description,
				ObjectUID = uid,
				ObjectName = xBase.PresentationName,
				ObjectStateClass = XStateClass.Norm,
				GKObjectNo = (ushort)xBase.GKDescriptorNo,
				UserName = CurrentClientCredentials.UserName,
				SubsystemType = XSubsystemType.System
			};

			AddJournalItem(journalItem);
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
	}
}