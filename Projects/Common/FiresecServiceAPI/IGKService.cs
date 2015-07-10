﻿using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using FiresecAPI.GK;
using FiresecAPI.Journal;

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
		OperationResult<bool> GKReadConfigurationFromGKFile(Guid deviceUID);

		[OperationContract]
		OperationResult<GKDevice> GKAutoSearch(Guid deviceUID);

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
		OperationResult<JournalItem> GKReadJournalItem(Guid deviceUID, int no);

		[OperationContract]
		OperationResult<bool> GKSetSingleParameter(Guid objectUID, List<byte> parameterBytes, List<GKProperty> deviceProperties);

		[OperationContract]
		OperationResult<List<GKProperty>> GKGetSingleParameter(Guid objectUID);

		[OperationContract]
		OperationResult<bool> GKRewriteAllSchedules(Guid gkDeviceUID);

		[OperationContract]
		OperationResult<bool> GKSetSchedule(GKSchedule schedule);

		[OperationContract]
		OperationResult<bool> GKGetUsers(Guid gkDeviceUID);

		[OperationContract]
		OperationResult<bool> GKRewriteUsers(Guid gkDeviceUID);

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
		void GKTurnOnInAutomatic(Guid uid, GKBaseObjectType objectType);

		[OperationContract]
		void GKTurnOnNowInAutomatic(Guid uid, GKBaseObjectType objectType);

		[OperationContract]
		void GKTurnOff(Guid uid, GKBaseObjectType objectType);

		[OperationContract]
		void GKTurnOffNow(Guid uid, GKBaseObjectType objectType);

		[OperationContract]
		void GKTurnOffInAutomatic(Guid uid, GKBaseObjectType objectType);

		[OperationContract]
		void GKTurnOffNowInAutomatic(Guid uid, GKBaseObjectType objectType);

		[OperationContract]
		void GKStop(Guid uid, GKBaseObjectType objectType);

		[OperationContract]
		void GKStartMeasureMonitoring(Guid deviceUID);

		[OperationContract]
		void GKStopMeasureMonitoring(Guid deviceUID);

		[OperationContract]
		OperationResult<uint> GKGetReaderCode(Guid deviceUID);

		[OperationContract]
		void GKOpenSKDZone(Guid zoneUID);

		[OperationContract]
		void GKCloseSKDZone(Guid zoneUID);

		[OperationContract]
		OperationResult<CurrentConsumption> GetAlsMeasure(Guid alsUid);
	}
}