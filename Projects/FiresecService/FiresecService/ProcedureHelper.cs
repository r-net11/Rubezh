using FiresecAPI.Automation;
using FiresecAPI.GK;
using FiresecAPI.Journal;
using FiresecAPI.SKD;
using RubezhClient;
using FiresecService.Service;
using Infrastructure.Automation;
using Infrastructure.Common.BalloonTrayTip;
using RubezhDAL.DataClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiresecService
{
	public static class ProcedureHelper
	{
		public static void AddJournalItem(string message)
		{
			var journalItem = new JournalItem()
			{
				SystemDateTime = DateTime.Now,
				JournalEventNameType = JournalEventNameType.Сообщение_автоматизации,
				DescriptionText = message
			};
			Service.FiresecService.AddCommonJournalItems(new List<JournalItem>() { journalItem });
		}

		public static void ControlGKDevice(Guid deviceUid, GKStateBit command)
		{
			FiresecServiceManager.SafeFiresecService.GKExecuteDeviceCommand(deviceUid, command);
		}

		public static void StartRecord(Guid cameraUid, Guid? journalItemUid, Guid? eventUid, int timeout)
		{
			var camera = ProcedureExecutionContext.SystemConfiguration.Cameras.FirstOrDefault(x => x.UID == cameraUid);
			if (camera == null)
				return;
			if (journalItemUid.HasValue)
				using (var dbService = new DbService())
				{
					var operationResult = dbService.JournalTranslator.GetFilteredJournalItems(new JournalFilter() { UID = journalItemUid.Value });
					if (!operationResult.HasError)
					{
						var journalItem = operationResult.Result.FirstOrDefault();
						if (journalItem != null)
						{
							dbService.JournalTranslator.SaveVideoUID(journalItemUid.Value, eventUid.Value, cameraUid);
							journalItem.VideoUID = eventUid.Value;
							journalItem.CameraUID = cameraUid;
							FiresecService.Service.FiresecService.NotifyNewJournalItems(new List<JournalItem>() { journalItem });
						}
					}
				}
			RviClient.RviClientHelper.VideoRecordStart(ProcedureExecutionContext.SystemConfiguration, camera, eventUid.Value, timeout);
		}

		public static void StopRecord(Guid cameraUid, Guid eventUid)
		{
			var camera = ProcedureExecutionContext.SystemConfiguration.Cameras.FirstOrDefault(x => x.UID == cameraUid);
			if (camera == null)
				return;
			RviClient.RviClientHelper.VideoRecordStop(ProcedureExecutionContext.SystemConfiguration, camera, eventUid);
		}

		public static void Ptz(Guid cameraUid, int ptzNumber)
		{
			var camera = ProcedureExecutionContext.SystemConfiguration.Cameras.FirstOrDefault(x => x.UID == cameraUid);
			if (camera == null)
				return;
			RviClient.RviClientHelper.SetPtzPreset(ProcedureExecutionContext.SystemConfiguration, camera, ptzNumber);
		}

		public static void RviAlarm(string name)
		{
			RviClient.RviClientHelper.AlarmRuleExecute(ProcedureExecutionContext.SystemConfiguration, name);
		}

		public static void ControlFireZone(Guid uid, ZoneCommandType commandType)
		{
			if (commandType == ZoneCommandType.Ignore)
				FiresecServiceManager.SafeFiresecService.GKSetIgnoreRegime(uid, GKBaseObjectType.Zone);
			if (commandType == ZoneCommandType.ResetIgnore)
				FiresecServiceManager.SafeFiresecService.GKSetAutomaticRegime(uid, GKBaseObjectType.Zone);
			if (commandType == ZoneCommandType.Reset)
				FiresecServiceManager.SafeFiresecService.GKReset(uid, GKBaseObjectType.Zone);
		}

		public static void ControlGuardZone(Guid uid, GuardZoneCommandType commandType)
		{
			if (commandType == GuardZoneCommandType.Automatic)
				FiresecServiceManager.SafeFiresecService.GKSetAutomaticRegime(uid, GKBaseObjectType.GuardZone);
			if (commandType == GuardZoneCommandType.Ignore)
				FiresecServiceManager.SafeFiresecService.GKSetIgnoreRegime(uid, GKBaseObjectType.GuardZone);
			if (commandType == GuardZoneCommandType.Manual)
				FiresecServiceManager.SafeFiresecService.GKSetManualRegime(uid, GKBaseObjectType.GuardZone);
			if (commandType == GuardZoneCommandType.Reset)
				FiresecServiceManager.SafeFiresecService.GKReset(uid, GKBaseObjectType.GuardZone);
			if (commandType == GuardZoneCommandType.TurnOn)
				FiresecServiceManager.SafeFiresecService.GKTurnOn(uid, GKBaseObjectType.GuardZone);
			if (commandType == GuardZoneCommandType.TurnOnNow)
				FiresecServiceManager.SafeFiresecService.GKTurnOnNow(uid, GKBaseObjectType.GuardZone);
			if (commandType == GuardZoneCommandType.TurnOff)
				FiresecServiceManager.SafeFiresecService.GKTurnOff(uid, GKBaseObjectType.GuardZone);
			if (commandType == GuardZoneCommandType.TurnOffNow)
				FiresecServiceManager.SafeFiresecService.GKTurnOffNow(uid, GKBaseObjectType.GuardZone);
			if (commandType == GuardZoneCommandType.TurnOnInAutomatic)
				FiresecServiceManager.SafeFiresecService.GKTurnOnInAutomatic(uid, GKBaseObjectType.GuardZone);
			if (commandType == GuardZoneCommandType.TurnOnNowInAutomatic)
				FiresecServiceManager.SafeFiresecService.GKTurnOnNowInAutomatic(uid, GKBaseObjectType.GuardZone);
			if (commandType == GuardZoneCommandType.TurnOffInAutomatic)
				FiresecServiceManager.SafeFiresecService.GKTurnOffInAutomatic(uid, GKBaseObjectType.GuardZone);
			if (commandType == GuardZoneCommandType.TurnOffNowInAutomatic)
				FiresecServiceManager.SafeFiresecService.GKTurnOffNowInAutomatic(uid, GKBaseObjectType.GuardZone);
		}

		public static void ControlDirection(Guid uid, DirectionCommandType commandType)
		{
			if (commandType == DirectionCommandType.Automatic)
				FiresecServiceManager.SafeFiresecService.GKSetAutomaticRegime(uid, GKBaseObjectType.Direction);
			if (commandType == DirectionCommandType.ForbidStart)
				FiresecServiceManager.SafeFiresecService.GKStop(uid, GKBaseObjectType.Direction);
			if (commandType == DirectionCommandType.Ignore)
				FiresecServiceManager.SafeFiresecService.GKSetIgnoreRegime(uid, GKBaseObjectType.Direction);
			if (commandType == DirectionCommandType.Manual)
				FiresecServiceManager.SafeFiresecService.GKSetManualRegime(uid, GKBaseObjectType.Direction);
			if (commandType == DirectionCommandType.TurnOff)
				FiresecServiceManager.SafeFiresecService.GKTurnOff(uid, GKBaseObjectType.Direction);
			if (commandType == DirectionCommandType.TurnOn)
				FiresecServiceManager.SafeFiresecService.GKTurnOn(uid, GKBaseObjectType.Direction);
			if (commandType == DirectionCommandType.TurnOnNow)
				FiresecServiceManager.SafeFiresecService.GKTurnOnNow(uid, GKBaseObjectType.Direction);
		}

		public static void ControlGKDoor(Guid uid, GKDoorCommandType commandType)
		{
			var door = GKManager.Doors.FirstOrDefault(x => x.UID == uid);
			if (door == null)
				return;
			if (commandType == GKDoorCommandType.Open)
				FiresecServiceManager.SafeFiresecService.GKTurnOn(uid, GKBaseObjectType.Door);
			if (commandType == GKDoorCommandType.Close)
				FiresecServiceManager.SafeFiresecService.GKTurnOff(uid, GKBaseObjectType.Door);
			if (commandType == GKDoorCommandType.OpenInAutomatic)
				FiresecServiceManager.SafeFiresecService.GKTurnOnInAutomatic(uid, GKBaseObjectType.Door);
			if (commandType == GKDoorCommandType.CloseInAutomatic)
				FiresecServiceManager.SafeFiresecService.GKTurnOffInAutomatic(uid, GKBaseObjectType.Door);
			if (commandType == GKDoorCommandType.OpenNow)
				FiresecServiceManager.SafeFiresecService.GKTurnOnNow(uid, GKBaseObjectType.Door);
			if (commandType == GKDoorCommandType.CloseNow)
				FiresecServiceManager.SafeFiresecService.GKTurnOffNow(uid, GKBaseObjectType.Door);
			if (commandType == GKDoorCommandType.OpenNowInAutomatic)
				FiresecServiceManager.SafeFiresecService.GKTurnOnNowInAutomatic(uid, GKBaseObjectType.Door);
			if (commandType == GKDoorCommandType.CloseNowInAutomatic)
				FiresecServiceManager.SafeFiresecService.GKTurnOffNowInAutomatic(uid, GKBaseObjectType.Door);
			if (commandType == GKDoorCommandType.OpenForever)
			{
				FiresecServiceManager.SafeFiresecService.GKSetManualRegime(uid, GKBaseObjectType.Door);
				FiresecServiceManager.SafeFiresecService.GKTurnOn(uid, GKBaseObjectType.Door);
			}
			if (commandType == GKDoorCommandType.CloseForever)
			{
				FiresecServiceManager.SafeFiresecService.GKSetManualRegime(uid, GKBaseObjectType.Door);
				FiresecServiceManager.SafeFiresecService.GKTurnOff(uid, GKBaseObjectType.Door);
			}
			if (commandType == GKDoorCommandType.Norm)
			{
				FiresecServiceManager.SafeFiresecService.GKSetAutomaticRegime(uid, GKBaseObjectType.Door);
				FiresecServiceManager.SafeFiresecService.GKTurnOff(uid, GKBaseObjectType.Door);
			}
		}

		public static void ControlDelay(Guid uid, DelayCommandType commandType)
		{
			if (commandType == DelayCommandType.Automatic)
				FiresecServiceManager.SafeFiresecService.GKSetAutomaticRegime(uid, GKBaseObjectType.Delay);
			if (commandType == DelayCommandType.Ignore)
				FiresecServiceManager.SafeFiresecService.GKSetIgnoreRegime(uid, GKBaseObjectType.Delay);
			if (commandType == DelayCommandType.Manual)
				FiresecServiceManager.SafeFiresecService.GKSetManualRegime(uid, GKBaseObjectType.Delay);
			if (commandType == DelayCommandType.TurnOff)
				FiresecServiceManager.SafeFiresecService.GKTurnOff(uid, GKBaseObjectType.Delay);
			if (commandType == DelayCommandType.TurnOn)
				FiresecServiceManager.SafeFiresecService.GKTurnOn(uid, GKBaseObjectType.Delay);
			if (commandType == DelayCommandType.TurnOnNow)
				FiresecServiceManager.SafeFiresecService.GKTurnOnNow(uid, GKBaseObjectType.Delay);
		}

		public static void ExportJournal(bool isExportJournal, bool isExportPassJournal, DateTime minDate, DateTime maxDate, string path)
		{
			var result = FiresecServiceManager.SafeFiresecService.ExportJournal(
				new JournalExportFilter
				{
					IsExportJournal = isExportJournal,
					IsExportPassJournal = isExportPassJournal,
					MaxDate = maxDate,
					MinDate = minDate,
					Path = path
				});
			if (result.HasError)
				BalloonHelper.ShowFromServer("Экспорт журнала " + result.Error);
		}

		public static void ExportOrganisation(bool isWithDeleted, Guid organisationUid, string path)
		{
			var result = FiresecServiceManager.SafeFiresecService.ExportOrganisation(
				new ExportFilter
				{
					IsWithDeleted = isWithDeleted,
					OrganisationUID = organisationUid,
					Path = path
				});
			if (result.HasError)
				BalloonHelper.ShowFromServer(result.Error);
		}
		public static void ExportOrganisationList(bool isWithDeleted, string path)
		{
			var result = FiresecServiceManager.SafeFiresecService.ExportOrganisationList(
				new ExportFilter
				{
					IsWithDeleted = isWithDeleted,
					Path = path
				});
				if (result.HasError)
					BalloonHelper.ShowFromServer(result.Error);
		}

		public static void ExportConfiguration(bool isExportDevices, bool isExportDoors, bool isExportZones, string path)
		{
			var result = FiresecServiceManager.SafeFiresecService.ExportConfiguration(
				new ConfigurationExportFilter
				{
					IsExportDevices = isExportDevices,
					IsExportDoors = isExportDoors,
					IsExportZones = isExportZones,
					Path = path
				});
			if (result.HasError)
				BalloonHelper.ShowFromServer(result.Error);
		}
		public static void ImportOrganisation(bool isWithDeleted, string path)
		{
			var result = FiresecServiceManager.SafeFiresecService.ImportOrganisation(
				new ImportFilter
				{
					IsWithDeleted = isWithDeleted,
					Path = path
				});
			if (result.HasError)
				BalloonHelper.ShowFromServer(result.Error);
		}
		public static void ImportOrganisationList(bool isWithDeleted, string path)
		{
			var result = FiresecServiceManager.SafeFiresecService.ImportOrganisationList(
				new ImportFilter
				{
					IsWithDeleted = isWithDeleted,
					Path = path
				});
			if (result.HasError)
				BalloonHelper.ShowFromServer(result.Error);
		}
	}
}
