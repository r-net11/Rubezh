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
		OperationResult<GKDeviceConfiguration> GKReadConfiguration(Guid deviceUID);

		[OperationContract]
		OperationResult<GKDeviceConfiguration> GKReadConfigurationFromGKFile(Guid deviceUID);

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
		OperationResult<GKJournalItem> GKReadJournalItem(Guid deviceUID, int no);

		[OperationContract]
		OperationResult<bool> GKSetSingleParameter(Guid objectUID, List<byte> parameterBytes);

		[OperationContract]
		OperationResult<List<GKProperty>> GKGetSingleParameter(Guid objectUID);

		[OperationContract]
		OperationResult<bool> GKSetSchedule(GKSchedule schedule);

		[OperationContract]
		OperationResult<GKSchedule> GKGetSchedule(int no);

		[OperationContract]
		OperationResult<List<byte>> GKGKHash(Guid gkDeviceUID);

		[OperationContract]
		GKStates GKGetStates();

		[OperationContract]
		void GKExecuteDeviceCommand(Guid deviceUID, GKStateBit stateBit);

		[OperationContract]
		void GKReset(Guid uid, GKBaseObjectType objectType);

		[OperationContract]
		void GKResetFire1(Guid zoneUID);

		[OperationContract]
		void GKResetFire2(Guid zoneUID);

		[OperationContract]
		void GKSetAutomaticRegime(Guid uid, GKBaseObjectType objectType);

		[OperationContract]
		void GKSetManualRegime(Guid uid, GKBaseObjectType objectType);

		[OperationContract]
		void GKSetIgnoreRegime(Guid uid, GKBaseObjectType objectType);

		[OperationContract]
		void GKTurnOn(Guid uid, GKBaseObjectType objectType);

		[OperationContract]
		void GKTurnOnNow(Guid uid, GKBaseObjectType objectType);

		[OperationContract]
		void GKTurnOff(Guid uid, GKBaseObjectType objectType);

		[OperationContract]
		void GKTurnOffNow(Guid uid, GKBaseObjectType objectType);

		[OperationContract]
		void GKStop(Guid uid, GKBaseObjectType objectType);

		[OperationContract]
		void GKStartMeasureMonitoring(Guid deviceUID);

		[OperationContract]
		void GKStopMeasureMonitoring(Guid deviceUID);

		#region Journal
		[OperationContract]
		List<GKJournalItem> GetGKTopLastJournalItems(int count);

		[OperationContract]
		void BeginGetGKFilteredArchive(GKArchiveFilter archiveFilter, Guid archivePortionUID);

		[OperationContract]
		List<string> GetDistinctGKJournalNames();

		[OperationContract]
		List<string> GetDistinctGKJournalDescriptions();
		#endregion
	}
}