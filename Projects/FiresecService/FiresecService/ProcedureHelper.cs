using FiresecService.Service;
using Infrastructure.Automation;
using Infrastructure.Common.BalloonTrayTip;
using RubezhAPI;
using RubezhAPI.Automation;
using RubezhAPI.GK;
using RubezhAPI.Journal;
using RubezhAPI.SKD;
using RubezhDAL.DataClasses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FiresecService
{
	public static class ProcedureHelper
	{
		public static void AddJournalItem(string message, Guid clientUID)
		{
			var journalItem = new JournalItem()
			{
				SystemDateTime = DateTime.Now,
				JournalEventNameType = JournalEventNameType.Сообщение_автоматизации,
				DescriptionText = message
			};
			Service.FiresecService.AddCommonJournalItems(new List<JournalItem>() { journalItem });
		}

		public static void ControlGKDevice(Guid deviceUid, GKStateBit command, Guid clientUID)
		{
			FiresecServiceManager.SafeFiresecService.GKExecuteDeviceCommand(deviceUid, command, clientUID);
		}

		public static void StartRecord(Guid cameraUid, Guid? journalItemUid, Guid? eventUid, int timeout, Guid clientUID)
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
							FiresecService.Service.FiresecService.NotifyJournalItems(new List<JournalItem>() { journalItem }, false);
						}
					}
				}
			RviClient.RviClientHelper.VideoRecordStart(ProcedureExecutionContext.SystemConfiguration.RviSettings, camera, eventUid.Value, timeout);
		}

		public static void StopRecord(Guid cameraUid, Guid eventUid, Guid clientUID)
		{
			var camera = ProcedureExecutionContext.SystemConfiguration.Cameras.FirstOrDefault(x => x.UID == cameraUid);
			if (camera == null)
				return;
			RviClient.RviClientHelper.VideoRecordStop(ProcedureExecutionContext.SystemConfiguration.RviSettings, camera, eventUid);
		}

		public static void Ptz(Guid cameraUid, int ptzNumber, Guid clientUID)
		{
			var camera = ProcedureExecutionContext.SystemConfiguration.Cameras.FirstOrDefault(x => x.UID == cameraUid);
			if (camera == null)
				return;
			RviClient.RviClientHelper.SetPtzPreset(ProcedureExecutionContext.SystemConfiguration.RviSettings, camera, ptzNumber);
		}

		public static void RviAlarm(string name, Guid clientUID)
		{
			RviClient.RviClientHelper.AlarmRuleExecute(ProcedureExecutionContext.SystemConfiguration.RviSettings, name);
		}

		public static void ControlFireZone(Guid uid, ZoneCommandType commandType, Guid clientUID)
		{
			if (commandType == ZoneCommandType.Ignore)
				FiresecServiceManager.SafeFiresecService.GKSetIgnoreRegime(uid, GKBaseObjectType.Zone, clientUID);
			if (commandType == ZoneCommandType.ResetIgnore)
				FiresecServiceManager.SafeFiresecService.GKSetAutomaticRegime(uid, GKBaseObjectType.Zone, clientUID);
			if (commandType == ZoneCommandType.Reset)
				FiresecServiceManager.SafeFiresecService.GKReset(uid, GKBaseObjectType.Zone, clientUID);
		}

		public static void ControlGuardZone(Guid uid, GuardZoneCommandType commandType, Guid clientUID)
		{
			if (commandType == GuardZoneCommandType.Automatic)
				FiresecServiceManager.SafeFiresecService.GKSetAutomaticRegime(uid, GKBaseObjectType.GuardZone, clientUID);
			if (commandType == GuardZoneCommandType.Ignore)
				FiresecServiceManager.SafeFiresecService.GKSetIgnoreRegime(uid, GKBaseObjectType.GuardZone, clientUID);
			if (commandType == GuardZoneCommandType.Manual)
				FiresecServiceManager.SafeFiresecService.GKSetManualRegime(uid, GKBaseObjectType.GuardZone, clientUID);
			if (commandType == GuardZoneCommandType.Reset)
				FiresecServiceManager.SafeFiresecService.GKReset(uid, GKBaseObjectType.GuardZone, clientUID);
			if (commandType == GuardZoneCommandType.TurnOn)
				FiresecServiceManager.SafeFiresecService.GKTurnOn(uid, GKBaseObjectType.GuardZone, clientUID);
			if (commandType == GuardZoneCommandType.TurnOnNow)
				FiresecServiceManager.SafeFiresecService.GKTurnOnNow(uid, GKBaseObjectType.GuardZone, clientUID);
			if (commandType == GuardZoneCommandType.TurnOff)
				FiresecServiceManager.SafeFiresecService.GKTurnOff(uid, GKBaseObjectType.GuardZone, clientUID);
			if (commandType == GuardZoneCommandType.TurnOffNow)
				FiresecServiceManager.SafeFiresecService.GKTurnOffNow(uid, GKBaseObjectType.GuardZone, clientUID);
			if (commandType == GuardZoneCommandType.TurnOnInAutomatic)
				FiresecServiceManager.SafeFiresecService.GKTurnOnInAutomatic(uid, GKBaseObjectType.GuardZone, clientUID);
			if (commandType == GuardZoneCommandType.TurnOnNowInAutomatic)
				FiresecServiceManager.SafeFiresecService.GKTurnOnNowInAutomatic(uid, GKBaseObjectType.GuardZone, clientUID);
			if (commandType == GuardZoneCommandType.TurnOffInAutomatic)
				FiresecServiceManager.SafeFiresecService.GKTurnOffInAutomatic(uid, GKBaseObjectType.GuardZone, clientUID);
			if (commandType == GuardZoneCommandType.TurnOffNowInAutomatic)
				FiresecServiceManager.SafeFiresecService.GKTurnOffNowInAutomatic(uid, GKBaseObjectType.GuardZone, clientUID);
		}

		public static void ControlDirection(Guid uid, DirectionCommandType commandType, Guid clientUID)
		{
			if (commandType == DirectionCommandType.Automatic)
				FiresecServiceManager.SafeFiresecService.GKSetAutomaticRegime(uid, GKBaseObjectType.Direction, clientUID);
			if (commandType == DirectionCommandType.ForbidStart)
				FiresecServiceManager.SafeFiresecService.GKStop(uid, GKBaseObjectType.Direction, clientUID);
			if (commandType == DirectionCommandType.Ignore)
				FiresecServiceManager.SafeFiresecService.GKSetIgnoreRegime(uid, GKBaseObjectType.Direction, clientUID);
			if (commandType == DirectionCommandType.Manual)
				FiresecServiceManager.SafeFiresecService.GKSetManualRegime(uid, GKBaseObjectType.Direction, clientUID);
			if (commandType == DirectionCommandType.TurnOff)
				FiresecServiceManager.SafeFiresecService.GKTurnOff(uid, GKBaseObjectType.Direction, clientUID);
			if (commandType == DirectionCommandType.TurnOn)
				FiresecServiceManager.SafeFiresecService.GKTurnOn(uid, GKBaseObjectType.Direction, clientUID);
			if (commandType == DirectionCommandType.TurnOnNow)
				FiresecServiceManager.SafeFiresecService.GKTurnOnNow(uid, GKBaseObjectType.Direction, clientUID);
		}

		public static void ControlGKDoor(Guid uid, GKDoorCommandType commandType, Guid clientUID)
		{
			var door = GKManager.Doors.FirstOrDefault(x => x.UID == uid);
			if (door == null)
				return;
			if (commandType == GKDoorCommandType.Open)
				FiresecServiceManager.SafeFiresecService.GKTurnOn(uid, GKBaseObjectType.Door, clientUID);
			if (commandType == GKDoorCommandType.Close)
				FiresecServiceManager.SafeFiresecService.GKTurnOff(uid, GKBaseObjectType.Door, clientUID);
			if (commandType == GKDoorCommandType.OpenInAutomatic)
				FiresecServiceManager.SafeFiresecService.GKTurnOnInAutomatic(uid, GKBaseObjectType.Door, clientUID);
			if (commandType == GKDoorCommandType.CloseInAutomatic)
				FiresecServiceManager.SafeFiresecService.GKTurnOffInAutomatic(uid, GKBaseObjectType.Door, clientUID);
			if (commandType == GKDoorCommandType.OpenNow)
				FiresecServiceManager.SafeFiresecService.GKTurnOnNow(uid, GKBaseObjectType.Door, clientUID);
			if (commandType == GKDoorCommandType.CloseNow)
				FiresecServiceManager.SafeFiresecService.GKTurnOffNow(uid, GKBaseObjectType.Door, clientUID);
			if (commandType == GKDoorCommandType.OpenNowInAutomatic)
				FiresecServiceManager.SafeFiresecService.GKTurnOnNowInAutomatic(uid, GKBaseObjectType.Door, clientUID);
			if (commandType == GKDoorCommandType.CloseNowInAutomatic)
				FiresecServiceManager.SafeFiresecService.GKTurnOffNowInAutomatic(uid, GKBaseObjectType.Door, clientUID);
			if (commandType == GKDoorCommandType.OpenForever)
			{
				FiresecServiceManager.SafeFiresecService.GKSetManualRegime(uid, GKBaseObjectType.Door, clientUID);
				FiresecServiceManager.SafeFiresecService.GKTurnOn(uid, GKBaseObjectType.Door, clientUID);
			}
			if (commandType == GKDoorCommandType.CloseForever)
			{
				FiresecServiceManager.SafeFiresecService.GKSetManualRegime(uid, GKBaseObjectType.Door, clientUID);
				FiresecServiceManager.SafeFiresecService.GKTurnOff(uid, GKBaseObjectType.Door, clientUID);
			}
			if (commandType == GKDoorCommandType.Norm)
			{
				FiresecServiceManager.SafeFiresecService.GKTurnOff(uid, GKBaseObjectType.Door, clientUID);
				FiresecServiceManager.SafeFiresecService.GKSetAutomaticRegime(uid, GKBaseObjectType.Door, clientUID);
			}
		}

		public static void ControlDelay(Guid uid, DelayCommandType commandType, Guid clientUID)
		{
			if (commandType == DelayCommandType.Automatic)
				FiresecServiceManager.SafeFiresecService.GKSetAutomaticRegime(uid, GKBaseObjectType.Delay, clientUID);
			if (commandType == DelayCommandType.Ignore)
				FiresecServiceManager.SafeFiresecService.GKSetIgnoreRegime(uid, GKBaseObjectType.Delay, clientUID);
			if (commandType == DelayCommandType.Manual)
				FiresecServiceManager.SafeFiresecService.GKSetManualRegime(uid, GKBaseObjectType.Delay, clientUID);
			if (commandType == DelayCommandType.TurnOff)
				FiresecServiceManager.SafeFiresecService.GKTurnOff(uid, GKBaseObjectType.Delay, clientUID);
			if (commandType == DelayCommandType.TurnOn)
				FiresecServiceManager.SafeFiresecService.GKTurnOn(uid, GKBaseObjectType.Delay, clientUID);
			if (commandType == DelayCommandType.TurnOnNow)
				FiresecServiceManager.SafeFiresecService.GKTurnOnNow(uid, GKBaseObjectType.Delay, clientUID);
		}

		public static void ControlPumpStation(Guid uid, PumpStationCommandType commandType, Guid clientUID)
		{
			if (commandType == PumpStationCommandType.Automatic)
				FiresecServiceManager.SafeFiresecService.GKSetAutomaticRegime(uid, GKBaseObjectType.PumpStation, clientUID);
			if (commandType == PumpStationCommandType.Ignore)
				FiresecServiceManager.SafeFiresecService.GKSetIgnoreRegime(uid, GKBaseObjectType.PumpStation, clientUID);
			if (commandType == PumpStationCommandType.Manual)
				FiresecServiceManager.SafeFiresecService.GKSetManualRegime(uid, GKBaseObjectType.PumpStation, clientUID);
			if (commandType == PumpStationCommandType.TurnOff)
				FiresecServiceManager.SafeFiresecService.GKTurnOff(uid, GKBaseObjectType.PumpStation, clientUID);
			if (commandType == PumpStationCommandType.TurnOn)
				FiresecServiceManager.SafeFiresecService.GKTurnOn(uid, GKBaseObjectType.PumpStation, clientUID);
			if (commandType == PumpStationCommandType.Stop)
				FiresecServiceManager.SafeFiresecService.GKStop(uid, GKBaseObjectType.PumpStation, clientUID);
			if (commandType == PumpStationCommandType.ForbidStart)
				FiresecServiceManager.SafeFiresecService.GKStop(uid, GKBaseObjectType.PumpStation, clientUID);
		}

		public static void ControlMPT(Guid uid, MPTCommandType commandType, Guid clientUID)
		{
			if (commandType == MPTCommandType.Automatic)
				FiresecServiceManager.SafeFiresecService.GKSetAutomaticRegime(uid, GKBaseObjectType.MPT, clientUID);
			if (commandType == MPTCommandType.Ignore)
				FiresecServiceManager.SafeFiresecService.GKSetIgnoreRegime(uid, GKBaseObjectType.MPT, clientUID);
			if (commandType == MPTCommandType.Manual)
				FiresecServiceManager.SafeFiresecService.GKSetManualRegime(uid, GKBaseObjectType.MPT, clientUID);
			if (commandType == MPTCommandType.TurnOff)
				FiresecServiceManager.SafeFiresecService.GKTurnOff(uid, GKBaseObjectType.MPT, clientUID);
			if (commandType == MPTCommandType.TurnOn)
				FiresecServiceManager.SafeFiresecService.GKTurnOn(uid, GKBaseObjectType.MPT, clientUID);
			if (commandType == MPTCommandType.Stop)
				FiresecServiceManager.SafeFiresecService.GKStop(uid, GKBaseObjectType.MPT, clientUID);
			if (commandType == MPTCommandType.ForbidStart)
				FiresecServiceManager.SafeFiresecService.GKStop(uid, GKBaseObjectType.MPT, clientUID);
		}

		public static void ExportJournal(bool isExportJournal, bool isExportPassJournal, DateTime minDate, DateTime maxDate, string path, Guid clientUID)
		{
			var result = FiresecServiceManager.SafeFiresecService.ExportJournal(
				new JournalExportFilter
				{
					IsExportJournal = isExportJournal,
					IsExportPassJournal = isExportPassJournal,
					MaxDate = maxDate,
					MinDate = minDate,
					Path = path
				}, clientUID);
			if (result.HasError)
				BalloonHelper.ShowFromServer("Экспорт журнала " + result.Error);
		}

		public static void ExportOrganisation(bool isWithDeleted, Guid organisationUid, string path, Guid clientUID)
		{
			var result = FiresecServiceManager.SafeFiresecService.ExportOrganisation(
				new ExportFilter
				{
					IsWithDeleted = isWithDeleted,
					OrganisationUID = organisationUid,
					Path = path
				}, clientUID);
			if (result.HasError)
				BalloonHelper.ShowFromServer(result.Error);
		}
		public static void ExportOrganisationList(bool isWithDeleted, string path, Guid clientUID)
		{
			var result = FiresecServiceManager.SafeFiresecService.ExportOrganisationList(
				new ExportFilter
				{
					IsWithDeleted = isWithDeleted,
					Path = path
				}, clientUID);
			if (result.HasError)
				BalloonHelper.ShowFromServer(result.Error);
		}

		public static void ExportConfiguration(bool isExportDevices, bool isExportDoors, bool isExportZones, string path, Guid clientUID)
		{
			var result = FiresecServiceManager.SafeFiresecService.ExportConfiguration(
				new ConfigurationExportFilter
				{
					IsExportDevices = isExportDevices,
					IsExportDoors = isExportDoors,
					IsExportZones = isExportZones,
					Path = path
				}, clientUID);
			if (result.HasError)
				BalloonHelper.ShowFromServer(result.Error);
		}
		public static void ImportOrganisation(bool isWithDeleted, string path, Guid clientUID)
		{
			var result = FiresecServiceManager.SafeFiresecService.ImportOrganisation(
				new ImportFilter
				{
					IsWithDeleted = isWithDeleted,
					Path = path
				}, clientUID);
			if (result.HasError)
				BalloonHelper.ShowFromServer(result.Error);
		}
		public static void ImportOrganisationList(bool isWithDeleted, string path, Guid clientUID)
		{
			var result = FiresecServiceManager.SafeFiresecService.ImportOrganisationList(
				new ImportFilter
				{
					IsWithDeleted = isWithDeleted,
					Path = path
				}, clientUID);
			if (result.HasError)
				BalloonHelper.ShowFromServer(result.Error);
		}
	}
}
