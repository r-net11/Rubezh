using System;
using System.Collections.Generic;
using System.ServiceModel;
using FiresecAPI.GK;

namespace FiresecAPI
{
	[ServiceContract]
	public interface IGKService
	{
		[OperationContract]
		void CancelGKProgress(Guid progressCallbackUID, string userName);

		[OperationContract]
		OperationResult<bool> GKWriteConfiguration(Guid deviceUID);

		[OperationContract]
		OperationResult<XDeviceConfiguration> GKReadConfiguration(Guid deviceUID);

		[OperationContract]
		OperationResult<XDeviceConfiguration> GKReadConfigurationFromGKFile(Guid deviceUID);

		[OperationContract]
		OperationResult<bool> GKUpdateFirmware(Guid deviceUID, string fileName);

		[OperationContract]
		OperationResult<bool> GKUpdateFirmwareFSCS(HexFileCollectionInfo hxcFileInfo, string userName, List<Guid> deviceUIDs);
		
		[OperationContract]
		OperationResult<bool> GKSyncronyseTime(Guid deviceUID);

		[OperationContract]
		OperationResult<string> GKGetDeviceInfo(Guid deviceUID);

		[OperationContract]
		OperationResult<int> GKGetJournalItemsCount(Guid deviceUID);

		[OperationContract]
		OperationResult<XJournalItem> GKReadJournalItem(Guid deviceUID, int no);

		[OperationContract]
		OperationResult<bool> GKSetSingleParameter(Guid objectUID, List<byte> parameterBytes);

		[OperationContract]
		OperationResult<List<XProperty>> GKGetSingleParameter(Guid objectUID);

		[OperationContract]
		OperationResult<List<byte>> GKGKHash(Guid gkDeviceUID);

		[OperationContract]
		GKStates GKGetStates();

		[OperationContract]
		void GKExecuteDeviceCommand(Guid deviceUID, XStateBit stateBit);

		[OperationContract]
		void GKReset(Guid uid, XBaseObjectType objectType);

		[OperationContract]
		void GKResetFire1(Guid zoneUID);

		[OperationContract]
		void GKResetFire2(Guid zoneUID);

		[OperationContract]
		void GKSetAutomaticRegime(Guid uid, XBaseObjectType objectType);

		[OperationContract]
		void GKSetManualRegime(Guid uid, XBaseObjectType objectType);

		[OperationContract]
		void GKSetIgnoreRegime(Guid uid, XBaseObjectType objectType);

		[OperationContract]
		void GKTurnOn(Guid uid, XBaseObjectType objectType);

		[OperationContract]
		void GKTurnOnNow(Guid uid, XBaseObjectType objectType);

		[OperationContract]
		void GKTurnOff(Guid uid, XBaseObjectType objectType);

		[OperationContract]
		void GKTurnOffNow(Guid uid, XBaseObjectType objectType);

		[OperationContract]
		void GKStop(Guid uid, XBaseObjectType objectType);

		[OperationContract]
		void GKStartMeasureMonitoring(Guid deviceUID);

		[OperationContract]
		void GKStopMeasureMonitoring(Guid deviceUID);

		#region Journal
		[OperationContract]
		List<XJournalItem> GetGKTopLastJournalItems(int count);

		[OperationContract]
		void BeginGetGKFilteredArchive(XArchiveFilter archiveFilter, Guid archivePortionUID);

		[OperationContract]
		List<string> GetDistinctGKJournalNames();

		[OperationContract]
		List<string> GetDistinctGKJournalDescriptions();
		#endregion
	}
}