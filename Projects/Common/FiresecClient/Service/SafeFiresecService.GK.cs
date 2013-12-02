using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecAPI.Models.Skud;
using GKProcessor;
using Infrastructure.Common.Windows;
using XFiresecAPI;
using Infrastructure.Common;

namespace FiresecClient
{
    public partial class SafeFiresecService
    {
		static bool IsGKAsAService = GlobalSettingsHelper.GlobalSettings.IsGKAsAService;

		public void GKWriteConfiguration(XDevice device, bool writeFileToGK = false)
		{
			if (IsGKAsAService)
			{
				SafeOperationCall(() => FiresecService.GKWriteConfiguration(device.BaseUID, writeFileToGK), "GKWriteConfiguration");
			}
			else
			{
				GKProcessorManager.AddGKMessage("Запись конфигурации в прибор", "", XStateClass.Info, device, true);
				GkDescriptorsWriter.WriteConfig(device, writeFileToGK);
				if (!String.IsNullOrEmpty(GkDescriptorsWriter.Error))
				{
					LoadingService.IsCanceled = true; 
					MessageBoxService.ShowError(GkDescriptorsWriter.Error); 
					return;
				}
				FiresecManager.FiresecService.NotifyClientsOnConfigurationChanged();
			}
		}

		public OperationResult<XDeviceConfiguration> GKReadConfiguration(XDevice device)
		{
			if (IsGKAsAService)
			{
				return SafeOperationCall(() => FiresecService.GKReadConfiguration(device.BaseUID), "GKReadConfiguration");
			}
			else
			{
				return GKProcessorManager.GKReadConfiguration(device);
			}
		}

		public void GKUpdateFirmware(XDevice device, string fileName)
		{
			if (IsGKAsAService)
			{
				SafeOperationCall(() => FiresecService.GKUpdateFirmware(device.BaseUID, fileName), "GKUpdateFirmware");
			}
			else
			{
				GKProcessorManager.GKUpdateFirmware(device, fileName);
			}
		}

		public bool GKSyncronyseTime(XDevice device)
		{
			if (IsGKAsAService)
			{
				return SafeOperationCall(() => { return FiresecService.GKSyncronyseTime(device.BaseUID); }, "GKSyncronyseTime");
			}
			else
			{
				return GKProcessorManager.GKSyncronyseTime(device);
			}
		}

		public string GKGetDeviceInfo(XDevice device)
		{
			if (IsGKAsAService)
			{
				return SafeOperationCall(() => { return FiresecService.GKGetDeviceInfo(device.BaseUID); }, "GKGetDeviceInfo");
			}
			else
			{
				return GKProcessorManager.GKGetDeviceInfo(device);
			}
		}

		public OperationResult<int> GKGetJournalItemsCount(XDevice device)
		{
			if (IsGKAsAService)
			{
				return SafeOperationCall(() => { return FiresecService.GKGetJournalItemsCount(device.BaseUID); }, "GKGetJournalItemsCount");
			}
			else
			{
				return GKProcessorManager.GKGetJournalItemsCount(device);
			}
		}

		public OperationResult<JournalItem> GKReadJournalItem(XDevice device, int no)
		{
			if (IsGKAsAService)
			{
				return SafeOperationCall(() => { return FiresecService.GKReadJournalItem(device.BaseUID, no); }, "GKReadJournalItem");
			}
			else
			{
				return GKProcessorManager.GKReadJournalItem(device, no);
			}
		}

		public OperationResult<bool> GKSetSingleParameter(XDevice device)
		{
			if (IsGKAsAService)
			{
				return SafeOperationCall<bool>(() => { return FiresecService.GKSetSingleParameter(device.BaseUID); }, "SetSingleParameter");
			}
			else
			{
				return GKProcessorManager.GKSetSingleParameter(device);
			}
		}

