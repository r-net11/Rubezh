using RubezhAPI.GK;
using RubezhAPI.Journal;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace RubezhAPI
{
	[ServiceContract]
	public interface IGKService
	{
		[OperationContract]
		void CancelGKProgress(Guid progressCallbackUID, string userName, Guid clientUID);

		[OperationContract]
		OperationResult<bool> GKWriteConfiguration(Guid deviceUID, Guid clientUID);

		[OperationContract]
		OperationResult<GKDeviceConfiguration> GKReadConfiguration(Guid deviceUID, Guid clientUID);

		[OperationContract]
		OperationResult<bool> GKReadConfigurationFromGKFile(Guid deviceUID, Guid clientUID);

		[OperationContract]
		OperationResult<GKDevice> GKAutoSearch(Guid deviceUID, Guid clientUID);

		[OperationContract]
		OperationResult<bool> GKUpdateFirmware(Guid deviceUID, List<byte> firmwareBytes, Guid clientUID);

		[OperationContract]
		OperationResult<bool> GKSyncronyseTime(Guid deviceUID, Guid clientUID);

		[OperationContract]
		OperationResult<string> GKGetDeviceInfo(Guid deviceUID, Guid clientUID);

		[OperationContract]
		OperationResult<int> GKGetJournalItemsCount(Guid deviceUID, Guid clientUID);

		[OperationContract]
		OperationResult<JournalItem> GKReadJournalItem(Guid deviceUID, int no, Guid clientUID);

		[OperationContract]
		OperationResult<bool> GKSetSingleParameter(Guid objectUID, List<byte> parameterBytes, Guid clientUID, List<GKProperty> deviceProperties);

		[OperationContract]
		OperationResult<List<GKProperty>> GKGetSingleParameter(Guid objectUID, Guid clientUID);

		[OperationContract]
		OperationResult<bool> GKRewriteAllSchedules(Guid gkDeviceUID, Guid clientUID);

		[OperationContract]
		OperationResult<bool> GKSetSchedule(GKSchedule schedule, Guid clientUID);

		[OperationContract]
		OperationResult<bool> GKGetUsers(Guid gkDeviceUID, Guid clientUID);

		[OperationContract]
		OperationResult<bool> GKRewriteUsers(Guid gkDeviceUID, Guid clientUID);

		[OperationContract]
		OperationResult<List<MirrorUser>> GKReadMirrorUsers(Guid gkDeviceUID, Guid clientUID);

		[OperationContract]
		OperationResult<bool> GKWriteMirrorUsers(Guid gkDeviceUID, List<MirrorUser> mirrorUsers, Guid clientUID);

		[OperationContract]
		OperationResult<List<GKUser>> GetGKUsers(Guid deviceUID, Guid clientUID);

		[OperationContract]
		OperationResult<bool> RewritePmfUsers(Guid uid, List<GKUser> users, Guid clientUID);

		[OperationContract]
		OperationResult<List<byte>> GKGKHash(Guid gkDeviceUID, Guid clientUID);

		[OperationContract]
		GKStates GKGetStates(Guid clientUID);

		[OperationContract]
		void GKExecuteDeviceCommand(Guid deviceUID, GKStateBit stateBit, Guid clientUID);

		[OperationContract]
		void GKReset(Guid uid, GKBaseObjectType objectType, Guid clientUID);

		[OperationContract]
		void GKResetFire1(Guid zoneUID, Guid clientUID);

		[OperationContract]
		void GKResetFire2(Guid zoneUID, Guid clientUID);

		[OperationContract]
		void GKSetAutomaticRegime(Guid uid, GKBaseObjectType objectType, Guid clientUID);

		[OperationContract]
		void GKSetManualRegime(Guid uid, GKBaseObjectType objectType, Guid clientUID);

		[OperationContract]
		void GKSetIgnoreRegime(Guid uid, GKBaseObjectType objectType, Guid clientUID);

		[OperationContract]
		void GKTurnOn(Guid uid, GKBaseObjectType objectType, Guid clientUID);

		[OperationContract]
		void GKTurnOnNow(Guid uid, GKBaseObjectType objectType, Guid clientUID);

		[OperationContract]
		void GKTurnOnInAutomatic(Guid uid, GKBaseObjectType objectType, Guid clientUID);

		[OperationContract]
		void GKTurnOnNowInAutomatic(Guid uid, GKBaseObjectType objectType, Guid clientUID);

		[OperationContract]
		void GKTurnOff(Guid uid, GKBaseObjectType objectType, Guid clientUID);

		[OperationContract]
		void GKTurnOffNow(Guid uid, GKBaseObjectType objectType, Guid clientUID);

		[OperationContract]
		void GKTurnOffInAutomatic(Guid uid, GKBaseObjectType objectType, Guid clientUID);

		[OperationContract]
		void GKTurnOffNowInAutomatic(Guid uid, GKBaseObjectType objectType, Guid clientUID);

		[OperationContract]
		void GKStop(Guid uid, GKBaseObjectType objectType, Guid clientUID);

		[OperationContract]
		void GKStartMeasureMonitoring(Guid deviceUID, Guid clientUID);

		[OperationContract]
		void GKStopMeasureMonitoring(Guid deviceUID, Guid clientUID);

		[OperationContract]
		OperationResult<uint> GKGetReaderCode(Guid deviceUID, Guid clientUID);

		[OperationContract]
		void GKOpenSKDZone(Guid zoneUID, Guid clientUID);

		[OperationContract]
		void GKCloseSKDZone(Guid zoneUID, Guid clientUID);

		[OperationContract]
		OperationResult<CurrentConsumption> GetAlsMeasure(Guid alsUid, Guid clientUID);
	}
}