﻿using RubezhAPI.GK;
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
		void CancelGKProgress(Guid clientUID, Guid progressCallbackUID, string userName);

		[OperationContract]
		OperationResult<bool> GKWriteConfiguration(Guid clientUID, Guid deviceUID);

		[OperationContract]
		OperationResult<GKDeviceConfiguration> GKReadConfiguration(Guid clientUID, Guid deviceUID);

		[OperationContract]
		OperationResult<bool> GKReadConfigurationFromGKFile(Guid clientUID, Guid deviceUID);

		[OperationContract]
		OperationResult<GKDevice> GKAutoSearch(Guid clientUID, Guid deviceUID);

		[OperationContract]
		OperationResult<bool> GKUpdateFirmware(Guid clientUID, Guid deviceUID, List<byte> firmwareBytes);

		[OperationContract]
		OperationResult<bool> GKSyncronyseTime(Guid clientUID, Guid deviceUID);

		[OperationContract]
		OperationResult<string> GKGetDeviceInfo(Guid clientUID, Guid deviceUID);

		[OperationContract]
		OperationResult<int> GKGetJournalItemsCount(Guid clientUID, Guid deviceUID);

		[OperationContract]
		OperationResult<JournalItem> GKReadJournalItem(Guid clientUID, Guid deviceUID, int no);

		[OperationContract]
		OperationResult<bool> GKSetSingleParameter(Guid clientUID, Guid objectUID, List<byte> parameterBytes, List<GKProperty> deviceProperties);

		[OperationContract]
		OperationResult<List<GKProperty>> GKGetSingleParameter(Guid clientUID, Guid objectUID);

		[OperationContract]
		OperationResult<bool> GKRewriteAllSchedules(Guid clientUID, Guid gkDeviceUID);

		[OperationContract]
		OperationResult<bool> GKSetSchedule(Guid clientUID, GKSchedule schedule);

		[OperationContract]
		OperationResult<bool> GKGetUsers(Guid clientUID, Guid gkDeviceUID);

		[OperationContract]
		OperationResult<bool> GKRewriteUsers(Guid clientUID, Guid gkDeviceUID);

		[OperationContract]
		OperationResult<List<GKUser>> GetGKUsers(Guid clientUID, Guid deviceUID);

		[OperationContract]
		OperationResult<bool> RewritePmfUsers(Guid clientUID, Guid uid, List<GKUser> users);

		[OperationContract]
		OperationResult<List<byte>> GKGKHash(Guid clientUID, Guid gkDeviceUID);

		[OperationContract]
		GKStates GKGetStates(Guid clientUID);

		[OperationContract]
		void GKExecuteDeviceCommand(Guid clientUID, Guid deviceUID, GKStateBit stateBit);

		[OperationContract]
		void GKReset(Guid clientUID, Guid uid, GKBaseObjectType objectType);

		[OperationContract]
		void GKResetFire1(Guid clientUID, Guid zoneUID);

		[OperationContract]
		void GKResetFire2(Guid clientUID, Guid zoneUID);

		[OperationContract]
		void GKSetAutomaticRegime(Guid clientUID, Guid uid, GKBaseObjectType objectType);

		[OperationContract]
		void GKSetManualRegime(Guid clientUID, Guid uid, GKBaseObjectType objectType);

		[OperationContract]
		void GKSetIgnoreRegime(Guid clientUID, Guid uid, GKBaseObjectType objectType);

		[OperationContract]
		void GKTurnOn(Guid clientUID, Guid uid, GKBaseObjectType objectType);

		[OperationContract]
		void GKTurnOnNow(Guid clientUID, Guid uid, GKBaseObjectType objectType);

		[OperationContract]
		void GKTurnOnInAutomatic(Guid clientUID, Guid uid, GKBaseObjectType objectType);

		[OperationContract]
		void GKTurnOnNowInAutomatic(Guid clientUID, Guid uid, GKBaseObjectType objectType);

		[OperationContract]
		void GKTurnOff(Guid clientUID, Guid uid, GKBaseObjectType objectType);

		[OperationContract]
		void GKTurnOffNow(Guid clientUID, Guid uid, GKBaseObjectType objectType);

		[OperationContract]
		void GKTurnOffInAutomatic(Guid clientUID, Guid uid, GKBaseObjectType objectType);

		[OperationContract]
		void GKTurnOffNowInAutomatic(Guid clientUID, Guid uid, GKBaseObjectType objectType);

		[OperationContract]
		void GKStop(Guid clientUID, Guid uid, GKBaseObjectType objectType);

		[OperationContract]
		void GKStartMeasureMonitoring(Guid clientUID, Guid deviceUID);

		[OperationContract]
		void GKStopMeasureMonitoring(Guid clientUID, Guid deviceUID);

		[OperationContract]
		OperationResult<uint> GKGetReaderCode(Guid clientUID, Guid deviceUID);

		[OperationContract]
		void GKOpenSKDZone(Guid clientUID, Guid zoneUID);

		[OperationContract]
		void GKCloseSKDZone(Guid clientUID, Guid zoneUID);

		[OperationContract]
		OperationResult<CurrentConsumption> GetAlsMeasure(Guid clientUID, Guid alsUid);
	}
}