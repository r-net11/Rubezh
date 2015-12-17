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
		public static void AddJournalItem(Guid clientUID, string message)
		{
			var journalItem = new JournalItem()
			{
				SystemDateTime = DateTime.Now,
				JournalEventNameType = JournalEventNameType.Сообщение_автоматизации,
				DescriptionText = message
			};
			Service.FiresecService.AddCommonJournalItems(new List<JournalItem>() { journalItem }, clientUID);
		}

		public static void ControlGKDevice(Guid clientUID, Guid deviceUid, GKStateBit command)
		{
			FiresecServiceManager.SafeFiresecService.GKExecuteDeviceCommand(clientUID, deviceUid, command);
		}

		public static void StartRecord(Guid clientUID, Guid cameraUid, Guid? journalItemUid, Guid? eventUid, int timeout)
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

		public static void StopRecord(Guid clientUID, Guid cameraUid, Guid eventUid)
		{
			var camera = ProcedureExecutionContext.SystemConfiguration.Cameras.FirstOrDefault(x => x.UID == cameraUid);
			if (camera == null)
				return;
			RviClient.RviClientHelper.VideoRecordStop(ProcedureExecutionContext.SystemConfiguration.RviSettings, camera, eventUid);
		}

		public static void Ptz(Guid clientUID, Guid cameraUid, int ptzNumber)
		{
			var camera = ProcedureExecutionContext.SystemConfiguration.Cameras.FirstOrDefault(x => x.UID == cameraUid);
			if (camera == null)
				return;
			RviClient.RviClientHelper.SetPtzPreset(ProcedureExecutionContext.SystemConfiguration.RviSettings, camera, ptzNumber);
		}

		public static void RviAlarm(Guid clientUID, string name)
		{
			RviClient.RviClientHelper.AlarmRuleExecute(ProcedureExecutionContext.SystemConfiguration.RviSettings, name);
		}

		public static void ControlFireZone(Guid clientUID, Guid uid, ZoneCommandType commandType)
		{
			if (commandType == ZoneCommandType.Ignore)
				FiresecServiceManager.SafeFiresecService.GKSetIgnoreRegime(clientUID, uid, GKBaseObjectType.Zone);
			if (commandType == ZoneCommandType.ResetIgnore)
				FiresecServiceManager.SafeFiresecService.GKSetAutomaticRegime(clientUID, uid, GKBaseObjectType.Zone);
			if (commandType == ZoneCommandType.Reset)
				FiresecServiceManager.SafeFiresecService.GKReset(clientUID, uid, GKBaseObjectType.Zone);
		}

		public static void ControlGuardZone(Guid clientUID, Guid uid, GuardZoneCommandType commandType)
		{
			if (commandType == GuardZoneCommandType.Automatic)
				FiresecServiceManager.SafeFiresecService.GKSetAutomaticRegime(clientUID, uid, GKBaseObjectType.GuardZone);
			if (commandType == GuardZoneCommandType.Ignore)
				FiresecServiceManager.SafeFiresecService.GKSetIgnoreRegime(clientUID, uid, GKBaseObjectType.GuardZone);
			if (commandType == GuardZoneCommandType.Manual)
				FiresecServiceManager.SafeFiresecService.GKSetManualRegime(clientUID, uid, GKBaseObjectType.GuardZone);
			if (commandType == GuardZoneCommandType.Reset)
				FiresecServiceManager.SafeFiresecService.GKReset(clientUID, uid, GKBaseObjectType.GuardZone);
			if (commandType == GuardZoneCommandType.TurnOn)
				FiresecServiceManager.SafeFiresecService.GKTurnOn(clientUID, uid, GKBaseObjectType.GuardZone);
			if (commandType == GuardZoneCommandType.TurnOnNow)
				FiresecServiceManager.SafeFiresecService.GKTurnOnNow(clientUID, uid, GKBaseObjectType.GuardZone);
			if (commandType == GuardZoneCommandType.TurnOff)
				FiresecServiceManager.SafeFiresecService.GKTurnOff(clientUID, uid, GKBaseObjectType.GuardZone);
			if (commandType == GuardZoneCommandType.TurnOffNow)
				FiresecServiceManager.SafeFiresecService.GKTurnOffNow(clientUID, uid, GKBaseObjectType.GuardZone);
			if (commandType == GuardZoneCommandType.TurnOnInAutomatic)
				FiresecServiceManager.SafeFiresecService.GKTurnOnInAutomatic(clientUID, uid, GKBaseObjectType.GuardZone);
			if (commandType == GuardZoneCommandType.TurnOnNowInAutomatic)
				FiresecServiceManager.SafeFiresecService.GKTurnOnNowInAutomatic(clientUID, uid, GKBaseObjectType.GuardZone);
			if (commandType == GuardZoneCommandType.TurnOffInAutomatic)
				FiresecServiceManager.SafeFiresecService.GKTurnOffInAutomatic(clientUID, uid, GKBaseObjectType.GuardZone);
			if (commandType == GuardZoneCommandType.TurnOffNowInAutomatic)
				FiresecServiceManager.SafeFiresecService.GKTurnOffNowInAutomatic(clientUID, uid, GKBaseObjectType.GuardZone);
		}

		public static void ControlDirection(Guid clientUID, Guid uid, DirectionCommandType commandType)
		{
			if (commandType == DirectionCommandType.Automatic)
				FiresecServiceManager.SafeFiresecService.GKSetAutomaticRegime(clientUID, uid, GKBaseObjectType.Direction);
			if (commandType == DirectionCommandType.ForbidStart)
				FiresecServiceManager.SafeFiresecService.GKStop(clientUID, uid, GKBaseObjectType.Direction);
			if (commandType == DirectionCommandType.Ignore)
				FiresecServiceManager.SafeFiresecService.GKSetIgnoreRegime(clientUID, uid, GKBaseObjectType.Direction);
			if (commandType == DirectionCommandType.Manual)
				FiresecServiceManager.SafeFiresecService.GKSetManualRegime(clientUID, uid, GKBaseObjectType.Direction);
			if (commandType == DirectionCommandType.TurnOn)
				FiresecServiceManager.SafeFiresecService.GKTurnOn(clientUID, uid, GKBaseObjectType.Direction);
			if (commandType == DirectionCommandType.TurnOff)
				FiresecServiceManager.SafeFiresecService.GKTurnOff(clientUID, uid, GKBaseObjectType.Direction);
			if (commandType == DirectionCommandType.TurnOnNow)
				FiresecServiceManager.SafeFiresecService.GKTurnOnNow(clientUID, uid, GKBaseObjectType.Direction);
			if (commandType == DirectionCommandType.TurnOffNow)
				FiresecServiceManager.SafeFiresecService.GKTurnOffNow(clientUID, uid, GKBaseObjectType.Direction);
			if (commandType == DirectionCommandType.TurnOn_InAutomatic)
				FiresecServiceManager.SafeFiresecService.GKTurnOnInAutomatic(clientUID, uid, GKBaseObjectType.Direction);
			if (commandType == DirectionCommandType.TurnOff_InAutomatic)
				FiresecServiceManager.SafeFiresecService.GKTurnOffInAutomatic(clientUID, uid, GKBaseObjectType.Direction);
			if (commandType == DirectionCommandType.TurnOnNow_InAutomatic)
				FiresecServiceManager.SafeFiresecService.GKTurnOnNowInAutomatic(clientUID, uid, GKBaseObjectType.Direction);
			if (commandType == DirectionCommandType.TurnOffNow_InAutomatic)
				FiresecServiceManager.SafeFiresecService.GKTurnOffNowInAutomatic(clientUID, uid, GKBaseObjectType.Direction);
		}

		public static void ControlGKDoor(Guid clientUID, Guid uid, GKDoorCommandType commandType)
		{
			var door = GKManager.Doors.FirstOrDefault(x => x.UID == uid);
			if (door == null)
				return;
			if (commandType == GKDoorCommandType.Automatic)
				FiresecServiceManager.SafeFiresecService.GKSetAutomaticRegime(clientUID, uid, GKBaseObjectType.Door);
			if (commandType == GKDoorCommandType.Ignore)
				FiresecServiceManager.SafeFiresecService.GKSetIgnoreRegime(clientUID, uid, GKBaseObjectType.Door);
			if (commandType == GKDoorCommandType.Manual)
				FiresecServiceManager.SafeFiresecService.GKSetManualRegime(clientUID, uid, GKBaseObjectType.Door);
			if (commandType == GKDoorCommandType.Open)
				FiresecServiceManager.SafeFiresecService.GKTurnOn(clientUID, uid, GKBaseObjectType.Door);
			if (commandType == GKDoorCommandType.Close)
				FiresecServiceManager.SafeFiresecService.GKTurnOff(clientUID, uid, GKBaseObjectType.Door);
			if (commandType == GKDoorCommandType.OpenInAutomatic)
				FiresecServiceManager.SafeFiresecService.GKTurnOnInAutomatic(clientUID, uid, GKBaseObjectType.Door);
			if (commandType == GKDoorCommandType.CloseInAutomatic)
				FiresecServiceManager.SafeFiresecService.GKTurnOffInAutomatic(clientUID, uid, GKBaseObjectType.Door);
			if (commandType == GKDoorCommandType.OpenNow)
				FiresecServiceManager.SafeFiresecService.GKTurnOnNow(clientUID, uid, GKBaseObjectType.Door);
			if (commandType == GKDoorCommandType.CloseNow)
				FiresecServiceManager.SafeFiresecService.GKTurnOffNow(clientUID, uid, GKBaseObjectType.Door);
			if (commandType == GKDoorCommandType.OpenNowInAutomatic)
				FiresecServiceManager.SafeFiresecService.GKTurnOnNowInAutomatic(clientUID, uid, GKBaseObjectType.Door);
			if (commandType == GKDoorCommandType.CloseNowInAutomatic)
				FiresecServiceManager.SafeFiresecService.GKTurnOffNowInAutomatic(clientUID, uid, GKBaseObjectType.Door);
			if (commandType == GKDoorCommandType.OpenForever)
			{
				FiresecServiceManager.SafeFiresecService.GKSetManualRegime(clientUID, uid, GKBaseObjectType.Door);
				FiresecServiceManager.SafeFiresecService.GKTurnOn(clientUID, uid, GKBaseObjectType.Door);
			}
			if (commandType == GKDoorCommandType.CloseForever)
			{
				FiresecServiceManager.SafeFiresecService.GKSetManualRegime(clientUID, uid, GKBaseObjectType.Door);
				FiresecServiceManager.SafeFiresecService.GKTurnOff(clientUID, uid, GKBaseObjectType.Door);
			}
			if (commandType == GKDoorCommandType.Norm)
			{
				FiresecServiceManager.SafeFiresecService.GKTurnOff(clientUID, uid, GKBaseObjectType.Door);
				FiresecServiceManager.SafeFiresecService.GKSetAutomaticRegime(clientUID, uid, GKBaseObjectType.Door);
			}
		}

		public static void ControlDelay(Guid clientUID, Guid uid, DelayCommandType commandType)
		{
			if (commandType == DelayCommandType.Automatic)
				FiresecServiceManager.SafeFiresecService.GKSetAutomaticRegime(clientUID, uid, GKBaseObjectType.Delay);
			if (commandType == DelayCommandType.Ignore)
				FiresecServiceManager.SafeFiresecService.GKSetIgnoreRegime(clientUID, uid, GKBaseObjectType.Delay);
			if (commandType == DelayCommandType.Manual)
				FiresecServiceManager.SafeFiresecService.GKSetManualRegime(clientUID, uid, GKBaseObjectType.Delay);
			if (commandType == DelayCommandType.TurnOn)
				FiresecServiceManager.SafeFiresecService.GKTurnOn(clientUID, uid, GKBaseObjectType.Delay);
			if (commandType == DelayCommandType.TurnOnNow)
				FiresecServiceManager.SafeFiresecService.GKTurnOnNow(clientUID, uid, GKBaseObjectType.Delay);
			if (commandType == DelayCommandType.TurnOff)
				FiresecServiceManager.SafeFiresecService.GKTurnOff(clientUID, uid, GKBaseObjectType.Delay);
			if (commandType == DelayCommandType.TurnOffNow)
				FiresecServiceManager.SafeFiresecService.GKTurnOffNow(clientUID, uid, GKBaseObjectType.Delay);
			if (commandType == DelayCommandType.TurnOn_InAutomatic)
				FiresecServiceManager.SafeFiresecService.GKTurnOnInAutomatic(clientUID, uid, GKBaseObjectType.Delay);
			if (commandType == DelayCommandType.TurnOnNow_InAutomatic)
				FiresecServiceManager.SafeFiresecService.GKTurnOnNowInAutomatic(clientUID, uid, GKBaseObjectType.Delay);
			if (commandType == DelayCommandType.TurnOff_InAutomatic)
				FiresecServiceManager.SafeFiresecService.GKTurnOffInAutomatic(clientUID, uid, GKBaseObjectType.Delay);
			if (commandType == DelayCommandType.TurnOffNow_InAutomatic)
				FiresecServiceManager.SafeFiresecService.GKTurnOffNowInAutomatic(clientUID, uid, GKBaseObjectType.Delay);
		}

		public static void ControlPumpStation(Guid uid, PumpStationCommandType commandType)
		{
			if (commandType == PumpStationCommandType.Automatic)
				FiresecServiceManager.SafeFiresecService.GKSetAutomaticRegime(uid, GKBaseObjectType.PumpStation);
			if (commandType == PumpStationCommandType.Ignore)
				FiresecServiceManager.SafeFiresecService.GKSetIgnoreRegime(uid, GKBaseObjectType.PumpStation);
			if (commandType == PumpStationCommandType.Manual)
				FiresecServiceManager.SafeFiresecService.GKSetManualRegime(uid, GKBaseObjectType.PumpStation);
			if (commandType == PumpStationCommandType.TurnOff)
				FiresecServiceManager.SafeFiresecService.GKTurnOff(uid, GKBaseObjectType.PumpStation);
			if (commandType == PumpStationCommandType.TurnOn)
				FiresecServiceManager.SafeFiresecService.GKTurnOn(uid, GKBaseObjectType.PumpStation);
			if (commandType == PumpStationCommandType.Stop)
				FiresecServiceManager.SafeFiresecService.GKStop(uid, GKBaseObjectType.PumpStation);
			if (commandType == PumpStationCommandType.ForbidStart)
				FiresecServiceManager.SafeFiresecService.GKStop(uid, GKBaseObjectType.PumpStation);
		}

		public static void ControlMPT(Guid uid, MPTCommandType commandType)
		{
			if (commandType == MPTCommandType.Automatic)
				FiresecServiceManager.SafeFiresecService.GKSetAutomaticRegime(uid, GKBaseObjectType.MPT);
			if (commandType == MPTCommandType.Ignore)
				FiresecServiceManager.SafeFiresecService.GKSetIgnoreRegime(uid, GKBaseObjectType.MPT);
			if (commandType == MPTCommandType.Manual)
				FiresecServiceManager.SafeFiresecService.GKSetManualRegime(uid, GKBaseObjectType.MPT);
			if (commandType == MPTCommandType.TurnOff)
				FiresecServiceManager.SafeFiresecService.GKTurnOff(uid, GKBaseObjectType.MPT);
			if (commandType == MPTCommandType.TurnOn)
				FiresecServiceManager.SafeFiresecService.GKTurnOn(uid, GKBaseObjectType.MPT);
			if (commandType == MPTCommandType.Stop)
				FiresecServiceManager.SafeFiresecService.GKStop(uid, GKBaseObjectType.MPT);
			if (commandType == MPTCommandType.ForbidStart)
				FiresecServiceManager.SafeFiresecService.GKStop(uid, GKBaseObjectType.MPT);
		}

		public static void ControlPumpStation(Guid clientUID, Guid uid, PumpStationCommandType commandType)
		{
			if (commandType == PumpStationCommandType.Automatic)
				FiresecServiceManager.SafeFiresecService.GKSetAutomaticRegime(clientUID, uid, GKBaseObjectType.PumpStation);
			if (commandType == PumpStationCommandType.Ignore)
				FiresecServiceManager.SafeFiresecService.GKSetIgnoreRegime(clientUID, uid, GKBaseObjectType.PumpStation);
			if (commandType == PumpStationCommandType.Manual)
				FiresecServiceManager.SafeFiresecService.GKSetManualRegime(clientUID, uid, GKBaseObjectType.PumpStation);
			if (commandType == PumpStationCommandType.TurnOn)
				FiresecServiceManager.SafeFiresecService.GKTurnOn(clientUID, uid, GKBaseObjectType.PumpStation);
			if (commandType == PumpStationCommandType.TurnOff)
				FiresecServiceManager.SafeFiresecService.GKTurnOff(clientUID, uid, GKBaseObjectType.PumpStation);
			if (commandType == PumpStationCommandType.TurnOnNow)
				FiresecServiceManager.SafeFiresecService.GKTurnOnNow(clientUID, uid, GKBaseObjectType.PumpStation);
			if (commandType == PumpStationCommandType.TurnOffNow)
				FiresecServiceManager.SafeFiresecService.GKTurnOffNow(clientUID, uid, GKBaseObjectType.PumpStation);
			if (commandType == PumpStationCommandType.TurnOn_InAutomatic)
				FiresecServiceManager.SafeFiresecService.GKTurnOnInAutomatic(clientUID, uid, GKBaseObjectType.PumpStation);
			if (commandType == PumpStationCommandType.TurnOff_InAutomatic)
				FiresecServiceManager.SafeFiresecService.GKTurnOffInAutomatic(clientUID, uid, GKBaseObjectType.PumpStation);
			if (commandType == PumpStationCommandType.TurnOnNow_InAutomatic)
				FiresecServiceManager.SafeFiresecService.GKTurnOnNowInAutomatic(clientUID, uid, GKBaseObjectType.PumpStation);
			if (commandType == PumpStationCommandType.TurnOffNow_InAutomatic)
				FiresecServiceManager.SafeFiresecService.GKTurnOffNowInAutomatic(clientUID, uid, GKBaseObjectType.PumpStation);
			if (commandType == PumpStationCommandType.Stop)
				FiresecServiceManager.SafeFiresecService.GKStop(clientUID, uid, GKBaseObjectType.PumpStation);
			if (commandType == PumpStationCommandType.ForbidStart)
				FiresecServiceManager.SafeFiresecService.GKStop(clientUID, uid, GKBaseObjectType.PumpStation);
		}

		public static void ControlMPT(Guid clientUID, Guid uid, MPTCommandType commandType)
		{
			if (commandType == MPTCommandType.Automatic)
				FiresecServiceManager.SafeFiresecService.GKSetAutomaticRegime(clientUID, uid, GKBaseObjectType.MPT);
			if (commandType == MPTCommandType.Ignore)
				FiresecServiceManager.SafeFiresecService.GKSetIgnoreRegime(clientUID, uid, GKBaseObjectType.MPT);
			if (commandType == MPTCommandType.Manual)
				FiresecServiceManager.SafeFiresecService.GKSetManualRegime(clientUID, uid, GKBaseObjectType.MPT);
			if (commandType == MPTCommandType.TurnOn)
				FiresecServiceManager.SafeFiresecService.GKTurnOn(clientUID, uid, GKBaseObjectType.MPT);
			if (commandType == MPTCommandType.TurnOff)
				FiresecServiceManager.SafeFiresecService.GKTurnOff(clientUID, uid, GKBaseObjectType.MPT);
			if (commandType == MPTCommandType.TurnOnNow)
				FiresecServiceManager.SafeFiresecService.GKTurnOnNow(clientUID, uid, GKBaseObjectType.MPT);
			if (commandType == MPTCommandType.TurnOffNow)
				FiresecServiceManager.SafeFiresecService.GKTurnOffNow(clientUID, uid, GKBaseObjectType.MPT);
			if (commandType == MPTCommandType.TurnOn_InAutomatic)
				FiresecServiceManager.SafeFiresecService.GKTurnOnInAutomatic(clientUID, uid, GKBaseObjectType.MPT);
			if (commandType == MPTCommandType.TurnOff_InAutomatic)
				FiresecServiceManager.SafeFiresecService.GKTurnOffInAutomatic(clientUID, uid, GKBaseObjectType.MPT);
			if (commandType == MPTCommandType.TurnOnNow_InAutomatic)
				FiresecServiceManager.SafeFiresecService.GKTurnOnNowInAutomatic(clientUID, uid, GKBaseObjectType.MPT);
			if (commandType == MPTCommandType.TurnOffNow_InAutomatic)
				FiresecServiceManager.SafeFiresecService.GKTurnOffNowInAutomatic(clientUID, uid, GKBaseObjectType.MPT);
			if (commandType == MPTCommandType.Stop)
				FiresecServiceManager.SafeFiresecService.GKStop(clientUID, uid, GKBaseObjectType.MPT);
			if (commandType == MPTCommandType.ForbidStart)
				FiresecServiceManager.SafeFiresecService.GKStop(clientUID, uid, GKBaseObjectType.MPT);
		}

		public static void ExportJournal(Guid clientUID, bool isExportJournal, bool isExportPassJournal, DateTime minDate, DateTime maxDate, string path)
		{
			var result = FiresecServiceManager.SafeFiresecService.ExportJournal(clientUID,
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

		public static void ExportOrganisation(Guid clientUID, bool isWithDeleted, Guid organisationUid, string path)
		{
			var result = FiresecServiceManager.SafeFiresecService.ExportOrganisation(clientUID,
				new ExportFilter
				{
					IsWithDeleted = isWithDeleted,
					OrganisationUID = organisationUid,
					Path = path
				});
			if (result.HasError)
				BalloonHelper.ShowFromServer(result.Error);
		}
		public static void ExportOrganisationList(Guid clientUID, bool isWithDeleted, string path)
		{
			var result = FiresecServiceManager.SafeFiresecService.ExportOrganisationList(clientUID,
				new ExportFilter
				{
					IsWithDeleted = isWithDeleted,
					Path = path
				});
			if (result.HasError)
				BalloonHelper.ShowFromServer(result.Error);
		}

		public static void ExportConfiguration(Guid clientUID, bool isExportDevices, bool isExportDoors, bool isExportZones, string path)
		{
			var result = FiresecServiceManager.SafeFiresecService.ExportConfiguration(clientUID,
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
		public static void ImportOrganisation(Guid clientUID, bool isWithDeleted, string path)
		{
			var result = FiresecServiceManager.SafeFiresecService.ImportOrganisation(clientUID,
				new ImportFilter
				{
					IsWithDeleted = isWithDeleted,
					Path = path
				});
			if (result.HasError)
				BalloonHelper.ShowFromServer(result.Error);
		}
		public static void ImportOrganisationList(Guid clientUID, bool isWithDeleted, string path)
		{
			var result = FiresecServiceManager.SafeFiresecService.ImportOrganisationList(clientUID,
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
