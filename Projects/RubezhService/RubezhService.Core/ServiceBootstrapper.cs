using RubezhService.Service;
using Infrastructure.Automation;
using RubezhAPI;
using RubezhAPI.Automation;
using RubezhAPI.AutomationCallback;
using RubezhAPI.SKD;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RubezhService
{
	public static class ServiceBootstrapper
	{
		public static void Run()
		{
			ProcedureExecutionContext.Initialize(
					ContextType.Server,
					() => { return ConfigurationCashHelper.SystemConfiguration; },
					Service.RubezhService.NotifyAutomation,
					null,
					null,
					ProcedureHelper.AddJournalItem,
					ProcedureHelper.ControlGKDevice,
					ProcedureHelper.StartRecord,
					ProcedureHelper.StopRecord,
					ProcedureHelper.Ptz,
					ProcedureHelper.RviAlarm,
					ProcedureHelper.RviOpenWindow,
					ProcedureHelper.ControlFireZone,
					ProcedureHelper.ControlGuardZone,
					ProcedureHelper.ControlDirection,
					ProcedureHelper.ControlGKDoor,
					ProcedureHelper.ControlDelay,
					ProcedureHelper.ControlPumpStation,
					ProcedureHelper.ControlMPT,
					ProcedureHelper.ExportJournal,
					ProcedureHelper.ExportOrganisation,
					ProcedureHelper.ExportOrganisationList,
					ProcedureHelper.ExportConfiguration,
					ProcedureHelper.ImportOrganisation,
					ProcedureHelper.ImportOrganisationList,
					GetOrganisations
					);

			ExplicitValue.ResolveObjectName += ObjectReference_ResolveObjectName;
			ExplicitValue.ResolveObjectValue += ExplicitValue_ResolveObjectValue;

			RubezhService.Service.RubezhService.AfterConnect += RubezhService_AfterConnect;
		}

		static List<Organisation> GetOrganisations(Guid clientUID)
		{
			var result = RubezhServiceManager.SafeRubezhService.GetOrganisations(clientUID, new OrganisationFilter());
			return result.HasError ? new List<Organisation>() : result.Result;
		}

		static object ExplicitValue_ResolveObjectValue(Guid objectUID, ObjectType objectType)
		{
			if (objectUID == Guid.Empty)
				return null;
			switch (objectType)
			{
				case ObjectType.Device: return GKManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == objectUID);
				case ObjectType.Zone: return GKManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.UID == objectUID);
				case ObjectType.Direction: return GKManager.DeviceConfiguration.Directions.FirstOrDefault(x => x.UID == objectUID);
				case ObjectType.Delay: return GKManager.DeviceConfiguration.Delays.FirstOrDefault(x => x.UID == objectUID);
				case ObjectType.GuardZone: return GKManager.DeviceConfiguration.GuardZones.FirstOrDefault(x => x.UID == objectUID);
				case ObjectType.PumpStation: return GKManager.DeviceConfiguration.PumpStations.FirstOrDefault(x => x.UID == objectUID);
				case ObjectType.MPT: return GKManager.DeviceConfiguration.MPTs.FirstOrDefault(x => x.UID == objectUID);
				case ObjectType.VideoDevice: return ProcedureExecutionContext.SystemConfiguration.Cameras.FirstOrDefault(x => x.UID == objectUID);
				case ObjectType.GKDoor: return GKManager.Doors.FirstOrDefault(x => x.UID == objectUID);
				case ObjectType.Organisation: return GetOrganisation(objectUID);
			}
			return null;
		}

		static string ObjectReference_ResolveObjectName(Guid objectUID, ObjectType objectType)
		{
			if (objectUID == Guid.Empty)
				return "Null";
			switch (objectType)
			{
				case ObjectType.Device:
					var device = GKManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == objectUID);
					if (device != null)
						return device.PresentationName;
					break;
				case ObjectType.Zone:
					var zone = GKManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.UID == objectUID);
					if (zone != null)
						return zone.PresentationName;
					break;
				case ObjectType.Direction:
					var direction = GKManager.DeviceConfiguration.Directions.FirstOrDefault(x => x.UID == objectUID);
					if (direction != null)
						return direction.PresentationName;
					break;
				case ObjectType.Delay:
					var delay = GKManager.DeviceConfiguration.Delays.FirstOrDefault(x => x.UID == objectUID);
					if (delay != null)
						return delay.PresentationName;
					break;
				case ObjectType.GuardZone:
					var guardZone = GKManager.DeviceConfiguration.GuardZones.FirstOrDefault(x => x.UID == objectUID);
					if (guardZone != null)
						return guardZone.PresentationName;
					break;
				case ObjectType.PumpStation:
					var pumpStation = GKManager.DeviceConfiguration.PumpStations.FirstOrDefault(x => x.UID == objectUID);
					if (pumpStation != null)
						return pumpStation.PresentationName;
					break;
				case ObjectType.MPT:
					var mpt = GKManager.DeviceConfiguration.MPTs.FirstOrDefault(x => x.UID == objectUID);
					if (mpt != null)
						return mpt.PresentationName;
					break;
				case ObjectType.VideoDevice:
					var camera = ProcedureExecutionContext.SystemConfiguration.Cameras.FirstOrDefault(x => x.UID == objectUID);
					if (camera != null)
						return camera.PresentationName;
					break;
				case ObjectType.GKDoor:
					var gKDoor = GKManager.Doors.FirstOrDefault(x => x.UID == objectUID);
					if (gKDoor != null)
						return gKDoor.PresentationName;
					break;
				case ObjectType.Organisation:
					var organisation = GetOrganisation(objectUID);
					if (organisation != null)
						return organisation.Name;
					break;
			}
			return "Null";
		}

		static RubezhAPI.SKD.Organisation GetOrganisation(Guid uid)
		{
			var organisations = RubezhServiceManager.SafeRubezhService.RubezhService.GetOrganisations(Guid.Empty, new RubezhAPI.SKD.OrganisationFilter());
			return organisations.HasError ? null : organisations.Result.FirstOrDefault(x => x.UID == uid);
		}

		static void RubezhService_AfterConnect(Guid clientUID)
		{
			foreach (var tag in ConfigurationCashHelper.SystemConfiguration.AutomationConfiguration.OpcDaTsServers.SelectMany(x => x.Tags))
				RubezhService.Service.RubezhService.NotifyAutomation(new AutomationCallbackResult
				{
					CallbackUID = Guid.NewGuid(),
					ContextType = ContextType.Server,
					AutomationCallbackType = AutomationCallbackType.OpcDaTag,
					Data = new OpcDaTagCallBackData
					{
						TagUID = tag.Uid,
						Value = OpcDaHelper.GetTagValue(tag.Uid)
					}
				}, clientUID);

			foreach (var variable in ConfigurationCashHelper.SystemConfiguration.AutomationConfiguration.GlobalVariables)
				RubezhService.Service.RubezhService.NotifyAutomation(new AutomationCallbackResult
				{
					CallbackUID = Guid.NewGuid(),
					ContextType = ContextType.Server,
					AutomationCallbackType = AutomationCallbackType.GlobalVariable,
					Data = new GlobalVariableCallBackData
					{
						VariableUID = variable.Uid,
						ExplicitValue = (ExplicitValue)variable
					}
				}, clientUID);
		}
	}
}
