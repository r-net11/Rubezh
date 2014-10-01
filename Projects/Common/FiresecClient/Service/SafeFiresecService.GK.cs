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

		public OperationResult<bool> GKWriteConfiguration(XDevice device)
		{
			return SafeOperationCall(() => FiresecService.GKWriteConfiguration(device.UID), "GKWriteConfiguration");
		}

		public OperationResult<XDeviceConfiguration> GKReadConfiguration(XDevice device)
		{
			return SafeOperationCall(() => FiresecService.GKReadConfiguration(device.UID), "GKReadConfiguration");
		}

		public OperationResult<XDeviceConfiguration> GKReadConfigurationFromGKFile(XDevice device)
		{
			return SafeOperationCall(() => FiresecService.GKReadConfigurationFromGKFile(device.UID), "GKReadConfigurationFromGKFile");
		}

		public OperationResult<bool> GKUpdateFirmware(XDevice device, string fileName)
		{
			return SafeOperationCall(() => FiresecService.GKUpdateFirmware(device.UID, fileName), "GKUpdateFirmware");
		}

		public OperationResult<bool> GKUpdateFirmwareFSCS(HexFileCollectionInfo hxcFileInfo, List<XDevice> devices)
		{
			var deviceUIDs = new List<Guid>();
			foreach (var device in devices)
			{
				deviceUIDs.Add(device.UID);
			}
			var result = SafeOperationCall(() => FiresecService.GKUpdateFirmwareFSCS(hxcFileInfo, FiresecManager.CurrentUser.Name, deviceUIDs), "GKUpdateFirmwareFSCS");
			return result;
		}

		public OperationResult<bool> GKSyncronyseTime(XDevice device)
		{
			return SafeOperationCall(() => { return FiresecService.GKSyncronyseTime(device.UID); }, "GKSyncronyseTime");
		}

		public OperationResult<string> GKGetDeviceInfo(XDevice device)
		{
			return SafeOperationCall(() => { return FiresecService.GKGetDeviceInfo(device.UID); }, "GKGetDeviceInfo");
		}

		public OperationResult<int> GKGetJournalItemsCount(XDevice device)
		{
			return SafeOperationCall(() => { return FiresecService.GKGetJournalItemsCount(device.UID); }, "GKGetJournalItemsCount");
		}

		public OperationResult<XJournalItem> GKReadJournalItem(XDevice device, int no)
		{
			return SafeOperationCall(() => { return FiresecService.GKReadJournalItem(device.UID, no); }, "GKReadJournalItem");
		}

		public OperationResult<bool> GKSetSingleParameter(XBase xBase, List<byte> parameterBytes)
		{
			return SafeOperationCall<bool>(() => { return FiresecService.GKSetSingleParameter(xBase.UID, parameterBytes); }, "SetSingleParameter");
		}

		public OperationResult<List<XProperty>> GKGetSingleParameter(XBase xBase)
		{
			return SafeOperationCall<List<XProperty>>(() => { return FiresecService.GKGetSingleParameter(xBase.UID); }, "GetSingleParameter");
		}

		public OperationResult<List<byte>> GKGKHash(XDevice device)
		{
			return SafeOperationCall<List<byte>>(() => { return FiresecService.GKGKHash(device.UID); }, "GKGKHash");
		}

		public GKStates GKGetStates()
		{
			return SafeOperationCall<GKStates>(() => { return FiresecService.GKGetStates(); }, "GKGetStates");
		}

		public void GKExecuteDeviceCommand(XDevice device, XStateBit stateBit)
		{
			SafeOperationCall(() => { FiresecService.GKExecuteDeviceCommand(device.UID, stateBit); }, "GKExecuteDeviceCommand");
		}

		public void GKReset(XBase xBase)
		{
			SafeOperationCall(() => { FiresecService.GKReset(xBase.UID, xBase.ObjectType); }, "GKReset");
		}

		public void GKResetFire1(XZone zone)
		{
			SafeOperationCall(() => { FiresecService.GKResetFire1(zone.UID); }, "GKResetFire1");
		}

		public void GKResetFire2(XZone zone)
		{
			SafeOperationCall(() => { FiresecService.GKResetFire2(zone.UID); }, "GKResetFire2");
		}

		public void GKSetAutomaticRegime(XBase xBase)
		{
			SafeOperationCall(() => { FiresecService.GKSetAutomaticRegime(xBase.UID, xBase.ObjectType); }, "GKSetAutomaticRegime");
		}

		public void GKSetManualRegime(XBase xBase)
		{
			SafeOperationCall(() => { FiresecService.GKSetManualRegime(xBase.UID, xBase.ObjectType); }, "GKSetManualRegime");
		}

		public void GKSetIgnoreRegime(XBase xBase)
		{
			SafeOperationCall(() => { FiresecService.GKSetIgnoreRegime(xBase.UID, xBase.ObjectType); }, "GKTurnOn");
		}

		public void GKTurnOn(XBase xBase)
		{
			SafeOperationCall(() => { FiresecService.GKTurnOn(xBase.UID, xBase.ObjectType); }, "GKTurnOn");
		}

		public void GKTurnOnNow(XBase xBase)
		{
			SafeOperationCall(() => { FiresecService.GKTurnOnNow(xBase.UID, xBase.ObjectType); }, "GKTurnOnNow");
		}

		public void GKTurnOff(XBase xBase)
		{
			SafeOperationCall(() => { FiresecService.GKTurnOff(xBase.UID, xBase.ObjectType); }, "GKTurnOff");
		}

		public void GKTurnOffNow(XBase xBase)
		{
			SafeOperationCall(() => { FiresecService.GKTurnOffNow(xBase.UID, xBase.ObjectType); }, "GKTurnOffNow");
		}

		public void GKStop(XBase xBase)
		{
			SafeOperationCall(() => { FiresecService.GKStop(xBase.UID, xBase.ObjectType); }, "GKStop");
		}

		public void GKStartMeasureMonitoring(XDevice device)
		{
			SafeOperationCall(() => { FiresecService.GKStartMeasureMonitoring(device.UID); }, "GKStartMeasureMonitoring");
		}

		public void GKStopMeasureMonitoring(XDevice device)
		{
			SafeOperationCall(() => { FiresecService.GKStopMeasureMonitoring(device.UID); }, "GKStopMeasureMonitoring");
		}

		public void GKAddMessage(JournalEventNameType journalEventNameType, string description)
		{
		}

		#region Journal
		public List<XJournalItem> GetGKTopLastJournalItems(int count)
		{
			return SafeOperationCall(() => FiresecService.GetGKTopLastJournalItems(count), "GetGKTopLastJournalItems");
		}

		public void BeginGetGKFilteredArchive(XArchiveFilter archiveFilter, Guid archivePortionUID)
		{
			SafeOperationCall(() => FiresecService.BeginGetGKFilteredArchive(archiveFilter, archivePortionUID), "BeginGetGKFilteredArchive");
		}

		public List<string> GetGkEventNames()
		{
			return SafeOperationCall(() => FiresecService.GetDistinctGKJournalNames(), "GetGkEventNames");
		}

		public List<string> GetGkEventDescriptions()
		{
			return SafeOperationCall(() => FiresecService.GetDistinctGKJournalDescriptions(), "GetGkEventDescriptions");
		}
		#endregion
	}
}