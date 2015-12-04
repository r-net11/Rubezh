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
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					firesecService.CancelGKProgress(progressCallbackUID, userName);
			}, "CancelGKProgress");
		}

		public OperationResult<bool> GKWriteConfiguration(GKDevice device)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GKWriteConfiguration(device.UID);
			}, "GKWriteConfiguration");
		}

		public OperationResult<GKDeviceConfiguration> GKReadConfiguration(GKDevice device)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GKReadConfiguration(device.UID);
			}, "GKReadConfiguration");
		}

		public OperationResult<bool> GKReadConfigurationFromGKFile(GKDevice device)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GKReadConfigurationFromGKFile(device.UID);
			}, "GKReadConfigurationFromGKFile");
		}

		public Stream GetServerFile(string filePath)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GetServerFile(filePath);
			}, "GetServerFile");
		}

		public OperationResult<GKDevice> GKAutoSearch(GKDevice device)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GKAutoSearch(device.UID);
			}, "GKAutoSearch");
		}

		public OperationResult<bool> GKUpdateFirmware(GKDevice device, List<byte> firmwareBytes)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GKUpdateFirmware(device.UID, firmwareBytes);
			}, "GKUpdateFirmware");
		}

		public OperationResult<bool> GKSyncronyseTime(GKDevice device)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GKSyncronyseTime(device.UID);
			}, "GKSyncronyseTime");
		}

		public OperationResult<string> GKGetDeviceInfo(GKDevice device)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GKGetDeviceInfo(device.UID);
			}, "GKGetDeviceInfo");
		}

		public OperationResult<int> GKGetJournalItemsCount(GKDevice device)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GKGetJournalItemsCount(device.UID);
			}, "GKGetJournalItemsCount");
		}

		public OperationResult<JournalItem> GKReadJournalItem(GKDevice device, int no)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GKReadJournalItem(device.UID, no);
			}, "GKReadJournalItem");
		}

		public OperationResult<bool> GKSetSingleParameter(GKBase gkBase, List<byte> parameterBytes, List<GKProperty> deviceProperties = null)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GKSetSingleParameter(gkBase.UID, parameterBytes, deviceProperties);
			}, "SetSingleParameter");
		}

		public OperationResult<List<GKProperty>> GKGetSingleParameter(GKBase gkBase)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GKGetSingleParameter(gkBase.UID);
			}, "GetSingleParameter");
		}

		public OperationResult<bool> GKRewriteAllSchedules(GKDevice device)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GKRewriteAllSchedules(device.UID);
			}, "GKRewriteAllSchedules");
		}

		public OperationResult<bool> GKSetSchedule(GKSchedule schedule)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GKSetSchedule(schedule);
			}, "GKSetSchedule");
		}

		public OperationResult<bool> GKGetUsers(GKDevice device)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GKGetUsers(device.UID);
			}, "GKGetUsers");
		}

		public OperationResult<bool> GKRewriteUsers(GKDevice device)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GKRewriteUsers(device.UID);
			}, "GKRewriteUsers");
		}

		public OperationResult<List<MirrorUser>> GKReadMirrorUsers(GKDevice device)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GKReadMirrorUsers(device.UID);
			}, "GKReadMirrorUsers");
		}

		public OperationResult<bool> GKWriteMirrorUsers(GKDevice device, List<MirrorUser> mirrorUsers)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GKWriteMirrorUsers(device.UID, mirrorUsers);
			}, "GKWriteMirrorUsers");
		}

		public OperationResult<List<GKUser>> GetGKUsers(Guid deviceUID)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GetGKUsers(deviceUID);
			}, "GetGKUsers");
		}

		public OperationResult<bool> RewritePmfUsers(Guid uid, List<GKUser> users)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.RewritePmfUsers(uid, users);
			}, "RewritePmfUsers");
		}

		public OperationResult<List<byte>> GKGKHash(GKDevice device)
		{
			return SafeOperationCall<List<byte>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GKGKHash(device.UID);
			}, "GKGKHash");
		}

		public GKStates GKGetStates()
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GKGetStates();
			}, "GKGetStates");
		}

		public void GKExecuteDeviceCommand(GKDevice device, GKStateBit stateBit)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					firesecService.GKExecuteDeviceCommand(device.UID, stateBit);
			}, "GKExecuteDeviceCommand");
		}

		public void GKReset(GKBase gkBase)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					firesecService.GKReset(gkBase.UID, gkBase.ObjectType);
			}, "GKReset");
		}

		public void GKResetFire1(GKZone zone)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					firesecService.GKResetFire1(zone.UID);
			}, "GKResetFire1");
		}

		public void GKResetFire2(GKZone zone)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					firesecService.GKResetFire2(zone.UID);
			}, "GKResetFire2");
		}

		public void GKSetAutomaticRegime(GKBase gkBase)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					firesecService.GKSetAutomaticRegime(gkBase.UID, gkBase.ObjectType);
			}, "GKSetAutomaticRegime");
		}

		public void GKSetManualRegime(GKBase gkBase)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					firesecService.GKSetManualRegime(gkBase.UID, gkBase.ObjectType);
			}, "GKSetManualRegime");
		}

		public void GKSetIgnoreRegime(GKBase gkBase)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					firesecService.GKSetIgnoreRegime(gkBase.UID, gkBase.ObjectType);
			}, "GKTurnOn");
		}

		public void GKTurnOn(GKBase gkBase)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					firesecService.GKTurnOn(gkBase.UID, gkBase.ObjectType);
			}, "GKTurnOn");
		}

		public void GKTurnOnNow(GKBase gkBase)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					firesecService.GKTurnOnNow(gkBase.UID, gkBase.ObjectType);
			}, "GKTurnOnNow");
		}

		public void GKTurnOnInAutomatic(GKBase gkBase)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					firesecService.GKTurnOnInAutomatic(gkBase.UID, gkBase.ObjectType);
			}, "GKTurnOnInAutomatic");
		}

		public void GKTurnOnNowInAutomatic(GKBase gkBase)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					firesecService.GKTurnOnNowInAutomatic(gkBase.UID, gkBase.ObjectType);
			}, "GKTurnOnNowInAutomatic");
		}

		public void GKTurnOff(GKBase gkBase)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					firesecService.GKTurnOff(gkBase.UID, gkBase.ObjectType);
			}, "GKTurnOff");
		}

		public void GKTurnOffNow(GKBase gkBase)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					firesecService.GKTurnOffNow(gkBase.UID, gkBase.ObjectType);
			}, "GKTurnOffNow");
		}

		public void GKTurnOffInAutomatic(GKBase gkBase)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					firesecService.GKTurnOffInAutomatic(gkBase.UID, gkBase.ObjectType);
			}, "GKTurnOffInAutomatic");
		}

		public void GKTurnOffNowInAutomatic(GKBase gkBase)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					firesecService.GKTurnOffNowInAutomatic(gkBase.UID, gkBase.ObjectType);
			}, "GKTurnOffNowInAutomatic");
		}

		public void GKStop(GKBase gkBase)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					firesecService.GKStop(gkBase.UID, gkBase.ObjectType);
			}, "GKStop");
		}

		public void GKStartMeasureMonitoring(GKDevice device)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					firesecService.GKStartMeasureMonitoring(device.UID);
			}, "GKStartMeasureMonitoring");
		}

		public void GKStopMeasureMonitoring(GKDevice device)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					firesecService.GKStopMeasureMonitoring(device.UID);
			}, "GKStopMeasureMonitoring");
		}

		public OperationResult<uint> GKGetReaderCode(GKDevice device)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GKGetReaderCode(device.UID);
			}, "GKGetReaderCode");
		}

		public void GKOpenSKDZone(GKSKDZone zone)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					firesecService.GKOpenSKDZone(zone.UID);
			}, "GKOpenSKDZone");
		}

		public void GKCloseSKDZone(GKSKDZone zone)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					firesecService.GKCloseSKDZone(zone.UID);
			}, "GKCloseSKDZone");
		}

		public OperationResult<CurrentConsumption> GetAlsMeasure(Guid alsUid)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromSeconds(30));
				using (firesecService as IDisposable)
					return firesecService.GetAlsMeasure(alsUid);
			}, "GetAlsMeasure");
		}

		public void GKAddMessage(JournalEventNameType journalEventNameType, string description)
		{
		}
	}
}