		public OperationResult<bool> GKGetSingleParameter(XDevice device)
		{
			if (IsGKAsAService)
			{
				return SafeOperationCall<bool>(() => { return FiresecService.GKGetSingleParameter(device.BaseUID); }, "GetSingleParameter");
			}
			else
			{
				return GKProcessorManager.GKGetSingleParameter(device);
			}
		}

		public GKStates GKGetStates()
		{
			if (IsGKAsAService)
			{
				return SafeOperationCall<GKStates>(() => { return FiresecService.GKGetStates(); }, "GKGetStates");
			}
			else
			{
				return GKProcessorManager.GKGetStates();
			}
		}

		public void GKExecuteDeviceCommand(XDevice device, XStateBit stateBit)
		{
			if (IsGKAsAService)
			{
				SafeOperationCall(() => { FiresecService.GKExecuteDeviceCommand(device.BaseUID, stateBit); }, "GKExecuteDeviceCommand");
			}
			else
			{
				GKProcessorManager.GKExecuteDeviceCommand(device, stateBit);
			}
		}

		public void GKReset(XBase xBase)
		{
			if (IsGKAsAService)
			{
				SafeOperationCall(() => { FiresecService.GKReset(xBase.BaseUID, GetObjectType(xBase)); }, "GKReset");
			}
			else
			{
				GKProcessorManager.GKReset(xBase);
			}
		}

		public void GKResetFire1(XZone zone)
		{
			if (IsGKAsAService)
			{
				SafeOperationCall(() => { FiresecService.GKResetFire1(zone.UID); }, "GKResetFire1");
			}
			else
			{
				GKProcessorManager.GKResetFire1(zone);
			}
		}

		public void GKResetFire2(XZone zone)
		{
			if (IsGKAsAService)
			{
				SafeOperationCall(() => { FiresecService.GKResetFire2(zone.UID); }, "GKResetFire2");
			}
			else
			{
				GKProcessorManager.GKResetFire2(zone);
			}
		}

		public void GKSetAutomaticRegime(XBase xBase)
		{
			if (IsGKAsAService)
			{
				SafeOperationCall(() => { FiresecService.GKSetAutomaticRegime(xBase.BaseUID, GetObjectType(xBase)); }, "GKSetAutomaticRegime");
			}
			else
			{
				GKProcessorManager.GKSetAutomaticRegime(xBase);
			}
		}

		public void GKSetManualRegime(XBase xBase)
		{
			if (IsGKAsAService)
			{
				SafeOperationCall(() => { FiresecService.GKSetManualRegime(xBase.BaseUID, GetObjectType(xBase)); }, "GKSetManualRegime");
			}
			else
			{
				GKProcessorManager.GKSetManualRegime(xBase);
			}
		}

		public void GKSetIgnoreRegime(XBase xBase)
		{
			if (IsGKAsAService)
			{
				SafeOperationCall(() => { FiresecService.GKTurnOn(xBase.BaseUID, GetObjectType(xBase)); }, "GKTurnOn");
			}
			else
			{
				GKProcessorManager.GKSetIgnoreRegime(xBase);
			}
		}

		public void GKTurnOn(XBase xBase)
		{
			if (IsGKAsAService)
			{
				SafeOperationCall(() => { FiresecService.GKTurnOn(xBase.BaseUID, GetObjectType(xBase)); }, "GKTurnOn");
			}
			else
			{
				GKProcessorManager.GKTurnOn(xBase);
			}
		}

		public void GKTurnOnNow(XBase xBase)
		{
			if (IsGKAsAService)
			{
				SafeOperationCall(() => { FiresecService.GKTurnOnNow(xBase.BaseUID, GetObjectType(xBase)); }, "GKTurnOnNow");
			}
			else
			{
				GKProcessorManager.GKTurnOnNow(xBase);
			}
		}

