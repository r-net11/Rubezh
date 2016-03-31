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
					firesecService.CancelGKProgress(FiresecServiceFactory.UID, progressCallbackUID, userName);
			}, "CancelGKProgress");
		}

		public OperationResult<bool> GKWriteConfiguration(GKDevice device)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(60));
				using (firesecService as IDisposable)
					return firesecService.GKWriteConfiguration(FiresecServiceFactory.UID, device.UID);
			}, "GKWriteConfiguration");
		}

		public OperationResult<GKDeviceConfiguration> GKReadConfiguration(GKDevice device)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(30));
				using (firesecService as IDisposable)
					return firesecService.GKReadConfiguration(FiresecServiceFactory.UID, device.UID);
			}, "GKReadConfiguration");
		}

		public OperationResult<bool> GKReadConfigurationFromGKFile(GKDevice device)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(30));
				using (firesecService as IDisposable)
					return firesecService.GKReadConfigurationFromGKFile(FiresecServiceFactory.UID, device.UID);
			}, "GKReadConfigurationFromGKFile");
		}

		public Stream GetServerFile(string filePath)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(30));
				using (firesecService as IDisposable)
					return firesecService.GetServerFile(FiresecServiceFactory.UID, filePath);
			}, "GetServerFile");
		}

		public OperationResult<GKDevice> GKAutoSearch(GKDevice device)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(120));
				using (firesecService as IDisposable)
					return firesecService.GKAutoSearch(FiresecServiceFactory.UID, device.UID);
			}, "GKAutoSearch");
		}

		public OperationResult<bool> GKUpdateFirmware(GKDevice device, List<byte> firmwareBytes)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(30));
				using (firesecService as IDisposable)
					return firesecService.GKUpdateFirmware(FiresecServiceFactory.UID, device.UID, firmwareBytes);
			}, "GKUpdateFirmware");
		}

		public OperationResult<bool> GKSyncronyseTime(GKDevice device)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GKSyncronyseTime(FiresecServiceFactory.UID, device.UID);
			}, "GKSyncronyseTime");
		}

		public OperationResult<string> GKGetDeviceInfo(GKDevice device)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GKGetDeviceInfo(FiresecServiceFactory.UID, device.UID);
			}, "GKGetDeviceInfo");
		}

		public OperationResult<int> GKGetJournalItemsCount(GKDevice device)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GKGetJournalItemsCount(FiresecServiceFactory.UID, device.UID);
			}, "GKGetJournalItemsCount");
		}

		public OperationResult<JournalItem> GKReadJournalItem(GKDevice device, int no)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GKReadJournalItem(FiresecServiceFactory.UID, device.UID, no);
			}, "GKReadJournalItem");
		}

		public OperationResult<bool> GKSetSingleParameter(GKBase gkBase, List<byte> parameterBytes, List<GKProperty> deviceProperties = null)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GKSetSingleParameter(FiresecServiceFactory.UID, gkBase.UID, parameterBytes, deviceProperties);
			}, "SetSingleParameter");
		}

		public OperationResult<List<GKProperty>> GKGetSingleParameter(GKBase gkBase)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GKGetSingleParameter(FiresecServiceFactory.UID, gkBase.UID);
			}, "GetSingleParameter");
		}

		public OperationResult<bool> GKRewriteAllSchedules(GKDevice device)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GKRewriteAllSchedules(FiresecServiceFactory.UID, device.UID);
			}, "GKRewriteAllSchedules");
		}

		public OperationResult<bool> GKSetSchedule(GKSchedule schedule)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GKSetSchedule(FiresecServiceFactory.UID, schedule);
			}, "GKSetSchedule");
		}

		public OperationResult<bool> GKGetUsers(GKDevice device)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(100));
				using (firesecService as IDisposable)
					return firesecService.GKGetUsers(FiresecServiceFactory.UID, device.UID);
			}, "GKGetUsers");
		}

		public OperationResult<bool> GKRewriteUsers(Guid deviceUid)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(1000));
				using (firesecService as IDisposable)
					return firesecService.GKRewriteUsers(FiresecServiceFactory.UID, deviceUid);
			}, "GKRewriteUsers");
		}

		public OperationResult<bool> RewritePmfUsers(Guid uid, List<GKUser> users)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(1000));
				using (firesecService as IDisposable)
					return firesecService.RewritePmfUsers(FiresecServiceFactory.UID, uid, users);
			}, "RewritePmfUsers");
		}

		public OperationResult<List<byte>> GKGKHash(GKDevice device)
		{
			return SafeOperationCall<List<byte>>(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GKGKHash(FiresecServiceFactory.UID, device.UID);
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

		public void GKExecuteDeviceCommand(GKDevice device, GKStateBit stateBit, string userName = null)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.GKExecuteDeviceCommand(FiresecServiceFactory.UID, device.UID, stateBit,userName);
			}, "GKExecuteDeviceCommand");
		}

		public void GKReset(GKBase gkBase, string userName = null)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.GKReset(FiresecServiceFactory.UID, gkBase.UID, gkBase.ObjectType, userName);
			}, "GKReset");
		}

		public void GKResetFire1(GKZone zone)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.GKResetFire1(FiresecServiceFactory.UID, zone.UID);
			}, "GKResetFire1");
		}

		public void GKResetFire2(GKZone zone)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.GKResetFire2(FiresecServiceFactory.UID, zone.UID);
			}, "GKResetFire2");
		}

		public void GKSetAutomaticRegime(GKBase gkBase, string userName = null)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.GKSetAutomaticRegime(FiresecServiceFactory.UID, gkBase.UID, gkBase.ObjectType, userName);
			}, "GKSetAutomaticRegime");
		}

		public void GKSetManualRegime(GKBase gkBase, string userName = null)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.GKSetManualRegime(FiresecServiceFactory.UID, gkBase.UID, gkBase.ObjectType, userName);
			}, "GKSetManualRegime");
		}

		public void GKSetIgnoreRegime(GKBase gkBase, string userName = null)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.GKSetIgnoreRegime(FiresecServiceFactory.UID, gkBase.UID, gkBase.ObjectType, userName);
			}, "GKTurnOn");
		}

		public void GKTurnOn(GKBase gkBase, string userName = null)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.GKTurnOn(FiresecServiceFactory.UID, gkBase.UID, gkBase.ObjectType, userName);
			}, "GKTurnOn");
		}

		public void GKTurnOnNow(GKBase gkBase, string userName = null)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.GKTurnOnNow(FiresecServiceFactory.UID, gkBase.UID, gkBase.ObjectType, userName);
			}, "GKTurnOnNow");
		}

		public void GKTurnOnInAutomatic(GKBase gkBase)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.GKTurnOnInAutomatic(FiresecServiceFactory.UID, gkBase.UID, gkBase.ObjectType);
			}, "GKTurnOnInAutomatic");
		}

		public void GKTurnOnNowInAutomatic(GKBase gkBase)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.GKTurnOnNowInAutomatic(FiresecServiceFactory.UID, gkBase.UID, gkBase.ObjectType);
			}, "GKTurnOnNowInAutomatic");
		}

		public void GKTurnOff(GKBase gkBase, string userName = null)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.GKTurnOff(FiresecServiceFactory.UID, gkBase.UID, gkBase.ObjectType, userName);
			}, "GKTurnOff");
		}

		public void GKTurnOffNow(GKBase gkBase, string userName = null)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.GKTurnOffNow(FiresecServiceFactory.UID, gkBase.UID, gkBase.ObjectType, userName);
			}, "GKTurnOffNow");
		}

		public void GKTurnOffInAutomatic(GKBase gkBase, string userName = null)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.GKTurnOffInAutomatic(FiresecServiceFactory.UID, gkBase.UID, gkBase.ObjectType, userName);
			}, "GKTurnOffInAutomatic");
		}

		public void GKTurnOffNowInAutomatic(GKBase gkBase)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.GKTurnOffNowInAutomatic(FiresecServiceFactory.UID, gkBase.UID, gkBase.ObjectType);
			}, "GKTurnOffNowInAutomatic");
		}

		public void GKTurnOnNowGlobalPimsInAutomatic()
		{
			SafeOperationCall(() =>
				{
					var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
					using (firesecService as IDisposable)
						firesecService.GKTurnOnNowGlobalPimsInAutomatic(FiresecServiceFactory.UID);
				}, "GKTurnOnNowGlobalPimsInAutomatic");
		}

		public void GKTurnOffNowGlobalPimsInAutomatic()
		{
			SafeOperationCall(() =>
				{
					var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
					using (firesecService as IDisposable)
						firesecService.GKTurnOffNowGlobalPimsInAutomatic(FiresecServiceFactory.UID);
				}
				, "GKTurnOffNowGlobalPimsInAutomatic");
		}

		public void GKStop(GKBase gkBase, string userName = null)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.GKStop(FiresecServiceFactory.UID, gkBase.UID, gkBase.ObjectType, userName);
			}, "GKStop");
		}

		public void GKStartMeasureMonitoring(GKDevice device)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.GKStartMeasureMonitoring(FiresecServiceFactory.UID, device.UID);
			}, "GKStartMeasureMonitoring");
		}

		public void GKStopMeasureMonitoring(GKDevice device)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.GKStopMeasureMonitoring(FiresecServiceFactory.UID, device.UID);
			}, "GKStopMeasureMonitoring");
		}

		public OperationResult<uint> GKGetReaderCode(GKDevice device)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GKGetReaderCode(FiresecServiceFactory.UID, device.UID);
			}, "GKGetReaderCode");
		}

		public void GKOpenSKDZone(GKSKDZone zone)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.GKOpenSKDZone(FiresecServiceFactory.UID, zone.UID);
			}, "GKOpenSKDZone");
		}

		public void GKCloseSKDZone(GKSKDZone zone)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.GKCloseSKDZone(FiresecServiceFactory.UID, zone.UID);
			}, "GKCloseSKDZone");
		}

		public OperationResult<CurrentConsumption> GetAlsMeasure(Guid alsUid)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetAlsMeasure(FiresecServiceFactory.UID, alsUid);
			}, "GetAlsMeasure");
		}

		public void GKAddMessage(JournalEventNameType journalEventNameType, string description)
		{
		}
	}
}