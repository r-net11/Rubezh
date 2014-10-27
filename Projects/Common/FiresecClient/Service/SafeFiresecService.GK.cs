using System;
using System.Collections.Generic;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecAPI.Journal;
using GKProcessor;
using Infrastructure.Common;

namespace FiresecClient
{
	public partial class SafeFiresecService
	{
		public void CancelGKProgress(Guid progressCallbackUID, string userName)
		{
			SafeOperationCall(() => FiresecService.CancelGKProgress(progressCallbackUID, userName), "CancelGKProgress");
		}

		public OperationResult<bool> GKWriteConfiguration(GKDevice device)
		{
			return SafeOperationCall(() => FiresecService.GKWriteConfiguration(device.UID), "GKWriteConfiguration");
		}

		public OperationResult<GKDeviceConfiguration> GKReadConfiguration(GKDevice device)
		{
			return SafeOperationCall(() => FiresecService.GKReadConfiguration(device.UID), "GKReadConfiguration");
		}

		public OperationResult<GKDeviceConfiguration> GKReadConfigurationFromGKFile(GKDevice device)
		{
			return SafeOperationCall(() => FiresecService.GKReadConfigurationFromGKFile(device.UID), "GKReadConfigurationFromGKFile");
		}

		public OperationResult<bool> GKUpdateFirmware(GKDevice device, string fileName)
		{
			return SafeOperationCall(() => FiresecService.GKUpdateFirmware(device.UID, fileName), "GKUpdateFirmware");
		}

		public OperationResult<bool> GKUpdateFirmwareFSCS(HexFileCollectionInfo hxcFileInfo, List<GKDevice> devices)
		{
			var deviceUIDs = new List<Guid>();
			foreach (var device in devices)
			{
				deviceUIDs.Add(device.UID);
			}
			var result = SafeOperationCall(() => FiresecService.GKUpdateFirmwareFSCS(hxcFileInfo, FiresecManager.CurrentUser.Name, deviceUIDs), "GKUpdateFirmwareFSCS");
			return result;
		}

		public OperationResult<bool> GKSyncronyseTime(GKDevice device)
		{
			return SafeOperationCall(() => { return FiresecService.GKSyncronyseTime(device.UID); }, "GKSyncronyseTime");
		}

		public OperationResult<string> GKGetDeviceInfo(GKDevice device)
		{
			return SafeOperationCall(() => { return FiresecService.GKGetDeviceInfo(device.UID); }, "GKGetDeviceInfo");
		}

		public OperationResult<int> GKGetJournalItemsCount(GKDevice device)
		{
			return SafeOperationCall(() => { return FiresecService.GKGetJournalItemsCount(device.UID); }, "GKGetJournalItemsCount");
		}

		public OperationResult<GKJournalItem> GKReadJournalItem(GKDevice device, int no)
		{
			return SafeOperationCall(() => { return FiresecService.GKReadJournalItem(device.UID, no); }, "GKReadJournalItem");
		}

		public OperationResult<bool> GKSetSingleParameter(GKBase gkBase, List<byte> parameterBytes)
		{
			return SafeOperationCall<bool>(() => { return FiresecService.GKSetSingleParameter(gkBase.UID, parameterBytes); }, "SetSingleParameter");
		}

		public OperationResult<List<GKProperty>> GKGetSingleParameter(GKBase gkBase)
		{
			return SafeOperationCall<List<GKProperty>>(() => { return FiresecService.GKGetSingleParameter(gkBase.UID); }, "GetSingleParameter");
		}

		public OperationResult<bool> GKSetSchedule(GKSchedule schedule)
		{
			return SafeOperationCall(() => { return FiresecService.GKSetSchedule(schedule); }, "GKSetSchedule");
		}

		public OperationResult<GKSchedule> GKGetSchedule(int no)
		{
			return SafeOperationCall(() => { return FiresecService.GKGetSchedule(no); }, "GKGetSchedule");
		}

		public OperationResult<List<byte>> GKGKHash(GKDevice device)
		{
			return SafeOperationCall<List<byte>>(() => { return FiresecService.GKGKHash(device.UID); }, "GKGKHash");
		}

		public GKStates GKGetStates()
		{
			return SafeOperationCall<GKStates>(() => { return FiresecService.GKGetStates(); }, "GKGetStates");
		}

		public void GKExecuteDeviceCommand(GKDevice device, GKStateBit stateBit)
		{
			SafeOperationCall(() => { FiresecService.GKExecuteDeviceCommand(device.UID, stateBit); }, "GKExecuteDeviceCommand");
		}

		public void GKReset(GKBase gkBase)
		{
			SafeOperationCall(() => { FiresecService.GKReset(gkBase.UID, gkBase.ObjectType); }, "GKReset");
		}

		public void GKResetFire1(GKZone zone)
		{
			SafeOperationCall(() => { FiresecService.GKResetFire1(zone.UID); }, "GKResetFire1");
		}

		public void GKResetFire2(GKZone zone)
		{
			SafeOperationCall(() => { FiresecService.GKResetFire2(zone.UID); }, "GKResetFire2");
		}

		public void GKSetAutomaticRegime(GKBase gkBase)
		{
			SafeOperationCall(() => { FiresecService.GKSetAutomaticRegime(gkBase.UID, gkBase.ObjectType); }, "GKSetAutomaticRegime");
		}

		public void GKSetManualRegime(GKBase gkBase)
		{
			SafeOperationCall(() => { FiresecService.GKSetManualRegime(gkBase.UID, gkBase.ObjectType); }, "GKSetManualRegime");
		}

		public void GKSetIgnoreRegime(GKBase gkBase)
		{
			SafeOperationCall(() => { FiresecService.GKSetIgnoreRegime(gkBase.UID, gkBase.ObjectType); }, "GKTurnOn");
		}

		public void GKTurnOn(GKBase gkBase)
		{
			SafeOperationCall(() => { FiresecService.GKTurnOn(gkBase.UID, gkBase.ObjectType); }, "GKTurnOn");
		}

		public void GKTurnOnNow(GKBase gkBase)
		{
			SafeOperationCall(() => { FiresecService.GKTurnOnNow(gkBase.UID, gkBase.ObjectType); }, "GKTurnOnNow");
		}

		public void GKTurnOff(GKBase gkBase)
		{
			SafeOperationCall(() => { FiresecService.GKTurnOff(gkBase.UID, gkBase.ObjectType); }, "GKTurnOff");
		}

		public void GKTurnOffNow(GKBase gkBase)
		{
			SafeOperationCall(() => { FiresecService.GKTurnOffNow(gkBase.UID, gkBase.ObjectType); }, "GKTurnOffNow");
		}

		public void GKStop(GKBase gkBase)
		{
			SafeOperationCall(() => { FiresecService.GKStop(gkBase.UID, gkBase.ObjectType); }, "GKStop");
		}

		public void GKStartMeasureMonitoring(GKDevice device)
		{
			SafeOperationCall(() => { FiresecService.GKStartMeasureMonitoring(device.UID); }, "GKStartMeasureMonitoring");
		}

		public void GKStopMeasureMonitoring(GKDevice device)
		{
			SafeOperationCall(() => { FiresecService.GKStopMeasureMonitoring(device.UID); }, "GKStopMeasureMonitoring");
		}

		public void GKAddMessage(JournalEventNameType journalEventNameType, string description)
		{
		}
	}
}