using System;
using System.Collections.Generic;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecAPI.Journal;
using GKProcessor;
using Infrastructure.Common;
using System.IO;

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

		public Stream GKReadConfigurationFromGKFile(GKDevice device)
		{
			return SafeOperationCall(() => FiresecService.GKReadConfigurationFromGKFile(device.UID), "GKReadConfigurationFromGKFile");
		}

		public OperationResult<GKDevice> GKAutoSearch(GKDevice device)
		{
			return SafeOperationCall(() => FiresecService.GKAutoSearch(device.UID), "GKAutoSearch");
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

		public OperationResult<JournalItem> GKReadJournalItem(GKDevice device, int no)
		{
			return SafeOperationCall(() => { return FiresecService.GKReadJournalItem(device.UID, no); }, "GKReadJournalItem");
		}

		public OperationResult<bool> GKSetSingleParameter(GKBase gkBase, List<byte> parameterBytes)
		{
			return SafeOperationCall(() => { return FiresecService.GKSetSingleParameter(gkBase.UID, parameterBytes); }, "SetSingleParameter");
		}

		public OperationResult<List<GKProperty>> GKGetSingleParameter(GKBase gkBase)
		{
			return SafeOperationCall(() => { return FiresecService.GKGetSingleParameter(gkBase.UID); }, "GetSingleParameter");
		}

		public OperationResult<bool> GKRewriteAllSchedules(GKDevice device)
		{
			return SafeOperationCall(() => { return FiresecService.GKRewriteAllSchedules(device.UID); }, "GKRewriteAllSchedules");
		}

		public OperationResult<bool> GKSetSchedule(GKSchedule schedule)
		{
			return SafeOperationCall(() => { return FiresecService.GKSetSchedule(schedule); }, "GKSetSchedule");
		}

		public OperationResult<List<GKUser>> GKGetUsers(GKDevice device)
		{
			return SafeOperationCall(() => { return FiresecService.GKGetUsers(device.UID); }, "GKGetUsers");
		}

		public OperationResult<bool> GKRewriteUsers(GKDevice device)
		{
			return SafeOperationCall(() => { return FiresecService.GKRewriteUsers(device.UID); }, "GKRewriteUsers");
		}

		public OperationResult<List<byte>> GKGKHash(GKDevice device)
		{
			return SafeOperationCall<List<byte>>(() => { return FiresecService.GKGKHash(device.UID); }, "GKGKHash");
		}

		public GKStates GKGetStates()
		{
			return SafeOperationCall(() => { return FiresecService.GKGetStates(); }, "GKGetStates");
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

		public void GKTurnOnInAutomatic(GKBase gkBase)
		{
			SafeOperationCall(() => { FiresecService.GKTurnOnInAutomatic(gkBase.UID, gkBase.ObjectType); }, "GKTurnOnInAutomatic");
		}

		public void GKTurnOnNowInAutomatic(GKBase gkBase)
		{
			SafeOperationCall(() => { FiresecService.GKTurnOnNowInAutomatic(gkBase.UID, gkBase.ObjectType); }, "GKTurnOnNowInAutomatic");
		}

		public void GKTurnOff(GKBase gkBase)
		{
			SafeOperationCall(() => { FiresecService.GKTurnOff(gkBase.UID, gkBase.ObjectType); }, "GKTurnOff");
		}

		public void GKTurnOffNow(GKBase gkBase)
		{
			SafeOperationCall(() => { FiresecService.GKTurnOffNow(gkBase.UID, gkBase.ObjectType); }, "GKTurnOffNow");
		}

		public void GKTurnOffInAutomatic(GKBase gkBase)
		{
			SafeOperationCall(() => { FiresecService.GKTurnOffInAutomatic(gkBase.UID, gkBase.ObjectType); }, "GKTurnOffInAutomatic");
		}

		public void GKTurnOffNowInAutomatic(GKBase gkBase)
		{
			SafeOperationCall(() => { FiresecService.GKTurnOffNowInAutomatic(gkBase.UID, gkBase.ObjectType); }, "GKTurnOffNowInAutomatic");
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

		public OperationResult<uint> GKGetReaderCode(GKDevice device)
		{
			return SafeOperationCall(() => { return FiresecService.GKGetReaderCode(device.UID); }, "GKGetReaderCode");
		}

		public void GKOpenSKDZone(GKSKDZone zone)
		{
			SafeOperationCall(() => { FiresecService.GKOpenSKDZone(zone.UID); }, "GKOpenSKDZone");
		}

		public void GKCloseSKDZone(GKSKDZone zone)
		{
			SafeOperationCall(() => { FiresecService.GKCloseSKDZone(zone.UID); }, "GKCloseSKDZone");
		}

		public OperationResult<CurrentConsumption> GetAlsMeasure(Guid alsUid)
		{
			return SafeOperationCall(() => FiresecService.GetAlsMeasure(alsUid), "GetAlsMeasure");
		}

		public void GKAddMessage(JournalEventNameType journalEventNameType, string description)
		{
		}
	}
}