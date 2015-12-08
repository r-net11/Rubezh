using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Journal;
using System;
using System.Collections.Generic;
using System.IO;

namespace RubezhClient
{
	public partial class SafeFiresecService
	{
		public void CancelGKProgress(Guid progressCallbackUID, string userName)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.CancelGKProgress(progressCallbackUID, userName, FiresecServiceFactory.UID);
			}, "CancelGKProgress");
		}

		public OperationResult<bool> GKWriteConfiguration(GKDevice device)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GKWriteConfiguration(device.UID, FiresecServiceFactory.UID);
			}, "GKWriteConfiguration");
		}

		public OperationResult<GKDeviceConfiguration> GKReadConfiguration(GKDevice device)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GKReadConfiguration(device.UID, FiresecServiceFactory.UID);
			}, "GKReadConfiguration");
		}

		public OperationResult<bool> GKReadConfigurationFromGKFile(GKDevice device)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GKReadConfigurationFromGKFile(device.UID, FiresecServiceFactory.UID);
			}, "GKReadConfigurationFromGKFile");
		}

		public Stream GetServerFile(string filePath)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetServerFile(filePath, FiresecServiceFactory.UID);
			}, "GetServerFile");
		}

		public OperationResult<GKDevice> GKAutoSearch(GKDevice device)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GKAutoSearch(device.UID, FiresecServiceFactory.UID);
			}, "GKAutoSearch");
		}

		public OperationResult<bool> GKUpdateFirmware(GKDevice device, List<byte> firmwareBytes)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GKUpdateFirmware(device.UID, firmwareBytes, FiresecServiceFactory.UID);
			}, "GKUpdateFirmware");
		}

		public OperationResult<bool> GKSyncronyseTime(GKDevice device)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GKSyncronyseTime(device.UID, FiresecServiceFactory.UID);
			}, "GKSyncronyseTime");
		}

		public OperationResult<string> GKGetDeviceInfo(GKDevice device)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GKGetDeviceInfo(device.UID, FiresecServiceFactory.UID);
			}, "GKGetDeviceInfo");
		}

		public OperationResult<int> GKGetJournalItemsCount(GKDevice device)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GKGetJournalItemsCount(device.UID, FiresecServiceFactory.UID);
			}, "GKGetJournalItemsCount");
		}

		public OperationResult<JournalItem> GKReadJournalItem(GKDevice device, int no)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GKReadJournalItem(device.UID, no, FiresecServiceFactory.UID);
			}, "GKReadJournalItem");
		}

		public OperationResult<bool> GKSetSingleParameter(GKBase gkBase, List<byte> parameterBytes, List<GKProperty> deviceProperties = null)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GKSetSingleParameter(gkBase.UID, parameterBytes, FiresecServiceFactory.UID, deviceProperties);
			}, "SetSingleParameter");
		}

		public OperationResult<List<GKProperty>> GKGetSingleParameter(GKBase gkBase)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GKGetSingleParameter(gkBase.UID, FiresecServiceFactory.UID);
			}, "GetSingleParameter");
		}

		public OperationResult<bool> GKRewriteAllSchedules(GKDevice device)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GKRewriteAllSchedules(device.UID, FiresecServiceFactory.UID);
			}, "GKRewriteAllSchedules");
		}

		public OperationResult<bool> GKSetSchedule(GKSchedule schedule)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GKSetSchedule(schedule, FiresecServiceFactory.UID);
			}, "GKSetSchedule");
		}

		public OperationResult<bool> GKGetUsers(GKDevice device)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GKGetUsers(device.UID, FiresecServiceFactory.UID);
			}, "GKGetUsers");
		}

		public OperationResult<bool> GKRewriteUsers(GKDevice device)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GKRewriteUsers(device.UID, FiresecServiceFactory.UID);
			}, "GKRewriteUsers");
		}

		public OperationResult<List<MirrorUser>> GKReadMirrorUsers(GKDevice device)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GKReadMirrorUsers(device.UID, FiresecServiceFactory.UID);
			}, "GKReadMirrorUsers");
		}

		public OperationResult<bool> GKWriteMirrorUsers(GKDevice device, List<MirrorUser> mirrorUsers)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GKWriteMirrorUsers(device.UID, mirrorUsers, FiresecServiceFactory.UID);
			}, "GKWriteMirrorUsers");
		}

		public OperationResult<List<GKUser>> GetGKUsers(Guid deviceUID)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetGKUsers(deviceUID, FiresecServiceFactory.UID);
			}, "GetGKUsers");
		}

		public OperationResult<bool> RewritePmfUsers(Guid uid, List<GKUser> users)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.RewritePmfUsers(uid, users, FiresecServiceFactory.UID);
			}, "RewritePmfUsers");
		}

		public OperationResult<List<byte>> GKGKHash(GKDevice device)
		{
			return SafeOperationCall<List<byte>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GKGKHash(device.UID, FiresecServiceFactory.UID);
			}, "GKGKHash");
		}

		public GKStates GKGetStates()
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GKGetStates(FiresecServiceFactory.UID);
			}, "GKGetStates");
		}

		public void GKExecuteDeviceCommand(GKDevice device, GKStateBit stateBit)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.GKExecuteDeviceCommand(device.UID, stateBit, FiresecServiceFactory.UID);
			}, "GKExecuteDeviceCommand");
		}

		public void GKReset(GKBase gkBase)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.GKReset(gkBase.UID, gkBase.ObjectType, FiresecServiceFactory.UID);
			}, "GKReset");
		}

		public void GKResetFire1(GKZone zone)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.GKResetFire1(zone.UID, FiresecServiceFactory.UID);
			}, "GKResetFire1");
		}

		public void GKResetFire2(GKZone zone)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.GKResetFire2(zone.UID, FiresecServiceFactory.UID);
			}, "GKResetFire2");
		}

		public void GKSetAutomaticRegime(GKBase gkBase)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.GKSetAutomaticRegime(gkBase.UID, gkBase.ObjectType, FiresecServiceFactory.UID);
			}, "GKSetAutomaticRegime");
		}

		public void GKSetManualRegime(GKBase gkBase)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.GKSetManualRegime(gkBase.UID, gkBase.ObjectType, FiresecServiceFactory.UID);
			}, "GKSetManualRegime");
		}

		public void GKSetIgnoreRegime(GKBase gkBase)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.GKSetIgnoreRegime(gkBase.UID, gkBase.ObjectType, FiresecServiceFactory.UID);
			}, "GKTurnOn");
		}

		public void GKTurnOn(GKBase gkBase)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.GKTurnOn(gkBase.UID, gkBase.ObjectType, FiresecServiceFactory.UID);
			}, "GKTurnOn");
		}

		public void GKTurnOnNow(GKBase gkBase)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.GKTurnOnNow(gkBase.UID, gkBase.ObjectType, FiresecServiceFactory.UID);
			}, "GKTurnOnNow");
		}

		public void GKTurnOnInAutomatic(GKBase gkBase)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.GKTurnOnInAutomatic(gkBase.UID, gkBase.ObjectType, FiresecServiceFactory.UID);
			}, "GKTurnOnInAutomatic");
		}

		public void GKTurnOnNowInAutomatic(GKBase gkBase)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.GKTurnOnNowInAutomatic(gkBase.UID, gkBase.ObjectType, FiresecServiceFactory.UID);
			}, "GKTurnOnNowInAutomatic");
		}

		public void GKTurnOff(GKBase gkBase)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.GKTurnOff(gkBase.UID, gkBase.ObjectType, FiresecServiceFactory.UID);
			}, "GKTurnOff");
		}

		public void GKTurnOffNow(GKBase gkBase)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.GKTurnOffNow(gkBase.UID, gkBase.ObjectType, FiresecServiceFactory.UID);
			}, "GKTurnOffNow");
		}

		public void GKTurnOffInAutomatic(GKBase gkBase)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.GKTurnOffInAutomatic(gkBase.UID, gkBase.ObjectType, FiresecServiceFactory.UID);
			}, "GKTurnOffInAutomatic");
		}

		public void GKTurnOffNowInAutomatic(GKBase gkBase)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.GKTurnOffNowInAutomatic(gkBase.UID, gkBase.ObjectType, FiresecServiceFactory.UID);
			}, "GKTurnOffNowInAutomatic");
		}

		public void GKStop(GKBase gkBase)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.GKStop(gkBase.UID, gkBase.ObjectType, FiresecServiceFactory.UID);
			}, "GKStop");
		}

		public void GKStartMeasureMonitoring(GKDevice device)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.GKStartMeasureMonitoring(device.UID, FiresecServiceFactory.UID);
			}, "GKStartMeasureMonitoring");
		}

		public void GKStopMeasureMonitoring(GKDevice device)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.GKStopMeasureMonitoring(device.UID, FiresecServiceFactory.UID);
			}, "GKStopMeasureMonitoring");
		}

		public OperationResult<uint> GKGetReaderCode(GKDevice device)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GKGetReaderCode(device.UID, FiresecServiceFactory.UID);
			}, "GKGetReaderCode");
		}

		public void GKOpenSKDZone(GKSKDZone zone)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.GKOpenSKDZone(zone.UID, FiresecServiceFactory.UID);
			}, "GKOpenSKDZone");
		}

		public void GKCloseSKDZone(GKSKDZone zone)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.GKCloseSKDZone(zone.UID, FiresecServiceFactory.UID);
			}, "GKCloseSKDZone");
		}

		public OperationResult<CurrentConsumption> GetAlsMeasure(Guid alsUid)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetAlsMeasure(alsUid, FiresecServiceFactory.UID);
			}, "GetAlsMeasure");
		}

		public void GKAddMessage(JournalEventNameType journalEventNameType, string description)
		{
		}
	}
}