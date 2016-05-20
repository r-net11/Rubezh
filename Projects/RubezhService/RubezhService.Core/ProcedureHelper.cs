using RubezhService.Service;
using Infrastructure.Automation;
using RubezhAPI;
using RubezhAPI.Automation;
using RubezhAPI.GK;
using RubezhAPI.Journal;
using RubezhAPI.SKD;
using RubezhDAL.DataClasses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RubezhService
{
	public static class ProcedureHelper
	{
		public static void AddJournalItem(Guid clientUID, string message, Guid? objectUID = null)
		{
			var journalItem = new JournalItem()
			{
				SystemDateTime = DateTime.Now,
				JournalEventNameType = JournalEventNameType.Сообщение_автоматизации,
				DescriptionText = message
			};
			if (objectUID.HasValue)
			{
				string objectName;
				JournalObjectType journalObjectType;
				if (GetObjectInfo(objectUID.Value, out objectName, out journalObjectType))
				{
					journalItem.ObjectUID = objectUID.Value;
					journalItem.ObjectName = objectName;
					journalItem.JournalObjectType = journalObjectType;
				}
			}
			Service.RubezhService.AddCommonJournalItems(new List<JournalItem>() { journalItem }, clientUID);
		}

		static bool GetObjectInfo(Guid objectUID, out string objectName, out JournalObjectType journalObjectType)
		{
			var device = GKManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == objectUID);
			if (device != null)
			{
				objectName = device.Name;
				journalObjectType = JournalObjectType.GKDevice;
				return true;
			}

			var zone = GKManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.UID == objectUID);
			if (zone != null)
			{
				objectName = zone.Name;
				journalObjectType = JournalObjectType.GKZone;
				return true;
			}

			var guardZone = GKManager.DeviceConfiguration.GuardZones.FirstOrDefault(x => x.UID == objectUID);
			if (guardZone != null)
			{
				objectName = guardZone.Name;
				journalObjectType = JournalObjectType.GKGuardZone;
				return true;
			}

			var camera = ConfigurationCashHelper.SystemConfiguration.Cameras.FirstOrDefault(x => x.UID == objectUID);
			if (camera != null)
			{
				objectName = camera.Name;
				journalObjectType = JournalObjectType.Camera;
				return true;
			}

			var direction = GKManager.DeviceConfiguration.Directions.FirstOrDefault(x => x.UID == objectUID);
			if (direction != null)
			{
				objectName = direction.Name;
				journalObjectType = JournalObjectType.GKDirection;
				return true;
			}

			var delay = GKManager.DeviceConfiguration.Delays.FirstOrDefault(x => x.UID == objectUID);
			if (delay != null)
			{
				objectName = delay.Name;
				journalObjectType = JournalObjectType.GKDelay;
				return true;
			}

			var pumpStation = GKManager.DeviceConfiguration.PumpStations.FirstOrDefault(x => x.UID == objectUID);
			if (pumpStation != null)
			{
				objectName = pumpStation.Name;
				journalObjectType = JournalObjectType.GKPumpStation;
				return true;
			}

			var mpt = GKManager.DeviceConfiguration.MPTs.FirstOrDefault(x => x.UID == objectUID);
			if (mpt != null)
			{
				objectName = mpt.Name;
				journalObjectType = JournalObjectType.GKMPT;
				return true;
			}

			var door = GKManager.Doors.FirstOrDefault(x => x.UID == objectUID);
			if (door != null)
			{
				objectName = door.Name;
				journalObjectType = JournalObjectType.GKDoor;
				return true;
			}

			var organisations = ProcedureExecutionContext.GetOrganisations(Guid.Empty);
			var organisation = organisations == null ? null : organisations.FirstOrDefault(x => x.UID == objectUID);
			if (organisation != null)
			{
				objectName = organisation.Name;
				journalObjectType = JournalObjectType.None;
				return true;
			}

			objectName = "";
			journalObjectType = JournalObjectType.None;
			return false;

		}

		public static void ControlGKDevice(Guid clientUID, Guid deviceUid, GKStateBit command)
		{
			RubezhServiceManager.SafeRubezhService.GKExecuteDeviceCommand(clientUID, deviceUid, command);
		}

		public static void StartRecord(Guid clientUID, Guid cameraUid, Guid? journalItemUid, Guid? eventUid, int timeout)
		{
			var camera = ConfigurationCashHelper.SystemConfiguration.Cameras.FirstOrDefault(x => x.UID == cameraUid);
			if (camera == null)
				return;
			if (journalItemUid.HasValue)
				using (var dbService = new DbService())
				{
					var operationResult = dbService.JournalTranslator.GetFilteredJournalItems(new JournalFilter() { ItemUID = journalItemUid.Value });
					if (!operationResult.HasError)
					{
						var journalItem = operationResult.Result.FirstOrDefault();
						if (journalItem != null)
						{
							dbService.JournalTranslator.SaveVideoUID(journalItemUid.Value, eventUid.Value, cameraUid);
							journalItem.VideoUID = eventUid.Value;
							journalItem.CameraUID = cameraUid;
							RubezhService.Service.RubezhService.NotifyJournalItems(new List<JournalItem>() { journalItem }, false);
						}
					}
				}
			RviClient.RviClientHelper.VideoRecordStart(ConfigurationCashHelper.SystemConfiguration.RviSettings, camera, eventUid.Value, timeout);
		}

		public static void StopRecord(Guid clientUID, Guid cameraUid, Guid eventUid)
		{
			var camera = ConfigurationCashHelper.SystemConfiguration.Cameras.FirstOrDefault(x => x.UID == cameraUid);
			if (camera == null)
				return;
			RviClient.RviClientHelper.VideoRecordStop(ConfigurationCashHelper.SystemConfiguration.RviSettings, camera, eventUid);
		}

		public static void Ptz(Guid clientUID, Guid cameraUid, int ptzNumber)
		{
			var camera = ConfigurationCashHelper.SystemConfiguration.Cameras.FirstOrDefault(x => x.UID == cameraUid);
			if (camera == null)
				return;
			RviClient.RviClientHelper.SetPtzPreset(ConfigurationCashHelper.SystemConfiguration.RviSettings, camera, ptzNumber);
		}

		public static void RviAlarm(Guid clientUID, string name)
		{
			RviClient.RviClientHelper.AlarmRuleExecute(ConfigurationCashHelper.SystemConfiguration.RviSettings, name);
		}

		public static void RviOpenWindow(Guid clientUid, string name, int x, int y, int monitorNumber, string login, string ip)
		{
			RviClient.RviClientHelper.OpenWindow(ConfigurationCashHelper.SystemConfiguration.RviSettings, name, x, y, monitorNumber, login, ip);
		}

		public static void ControlFireZone(Guid clientUID, Guid uid, ZoneCommandType commandType)
		{
			if (commandType == ZoneCommandType.Ignore)
				RubezhServiceManager.SafeRubezhService.GKSetIgnoreRegime(clientUID, uid, GKBaseObjectType.Zone);
			if (commandType == ZoneCommandType.ResetIgnore)
				RubezhServiceManager.SafeRubezhService.GKSetAutomaticRegime(clientUID, uid, GKBaseObjectType.Zone);
			if (commandType == ZoneCommandType.Reset)
				RubezhServiceManager.SafeRubezhService.GKReset(clientUID, uid, GKBaseObjectType.Zone);
		}

		public static void ControlGuardZone(Guid clientUID, Guid uid, GuardZoneCommandType commandType)
		{
			if (commandType == GuardZoneCommandType.Automatic)
				RubezhServiceManager.SafeRubezhService.GKSetAutomaticRegime(clientUID, uid, GKBaseObjectType.GuardZone);
			if (commandType == GuardZoneCommandType.Ignore)
				RubezhServiceManager.SafeRubezhService.GKSetIgnoreRegime(clientUID, uid, GKBaseObjectType.GuardZone);
			if (commandType == GuardZoneCommandType.Manual)
				RubezhServiceManager.SafeRubezhService.GKSetManualRegime(clientUID, uid, GKBaseObjectType.GuardZone);
			if (commandType == GuardZoneCommandType.Reset)
				RubezhServiceManager.SafeRubezhService.GKReset(clientUID, uid, GKBaseObjectType.GuardZone);
			if (commandType == GuardZoneCommandType.TurnOn)
				RubezhServiceManager.SafeRubezhService.GKTurnOn(clientUID, uid, GKBaseObjectType.GuardZone);
			if (commandType == GuardZoneCommandType.TurnOnNow)
				RubezhServiceManager.SafeRubezhService.GKTurnOnNow(clientUID, uid, GKBaseObjectType.GuardZone);
			if (commandType == GuardZoneCommandType.TurnOff)
				RubezhServiceManager.SafeRubezhService.GKTurnOff(clientUID, uid, GKBaseObjectType.GuardZone);
			if (commandType == GuardZoneCommandType.TurnOffNow)
				RubezhServiceManager.SafeRubezhService.GKTurnOffNow(clientUID, uid, GKBaseObjectType.GuardZone);
			if (commandType == GuardZoneCommandType.TurnOnInAutomatic)
				RubezhServiceManager.SafeRubezhService.GKTurnOnInAutomatic(clientUID, uid, GKBaseObjectType.GuardZone);
			if (commandType == GuardZoneCommandType.TurnOnNowInAutomatic)
				RubezhServiceManager.SafeRubezhService.GKTurnOnNowInAutomatic(clientUID, uid, GKBaseObjectType.GuardZone);
			if (commandType == GuardZoneCommandType.TurnOffInAutomatic)
				RubezhServiceManager.SafeRubezhService.GKTurnOffInAutomatic(clientUID, uid, GKBaseObjectType.GuardZone);
			if (commandType == GuardZoneCommandType.TurnOffNowInAutomatic)
				RubezhServiceManager.SafeRubezhService.GKTurnOffNowInAutomatic(clientUID, uid, GKBaseObjectType.GuardZone);
		}

		public static void ControlDirection(Guid clientUID, Guid uid, DirectionCommandType commandType)
		{
			if (commandType == DirectionCommandType.Automatic)
				RubezhServiceManager.SafeRubezhService.GKSetAutomaticRegime(clientUID, uid, GKBaseObjectType.Direction);
			if (commandType == DirectionCommandType.ForbidStart)
				RubezhServiceManager.SafeRubezhService.GKStop(clientUID, uid, GKBaseObjectType.Direction);
			if (commandType == DirectionCommandType.Ignore)
				RubezhServiceManager.SafeRubezhService.GKSetIgnoreRegime(clientUID, uid, GKBaseObjectType.Direction);
			if (commandType == DirectionCommandType.Manual)
				RubezhServiceManager.SafeRubezhService.GKSetManualRegime(clientUID, uid, GKBaseObjectType.Direction);
			if (commandType == DirectionCommandType.TurnOn)
				RubezhServiceManager.SafeRubezhService.GKTurnOn(clientUID, uid, GKBaseObjectType.Direction);
			if (commandType == DirectionCommandType.TurnOff)
				RubezhServiceManager.SafeRubezhService.GKTurnOff(clientUID, uid, GKBaseObjectType.Direction);
			if (commandType == DirectionCommandType.TurnOnNow)
				RubezhServiceManager.SafeRubezhService.GKTurnOnNow(clientUID, uid, GKBaseObjectType.Direction);
			if (commandType == DirectionCommandType.TurnOn_InAutomatic)
				RubezhServiceManager.SafeRubezhService.GKTurnOnInAutomatic(clientUID, uid, GKBaseObjectType.Direction);
			if (commandType == DirectionCommandType.TurnOff_InAutomatic)
				RubezhServiceManager.SafeRubezhService.GKTurnOffInAutomatic(clientUID, uid, GKBaseObjectType.Direction);
			if (commandType == DirectionCommandType.TurnOnNow_InAutomatic)
				RubezhServiceManager.SafeRubezhService.GKTurnOnNowInAutomatic(clientUID, uid, GKBaseObjectType.Direction);
		}

		public static void ControlGKDoor(Guid clientUID, Guid uid, GKDoorCommandType commandType)
		{
			var door = GKManager.Doors.FirstOrDefault(x => x.UID == uid);
			if (door == null)
				return;
			if (commandType == GKDoorCommandType.Automatic)
				RubezhServiceManager.SafeRubezhService.GKSetAutomaticRegime(clientUID, uid, GKBaseObjectType.Door);
			if (commandType == GKDoorCommandType.Ignore)
				RubezhServiceManager.SafeRubezhService.GKSetIgnoreRegime(clientUID, uid, GKBaseObjectType.Door);
			if (commandType == GKDoorCommandType.Manual)
				RubezhServiceManager.SafeRubezhService.GKSetManualRegime(clientUID, uid, GKBaseObjectType.Door);
			if (commandType == GKDoorCommandType.Open)
				RubezhServiceManager.SafeRubezhService.GKTurnOn(clientUID, uid, GKBaseObjectType.Door);
			if (commandType == GKDoorCommandType.Close)
				RubezhServiceManager.SafeRubezhService.GKTurnOff(clientUID, uid, GKBaseObjectType.Door);
			if (commandType == GKDoorCommandType.OpenInAutomatic)
				RubezhServiceManager.SafeRubezhService.GKTurnOnInAutomatic(clientUID, uid, GKBaseObjectType.Door);
			if (commandType == GKDoorCommandType.CloseInAutomatic)
				RubezhServiceManager.SafeRubezhService.GKTurnOffInAutomatic(clientUID, uid, GKBaseObjectType.Door);
			if (commandType == GKDoorCommandType.CloseNow)
				RubezhServiceManager.SafeRubezhService.GKTurnOffNow(clientUID, uid, GKBaseObjectType.Door);
			if (commandType == GKDoorCommandType.CloseNowInAutomatic)
				RubezhServiceManager.SafeRubezhService.GKTurnOffNowInAutomatic(clientUID, uid, GKBaseObjectType.Door);
			if (commandType == GKDoorCommandType.OpenForever)
			{
				RubezhServiceManager.SafeRubezhService.GKSetManualRegime(clientUID, uid, GKBaseObjectType.Door);
				RubezhServiceManager.SafeRubezhService.GKTurnOn(clientUID, uid, GKBaseObjectType.Door);
			}
			if (commandType == GKDoorCommandType.CloseForever)
			{
				RubezhServiceManager.SafeRubezhService.GKSetManualRegime(clientUID, uid, GKBaseObjectType.Door);
				RubezhServiceManager.SafeRubezhService.GKTurnOffNow(clientUID, uid, GKBaseObjectType.Door);
			}
			if (commandType == GKDoorCommandType.Norm)
			{
				RubezhServiceManager.SafeRubezhService.GKTurnOffNow(clientUID, uid, GKBaseObjectType.Door);
				RubezhServiceManager.SafeRubezhService.GKSetAutomaticRegime(clientUID, uid, GKBaseObjectType.Door);
			}
		}

		public static void ControlDelay(Guid clientUID, Guid uid, DelayCommandType commandType)
		{
			if (commandType == DelayCommandType.Automatic)
				RubezhServiceManager.SafeRubezhService.GKSetAutomaticRegime(clientUID, uid, GKBaseObjectType.Delay);
			if (commandType == DelayCommandType.Ignore)
				RubezhServiceManager.SafeRubezhService.GKSetIgnoreRegime(clientUID, uid, GKBaseObjectType.Delay);
			if (commandType == DelayCommandType.Manual)
				RubezhServiceManager.SafeRubezhService.GKSetManualRegime(clientUID, uid, GKBaseObjectType.Delay);
			if (commandType == DelayCommandType.TurnOn)
				RubezhServiceManager.SafeRubezhService.GKTurnOn(clientUID, uid, GKBaseObjectType.Delay);
			if (commandType == DelayCommandType.TurnOnNow)
				RubezhServiceManager.SafeRubezhService.GKTurnOnNow(clientUID, uid, GKBaseObjectType.Delay);
			if (commandType == DelayCommandType.TurnOff)
				RubezhServiceManager.SafeRubezhService.GKTurnOff(clientUID, uid, GKBaseObjectType.Delay);
			if (commandType == DelayCommandType.TurnOn_InAutomatic)
				RubezhServiceManager.SafeRubezhService.GKTurnOnInAutomatic(clientUID, uid, GKBaseObjectType.Delay);
			if (commandType == DelayCommandType.TurnOnNow_InAutomatic)
				RubezhServiceManager.SafeRubezhService.GKTurnOnNowInAutomatic(clientUID, uid, GKBaseObjectType.Delay);
			if (commandType == DelayCommandType.TurnOff_InAutomatic)
				RubezhServiceManager.SafeRubezhService.GKTurnOffInAutomatic(clientUID, uid, GKBaseObjectType.Delay);
		}

		public static void ControlPumpStation(Guid clientUID, Guid uid, PumpStationCommandType commandType)
		{
			if (commandType == PumpStationCommandType.Automatic)
				RubezhServiceManager.SafeRubezhService.GKSetAutomaticRegime(clientUID, uid, GKBaseObjectType.PumpStation);
			if (commandType == PumpStationCommandType.Ignore)
				RubezhServiceManager.SafeRubezhService.GKSetIgnoreRegime(clientUID, uid, GKBaseObjectType.PumpStation);
			if (commandType == PumpStationCommandType.Manual)
				RubezhServiceManager.SafeRubezhService.GKSetManualRegime(clientUID, uid, GKBaseObjectType.PumpStation);
			if (commandType == PumpStationCommandType.TurnOn)
				RubezhServiceManager.SafeRubezhService.GKTurnOn(clientUID, uid, GKBaseObjectType.PumpStation);
			if (commandType == PumpStationCommandType.TurnOff)
				RubezhServiceManager.SafeRubezhService.GKTurnOff(clientUID, uid, GKBaseObjectType.PumpStation);
			if (commandType == PumpStationCommandType.TurnOnNow)
				RubezhServiceManager.SafeRubezhService.GKTurnOnNow(clientUID, uid, GKBaseObjectType.PumpStation);
			if (commandType == PumpStationCommandType.TurnOn_InAutomatic)
				RubezhServiceManager.SafeRubezhService.GKTurnOnInAutomatic(clientUID, uid, GKBaseObjectType.PumpStation);
			if (commandType == PumpStationCommandType.TurnOff_InAutomatic)
				RubezhServiceManager.SafeRubezhService.GKTurnOffInAutomatic(clientUID, uid, GKBaseObjectType.PumpStation);
			if (commandType == PumpStationCommandType.TurnOnNow_InAutomatic)
				RubezhServiceManager.SafeRubezhService.GKTurnOnNowInAutomatic(clientUID, uid, GKBaseObjectType.PumpStation);
			if (commandType == PumpStationCommandType.ForbidStart)
				RubezhServiceManager.SafeRubezhService.GKStop(clientUID, uid, GKBaseObjectType.PumpStation);
		}

		public static void ControlMPT(Guid clientUID, Guid uid, MPTCommandType commandType)
		{
			if (commandType == MPTCommandType.Automatic)
				RubezhServiceManager.SafeRubezhService.GKSetAutomaticRegime(clientUID, uid, GKBaseObjectType.MPT);
			if (commandType == MPTCommandType.Ignore)
				RubezhServiceManager.SafeRubezhService.GKSetIgnoreRegime(clientUID, uid, GKBaseObjectType.MPT);
			if (commandType == MPTCommandType.Manual)
				RubezhServiceManager.SafeRubezhService.GKSetManualRegime(clientUID, uid, GKBaseObjectType.MPT);
			if (commandType == MPTCommandType.TurnOn)
				RubezhServiceManager.SafeRubezhService.GKTurnOn(clientUID, uid, GKBaseObjectType.MPT);
			if (commandType == MPTCommandType.TurnOff)
				RubezhServiceManager.SafeRubezhService.GKTurnOff(clientUID, uid, GKBaseObjectType.MPT);
			if (commandType == MPTCommandType.TurnOnNow)
				RubezhServiceManager.SafeRubezhService.GKTurnOnNow(clientUID, uid, GKBaseObjectType.MPT);
			if (commandType == MPTCommandType.TurnOn_InAutomatic)
				RubezhServiceManager.SafeRubezhService.GKTurnOnInAutomatic(clientUID, uid, GKBaseObjectType.MPT);
			if (commandType == MPTCommandType.TurnOff_InAutomatic)
				RubezhServiceManager.SafeRubezhService.GKTurnOffInAutomatic(clientUID, uid, GKBaseObjectType.MPT);
			if (commandType == MPTCommandType.TurnOnNow_InAutomatic)
				RubezhServiceManager.SafeRubezhService.GKTurnOnNowInAutomatic(clientUID, uid, GKBaseObjectType.MPT);
			if (commandType == MPTCommandType.ForbidStart)
				RubezhServiceManager.SafeRubezhService.GKStop(clientUID, uid, GKBaseObjectType.MPT);
		}

		public static void ExportJournal(Guid clientUID, bool isExportJournal, bool isExportPassJournal, DateTime minDate, DateTime maxDate, string path)
		{
			var result = RubezhServiceManager.SafeRubezhService.ExportJournal(clientUID,
				new JournalExportFilter
				{
					IsExportJournal = isExportJournal,
					IsExportPassJournal = isExportPassJournal,
					MaxDate = maxDate,
					MinDate = minDate,
					Path = path
				});
			if (result.HasError)
				Notifier.BalloonShowFromServer("Экспорт журнала " + result.Error);
		}

		public static void ExportOrganisation(Guid clientUID, bool isWithDeleted, Guid organisationUid, string path)
		{
			var result = RubezhServiceManager.SafeRubezhService.ExportOrganisation(clientUID,
				new ExportFilter
				{
					IsWithDeleted = isWithDeleted,
					OrganisationUID = organisationUid,
					Path = path
				});
			if (result.HasError)
				Notifier.BalloonShowFromServer(result.Error);
		}
		public static void ExportOrganisationList(Guid clientUID, bool isWithDeleted, string path)
		{
			var result = RubezhServiceManager.SafeRubezhService.ExportOrganisationList(clientUID,
				new ExportFilter
				{
					IsWithDeleted = isWithDeleted,
					Path = path
				});
			if (result.HasError)
				Notifier.BalloonShowFromServer(result.Error);
		}

		public static void ExportConfiguration(Guid clientUID, bool isExportDevices, bool isExportDoors, bool isExportZones, string path)
		{
			var result = RubezhServiceManager.SafeRubezhService.ExportConfiguration(clientUID,
				new ConfigurationExportFilter
				{
					IsExportDevices = isExportDevices,
					IsExportDoors = isExportDoors,
					IsExportZones = isExportZones,
					Path = path
				});
			if (result.HasError)
				Notifier.BalloonShowFromServer(result.Error);
		}
		public static void ImportOrganisation(Guid clientUID, bool isWithDeleted, string path)
		{
			var result = RubezhServiceManager.SafeRubezhService.ImportOrganisation(clientUID,
				new ImportFilter
				{
					IsWithDeleted = isWithDeleted,
					Path = path
				});
			if (result.HasError)
				Notifier.BalloonShowFromServer(result.Error);
		}
		public static void ImportOrganisationList(Guid clientUID, bool isWithDeleted, string path)
		{
			var result = RubezhServiceManager.SafeRubezhService.ImportOrganisationList(clientUID,
				new ImportFilter
				{
					IsWithDeleted = isWithDeleted,
					Path = path
				});
			if (result.HasError)
				Notifier.BalloonShowFromServer(result.Error);
		}
	}
}