		public void GKTurnOff(XBase xBase)
		{
			if (IsGKAsAService)
			{
				SafeOperationCall(() => { FiresecService.GKTurnOff(xBase.BaseUID, GetObjectType(xBase)); }, "GKTurnOff");
			}
			else
			{
				GKProcessorManager.GKTurnOff(xBase);
			}
		}

		public void GKTurnOffNow(XBase xBase)
		{
			if (IsGKAsAService)
			{
				SafeOperationCall(() => { FiresecService.GKTurnOffNow(xBase.BaseUID, GetObjectType(xBase)); }, "GKTurnOffNow");
			}
			else
			{
				GKProcessorManager.GKTurnOffNow(xBase);
			}
		}

		public void GKStop(XBase xBase)
		{
			if (IsGKAsAService)
			{
				SafeOperationCall(() => { FiresecService.GKStop(xBase.BaseUID, GetObjectType(xBase)); }, "GKStop");
			}
			else
			{
				GKProcessorManager.GKStop(xBase);
			}
		}

		//public void AddGKMessage(string message, string description, XStateClass stateClass, XBase xBase, bool isAdministrator = false)
		//{
		//    Guid uid = Guid.Empty;
		//    var journalItemType = JournalItemType.System;
		//    if (xBase != null)
		//    {
		//        if (xBase is XDevice)
		//        {
		//            uid = (xBase as XDevice).UID;
		//            journalItemType = JournalItemType.Device;
		//        }
		//        if (xBase is XZone)
		//        {
		//            uid = (xBase as XZone).UID;
		//            journalItemType = JournalItemType.Zone;
		//        }
		//        if (xBase is XDirection)
		//        {
		//            uid = (xBase as XDirection).UID;
		//            journalItemType = JournalItemType.Direction;
		//        }
		//        if (xBase is XDelay)
		//        {
		//            uid = (xBase as XDelay).UID;
		//            journalItemType = JournalItemType.Delay;
		//        }
		//    }

		//    var journalItem = new JournalItem()
		//    {
		//        SystemDateTime = DateTime.Now,
		//        DeviceDateTime = DateTime.Now,
		//        JournalItemType = journalItemType,
		//        StateClass = stateClass,
		//        Name = message,
		//        Description = description,
		//        ObjectUID = uid,
		//        ObjectStateClass = XStateClass.Norm,
		//        UserName = FiresecManager.CurrentUser.Name,
		//        SubsystemType = XSubsystemType.System
		//    };
		//    if (xBase != null)
		//    {
		//        journalItem.ObjectName = xBase.PresentationName;
		//        journalItem.GKObjectNo = (ushort)xBase.GKDescriptorNo;
		//    }

		//    if (isAdministrator)
		//    {
		//        SafeOperationCall(() => { FiresecService.AddJournalItem(journalItem); }, "AddJournalItem");
		//    }
		//    else
		//    {
		//        GKDBHelper.Add(journalItem);
		//        OnNewJournalItems(journalItem);
		//    }
		//}

		public void GKAddMessage(string name, string description)
		{
			if (IsGKAsAService)
			{
			}
			else
			{
				GKProcessorManager.AddGKMessage(name, description, XStateClass.Norm, null, true);
			}
		}

		//public event Action<List<JournalItem>> NewJournalItems;
		//void OnNewJournalItems(JournalItem journalItem)
		//{
		//    var journalItems = new List<JournalItem>() { journalItem };
		//    if (NewJournalItems != null)
		//        NewJournalItems(journalItems);
		//}

		XBaseObjectType GetObjectType(XBase xBase)
		{
			if (xBase is XDevice)
				return XBaseObjectType.Deivce;
			if (xBase is XZone)
				return XBaseObjectType.Zone;
			if (xBase is XDirection)
				return XBaseObjectType.Direction;
			if (xBase is XDelay)
				return XBaseObjectType.Delay;
			if (xBase is XPim)
				return XBaseObjectType.Pim;
			return XBaseObjectType.Deivce;
		}
    }
}