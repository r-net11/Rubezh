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

		public void CancelGKProgress()
		{
			if (IsGKAsAService)
			{
				SafeOperationCall(() => FiresecService.CancelGKProgress(), "CancelGKProgress");
			}
			else
			{
				GKProcessorManager.CancelGKProgress();
			}
		}

		public OperationResult<bool> GKWriteConfiguration(XDevice device, bool writeFileToGK)
		{
			if (IsGKAsAService)
			{
				return SafeOperationCall(() => FiresecService.GKWriteConfiguration(device.BaseUID, writeFileToGK), "GKWriteConfiguration");
			}
			else
			{
				var result = GKProcessorManager.GKWriteConfiguration(device, writeFileToGK, FiresecManager.CurrentUser.Name);
				if (!result.HasError)
					FiresecManager.FiresecService.NotifyClientsOnConfigurationChanged();
				return result;
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
				return GKProcessorManager.GKReadConfiguration(device, FiresecManager.CurrentUser.Name);
			}
		}

        public OperationResult<bool> GKUpdateFirmware(XDevice device, string fileName)
		{
			if (IsGKAsAService)
			{
				return SafeOperationCall(() => FiresecService.GKUpdateFirmware(device.BaseUID, fileName), "GKUpdateFirmware");
			}
			else
			{
                return GKProcessorManager.GKUpdateFirmware(device, fileName, FiresecManager.CurrentUser.Name);
			}
		}

        public OperationResult<bool> GKUpdateFirmwareFSCS(HexFileCollectionInfo hxcFileInfo, List<XDevice> devices)
		{
			if (IsGKAsAService)
			{
                return SafeOperationCall(() => FiresecService.GKUpdateFirmwareFSCS(hxcFileInfo, FiresecManager.CurrentUser.Name, devices), "GKUpdateFirmwareFSCS");
			}
			else
			{
				return GKProcessorManager.GKUpdateFirmwareFSCS(hxcFileInfo, FiresecManager.CurrentUser.Name, devices);
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
				return GKProcessorManager.GKSyncronyseTime(device, FiresecManager.CurrentUser.Name);
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
				return GKProcessorManager.GKGetDeviceInfo(device, FiresecManager.CurrentUser.Name);
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
				GKProcessorManager.GKExecuteDeviceCommand(device, stateBit, FiresecManager.CurrentUser.Name);
			}
		}

		public void GKReset(XBase xBase)
		{
			if (IsGKAsAService)
			{
				SafeOperationCall(() => { FiresecService.GKReset(xBase.BaseUID, xBase.ObjectType); }, "GKReset");
			}
			else
			{
				GKProcessorManager.GKReset(xBase, FiresecManager.CurrentUser.Name);
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
				GKProcessorManager.GKResetFire1(zone, FiresecManager.CurrentUser.Name);
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
				GKProcessorManager.GKResetFire2(zone, FiresecManager.CurrentUser.Name);
			}
		}

		public void GKSetAutomaticRegime(XBase xBase)
		{
			if (IsGKAsAService)
			{
				SafeOperationCall(() => { FiresecService.GKSetAutomaticRegime(xBase.BaseUID, xBase.ObjectType); }, "GKSetAutomaticRegime");
			}
			else
			{
				GKProcessorManager.GKSetAutomaticRegime(xBase, FiresecManager.CurrentUser.Name);
			}
		}

		public void GKSetManualRegime(XBase xBase)
		{
			if (IsGKAsAService)
			{
				SafeOperationCall(() => { FiresecService.GKSetManualRegime(xBase.BaseUID, xBase.ObjectType); }, "GKSetManualRegime");
			}
			else
			{
				GKProcessorManager.GKSetManualRegime(xBase, FiresecManager.CurrentUser.Name);
			}
		}

		public void GKSetIgnoreRegime(XBase xBase)
		{
			if (IsGKAsAService)
			{
				SafeOperationCall(() => { FiresecService.GKTurnOn(xBase.BaseUID, xBase.ObjectType); }, "GKTurnOn");
			}
			else
			{
				GKProcessorManager.GKSetIgnoreRegime(xBase, FiresecManager.CurrentUser.Name);
			}
		}

		public void GKTurnOn(XBase xBase)
		{
			if (IsGKAsAService)
			{
				SafeOperationCall(() => { FiresecService.GKTurnOn(xBase.BaseUID, xBase.ObjectType); }, "GKTurnOn");
			}
			else
			{
				GKProcessorManager.GKTurnOn(xBase, FiresecManager.CurrentUser.Name);
			}
		}

		public void GKTurnOnNow(XBase xBase)
		{
			if (IsGKAsAService)
			{
				SafeOperationCall(() => { FiresecService.GKTurnOnNow(xBase.BaseUID, xBase.ObjectType); }, "GKTurnOnNow");
			}
			else
			{
				GKProcessorManager.GKTurnOnNow(xBase, FiresecManager.CurrentUser.Name);
			}
		}

		public void GKTurnOff(XBase xBase)
		{
			if (IsGKAsAService)
			{
				SafeOperationCall(() => { FiresecService.GKTurnOff(xBase.BaseUID, xBase.ObjectType); }, "GKTurnOff");
			}
			else
			{
				GKProcessorManager.GKTurnOff(xBase, FiresecManager.CurrentUser.Name);
			}
		}

		public void GKTurnOffNow(XBase xBase)
		{
			if (IsGKAsAService)
			{
				SafeOperationCall(() => { FiresecService.GKTurnOffNow(xBase.BaseUID, xBase.ObjectType); }, "GKTurnOffNow");
			}
			else
			{
				GKProcessorManager.GKTurnOffNow(xBase, FiresecManager.CurrentUser.Name);
			}
		}

		public void GKStop(XBase xBase)
		{
			if (IsGKAsAService)
			{
				SafeOperationCall(() => { FiresecService.GKStop(xBase.BaseUID, xBase.ObjectType); }, "GKStop");
			}
			else
			{
				GKProcessorManager.GKStop(xBase, FiresecManager.CurrentUser.Name);
			}
		}

		public void GKStartMeasureMonitoring(XDevice device)
		{
			if (IsGKAsAService)
			{
				SafeOperationCall(() => { FiresecService.GKStartMeasureMonitoring(device.UID); }, "GKStartMeasureMonitoring");
			}
			else
			{
				GKProcessorManager.GKStartMeasureMonitoring(device);
			}
		}

		public void GKStopMeasureMonitoring(XDevice device)
		{
			if (IsGKAsAService)
			{
				SafeOperationCall(() => { FiresecService.GKStopMeasureMonitoring(device.UID); }, "GKStopMeasureMonitoring");
			}
			else
			{
				GKProcessorManager.GKStopMeasureMonitoring(device);
			}
		}

		public void GKAddMessage(string name, string description)
		{
			if (IsGKAsAService)
			{
			}
			else
			{
				GKProcessorManager.AddGKMessage(name, description, XStateClass.Norm, null, FiresecManager.CurrentUser.Name, true);
			}
		}

		public void AddJournalItem(JournalItem journalItem)
		{
			if (IsGKAsAService)
			{
				SafeOperationCall(() => { FiresecService.AddJournalItem(journalItem); }, "AddJournalItem");
			}
			else
			{
			}
		}
	}
}