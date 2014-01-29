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

		public void BeginGetGKFilteredArchive(XArchiveFilter archiveFilter)
		{
			if (IsGKAsAService)
			{
				SafeOperationCall(() => FiresecService.BeginGetGKFilteredArchive(archiveFilter), "BeginGetGKFilteredArchive");
			}
			else
			{
				SafeOperationCall(() => FiresecService.BeginGetGKFilteredArchive(archiveFilter), "BeginGetGKFilteredArchive");
			}
		}

		public void CancelGKProgress(Guid progressCallbackUID, string userName)
		{
			if (IsGKAsAService)
			{
				SafeOperationCall(() => FiresecService.CancelGKProgress(progressCallbackUID, userName), "CancelGKProgress");
			}
			else
			{
				GKProcessorManager.CancelGKProgress(progressCallbackUID, userName);
			}
		}

		public OperationResult<bool> GKWriteConfiguration(XDevice device)
		{
			if (IsGKAsAService)
			{
				return SafeOperationCall(() => FiresecService.GKWriteConfiguration(device.BaseUID), "GKWriteConfiguration");
			}
			else
			{
				var result = GKProcessorManager.GKWriteConfiguration(device, FiresecManager.CurrentUser.Name);
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

		public OperationResult<XDeviceConfiguration> GKReadConfigurationFromGKFile(XDevice device)
		{
			if (IsGKAsAService)
			{
				return SafeOperationCall(() => FiresecService.GKReadConfigurationFromGKFile(device.BaseUID), "GKReadConfigurationFromGKFile");
			}
			else
			{
				return GKProcessorManager.GKReadConfigurationFromGKFile(device, FiresecManager.CurrentUser.Name);
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
				var deviceUIDs = new List<Guid>();
				foreach (var device in devices)
				{
					deviceUIDs.Add(device.UID);
				}
				var result = SafeOperationCall(() => FiresecService.GKUpdateFirmwareFSCS(hxcFileInfo, FiresecManager.CurrentUser.Name, deviceUIDs), "GKUpdateFirmwareFSCS");
				return result;
			}
			else
			{
				return GKProcessorManager.GKUpdateFirmwareFSCS(hxcFileInfo, FiresecManager.CurrentUser.Name, devices);
			}
		}

		public OperationResult<bool> GKSyncronyseTime(XDevice device)
		{
			if (IsGKAsAService)
			{
				return SafeOperationCall(() => { return FiresecService.GKSyncronyseTime(device.BaseUID); }, "GKSyncronyseTime");
			}
			else
			{
				return new OperationResult<bool>() { Result = GKProcessorManager.GKSyncronyseTime(device, FiresecManager.CurrentUser.Name) };
			}
		}

		public OperationResult<string> GKGetDeviceInfo(XDevice device)
		{
			if (IsGKAsAService)
			{
				return SafeOperationCall(() => { return FiresecService.GKGetDeviceInfo(device.BaseUID); }, "GKGetDeviceInfo");
			}
			else
			{
				return new OperationResult<string>() { Result = GKProcessorManager.GKGetDeviceInfo(device, FiresecManager.CurrentUser.Name) };
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

		public OperationResult<bool> GKSetSingleParameter(XBase xBase, List<byte> parameterBytes)
		{
			if (IsGKAsAService)
			{
				return SafeOperationCall<bool>(() => { return FiresecService.GKSetSingleParameter(xBase.BaseUID, parameterBytes); }, "SetSingleParameter");
			}
			else
			{
				return GKProcessorManager.GKSetSingleParameter(xBase, parameterBytes);
			}
		}

		public OperationResult<List<XProperty>> GKGetSingleParameter(XBase xBase)
		{
			if (IsGKAsAService)
			{
				return SafeOperationCall<List<XProperty>>(() => { return FiresecService.GKGetSingleParameter(xBase.BaseUID); }, "GetSingleParameter");
			}
			else
			{
				return GKProcessorManager.GKGetSingleParameter(xBase);
			}
		}

		public OperationResult<List<byte>> GKGKHash(XDevice device)
		{
			if (IsGKAsAService)
			{
				return SafeOperationCall<List<byte>>(() => { return FiresecService.GKGKHash(device.BaseUID); }, "GKGKHash");
			}
			else
			{
				return GKProcessorManager.GKGKHash(device);
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
				SafeOperationCall(() => { FiresecService.GKSetIgnoreRegime(xBase.BaseUID, xBase.ObjectType); }, "GKTurnOn");
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

		public void GKAddMessage(EventNameEnum name, string description)
		{
			if (IsGKAsAService)
			{
			}
			else
			{
				GKProcessorManager.AddGKMessage(name, description, null, FiresecManager.CurrentUser.Name, true);
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