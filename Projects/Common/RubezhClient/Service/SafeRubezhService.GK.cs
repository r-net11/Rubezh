using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Journal;
using System;
using System.Collections.Generic;
using System.IO;

namespace RubezhClient
{
	public partial class SafeRubezhService
	{
		public void CancelGKProgress(Guid progressCallbackUID, string userName)
		{
			SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					rubezhService.CancelGKProgress(RubezhServiceFactory.UID, progressCallbackUID, userName);
			}, "CancelGKProgress");
		}

		public OperationResult<bool> GKWriteConfiguration(GKDevice device)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(60));
				using (rubezhService as IDisposable)
					return rubezhService.GKWriteConfiguration(RubezhServiceFactory.UID, device.UID);
			}, "GKWriteConfiguration");
		}

		public OperationResult<GKDeviceConfiguration> GKReadConfiguration(GKDevice device)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(30));
				using (rubezhService as IDisposable)
					return rubezhService.GKReadConfiguration(RubezhServiceFactory.UID, device.UID);
			}, "GKReadConfiguration");
		}

		public OperationResult<bool> GKReadConfigurationFromGKFile(GKDevice device)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(30));
				using (rubezhService as IDisposable)
					return rubezhService.GKReadConfigurationFromGKFile(RubezhServiceFactory.UID, device.UID);
			}, "GKReadConfigurationFromGKFile");
		}

		public Stream GetServerFile(string filePath)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(30));
				using (rubezhService as IDisposable)
					return rubezhService.GetServerFile(RubezhServiceFactory.UID, filePath);
			}, "GetServerFile");
		}

		public OperationResult<GKDevice> GKAutoSearch(GKDevice device)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(120));
				using (rubezhService as IDisposable)
					return rubezhService.GKAutoSearch(RubezhServiceFactory.UID, device.UID);
			}, "GKAutoSearch");
		}

		public OperationResult<bool> GKUpdateFirmware(GKDevice device, List<byte> firmwareBytes)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(30));
				using (rubezhService as IDisposable)
					return rubezhService.GKUpdateFirmware(RubezhServiceFactory.UID, device.UID, firmwareBytes);
			}, "GKUpdateFirmware");
		}

		public OperationResult<bool> GKSyncronyseTime(GKDevice device)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.GKSyncronyseTime(RubezhServiceFactory.UID, device.UID);
			}, "GKSyncronyseTime");
		}

		public OperationResult<string> GKGetDeviceInfo(GKDevice device)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.GKGetDeviceInfo(RubezhServiceFactory.UID, device.UID);
			}, "GKGetDeviceInfo");
		}

		public OperationResult<int> GKGetJournalItemsCount(GKDevice device)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.GKGetJournalItemsCount(RubezhServiceFactory.UID, device.UID);
			}, "GKGetJournalItemsCount");
		}

		public OperationResult<JournalItem> GKReadJournalItem(GKDevice device, int no)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.GKReadJournalItem(RubezhServiceFactory.UID, device.UID, no);
			}, "GKReadJournalItem");
		}

		public OperationResult<bool> GKSetSingleParameter(GKBase gkBase, List<byte> parameterBytes, List<GKProperty> deviceProperties = null)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.GKSetSingleParameter(RubezhServiceFactory.UID, gkBase.UID, parameterBytes, deviceProperties);
			}, "SetSingleParameter");
		}

		public OperationResult<List<GKProperty>> GKGetSingleParameter(GKBase gkBase)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.GKGetSingleParameter(RubezhServiceFactory.UID, gkBase.UID);
			}, "GetSingleParameter");
		}

		public OperationResult<bool> GKRewriteAllSchedules(GKDevice device)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.GKRewriteAllSchedules(RubezhServiceFactory.UID, device.UID);
			}, "GKRewriteAllSchedules");
		}

		public OperationResult<bool> GKSetSchedule(GKSchedule schedule)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.GKSetSchedule(RubezhServiceFactory.UID, schedule);
			}, "GKSetSchedule");
		}

		public OperationResult<bool> GKGetUsers(GKDevice device)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(100));
				using (rubezhService as IDisposable)
					return rubezhService.GKGetUsers(RubezhServiceFactory.UID, device.UID);
			}, "GKGetUsers");
		}

		public OperationResult<bool> GKRewriteUsers(Guid deviceUid)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(1000));
				using (rubezhService as IDisposable)
					return rubezhService.GKRewriteUsers(RubezhServiceFactory.UID, deviceUid);
			}, "GKRewriteUsers");
		}

		public OperationResult<bool> RewritePmfUsers(Guid uid, List<GKUser> users)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(1000));
				using (rubezhService as IDisposable)
					return rubezhService.RewritePmfUsers(RubezhServiceFactory.UID, uid, users);
			}, "RewritePmfUsers");
		}

		public OperationResult<List<byte>> GKGKHash(GKDevice device)
		{
			return SafeOperationCall<List<byte>>(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.GKGKHash(RubezhServiceFactory.UID, device.UID);
			}, "GKGKHash");
		}

		public GKStates GKGetStates()
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.GKGetStates(RubezhServiceFactory.UID);
			}, "GKGetStates");
		}

		public void GKExecuteDeviceCommand(GKDevice device, GKStateBit stateBit, string userName = null)
		{
			SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					rubezhService.GKExecuteDeviceCommand(RubezhServiceFactory.UID, device.UID, stateBit,userName);
			}, "GKExecuteDeviceCommand");
		}

		public void GKReset(GKBase gkBase, string userName = null)
		{
			SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					rubezhService.GKReset(RubezhServiceFactory.UID, gkBase.UID, gkBase.ObjectType, userName);
			}, "GKReset");
		}

		public void GKResetFire1(GKZone zone)
		{
			SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					rubezhService.GKResetFire1(RubezhServiceFactory.UID, zone.UID);
			}, "GKResetFire1");
		}

		public void GKResetFire2(GKZone zone)
		{
			SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					rubezhService.GKResetFire2(RubezhServiceFactory.UID, zone.UID);
			}, "GKResetFire2");
		}

		public void GKSetAutomaticRegime(GKBase gkBase, string userName = null)
		{
			SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					rubezhService.GKSetAutomaticRegime(RubezhServiceFactory.UID, gkBase.UID, gkBase.ObjectType, userName);
			}, "GKSetAutomaticRegime");
		}

		public void GKSetManualRegime(GKBase gkBase, string userName = null)
		{
			SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					rubezhService.GKSetManualRegime(RubezhServiceFactory.UID, gkBase.UID, gkBase.ObjectType, userName);
			}, "GKSetManualRegime");
		}

		public void GKSetIgnoreRegime(GKBase gkBase, string userName = null)
		{
			SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					rubezhService.GKSetIgnoreRegime(RubezhServiceFactory.UID, gkBase.UID, gkBase.ObjectType, userName);
			}, "GKTurnOn");
		}

		public void GKTurnOn(GKBase gkBase, string userName = null)
		{
			SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					rubezhService.GKTurnOn(RubezhServiceFactory.UID, gkBase.UID, gkBase.ObjectType, userName);
			}, "GKTurnOn");
		}

		public void GKTurnOnNow(GKBase gkBase, string userName = null)
		{
			SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					rubezhService.GKTurnOnNow(RubezhServiceFactory.UID, gkBase.UID, gkBase.ObjectType, userName);
			}, "GKTurnOnNow");
		}

		public void GKTurnOnInAutomatic(GKBase gkBase)
		{
			SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					rubezhService.GKTurnOnInAutomatic(RubezhServiceFactory.UID, gkBase.UID, gkBase.ObjectType);
			}, "GKTurnOnInAutomatic");
		}

		public void GKTurnOnNowInAutomatic(GKBase gkBase)
		{
			SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					rubezhService.GKTurnOnNowInAutomatic(RubezhServiceFactory.UID, gkBase.UID, gkBase.ObjectType);
			}, "GKTurnOnNowInAutomatic");
		}

		public void GKTurnOff(GKBase gkBase, string userName = null)
		{
			SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					rubezhService.GKTurnOff(RubezhServiceFactory.UID, gkBase.UID, gkBase.ObjectType, userName);
			}, "GKTurnOff");
		}

		public void GKTurnOffNow(GKBase gkBase, string userName = null)
		{
			SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					rubezhService.GKTurnOffNow(RubezhServiceFactory.UID, gkBase.UID, gkBase.ObjectType, userName);
			}, "GKTurnOffNow");
		}

		public void GKTurnOffInAutomatic(GKBase gkBase, string userName = null)
		{
			SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					rubezhService.GKTurnOffInAutomatic(RubezhServiceFactory.UID, gkBase.UID, gkBase.ObjectType, userName);
			}, "GKTurnOffInAutomatic");
		}

		public void GKTurnOffNowInAutomatic(GKBase gkBase)
		{
			SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					rubezhService.GKTurnOffNowInAutomatic(RubezhServiceFactory.UID, gkBase.UID, gkBase.ObjectType);
			}, "GKTurnOffNowInAutomatic");
		}

		public void GKTurnOnNowGlobalPimsInAutomatic()
		{
			SafeOperationCall(() =>
				{
					var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
					using (rubezhService as IDisposable)
						rubezhService.GKTurnOnNowGlobalPimsInAutomatic(RubezhServiceFactory.UID);
				}, "GKTurnOnNowGlobalPimsInAutomatic");
		}

		public void GKTurnOffNowGlobalPimsInAutomatic()
		{
			SafeOperationCall(() =>
				{
					var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
					using (rubezhService as IDisposable)
						rubezhService.GKTurnOffNowGlobalPimsInAutomatic(RubezhServiceFactory.UID);
				}
				, "GKTurnOffNowGlobalPimsInAutomatic");
		}

		public void GKStop(GKBase gkBase, string userName = null)
		{
			SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					rubezhService.GKStop(RubezhServiceFactory.UID, gkBase.UID, gkBase.ObjectType, userName);
			}, "GKStop");
		}

		public void SendOn2OPKS(GKBase gkBase, string userName = null)
		{
			SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					rubezhService.SendOn2OPKS(RubezhServiceFactory.UID, gkBase.UID, gkBase.ObjectType, userName);
			}, "SendOn2OPKS");
		}

		public void GKStartMeasureMonitoring(GKDevice device)
		{
			SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					rubezhService.GKStartMeasureMonitoring(RubezhServiceFactory.UID, device.UID);
			}, "GKStartMeasureMonitoring");
		}

		public void GKStopMeasureMonitoring(GKDevice device)
		{
			SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					rubezhService.GKStopMeasureMonitoring(RubezhServiceFactory.UID, device.UID);
			}, "GKStopMeasureMonitoring");
		}

		public OperationResult<uint> GKGetReaderCode(GKDevice device)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.GKGetReaderCode(RubezhServiceFactory.UID, device.UID);
			}, "GKGetReaderCode");
		}

		public void GKOpenSKDZone(GKSKDZone zone)
		{
			SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					rubezhService.GKOpenSKDZone(RubezhServiceFactory.UID, zone.UID);
			}, "GKOpenSKDZone");
		}

		public void GKCloseSKDZone(GKSKDZone zone)
		{
			SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					rubezhService.GKCloseSKDZone(RubezhServiceFactory.UID, zone.UID);
			}, "GKCloseSKDZone");
		}

		public OperationResult<CurrentConsumption> GetAlsMeasure(Guid alsUid)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.GetAlsMeasure(RubezhServiceFactory.UID, alsUid);
			}, "GetAlsMeasure");
		}

		public void GKAddMessage(JournalEventNameType journalEventNameType, string description)
		{
		}
	}
}