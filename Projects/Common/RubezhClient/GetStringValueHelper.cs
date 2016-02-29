using RubezhAPI;
using RubezhAPI.Automation;
using RubezhClient.SKDHelpers;
using System;
using System.Linq;

namespace RubezhClient
{
	public static class GetStringValueHelper
	{
		public static string GetStringValue(ExplicitValue explicitValue, ExplicitType explicitType, EnumType enumType)
		{
			switch (explicitType)
			{
				case ExplicitType.Boolean:
					return explicitValue.BoolValue ? "Да" : "Нет";
				case ExplicitType.DateTime:
					return explicitValue.DateTimeValue.ToString();
				case ExplicitType.Integer:
					return explicitValue.IntValue.ToString();
				case ExplicitType.Float:
					return explicitValue.FloatValue.ToString();
				case ExplicitType.String:
					return explicitValue.StringValue;
				case ExplicitType.Enum:
					{
						switch (enumType)
						{
							case EnumType.StateType:
								return explicitValue.StateTypeValue.ToDescription();
							case EnumType.DriverType:
								return explicitValue.DriverTypeValue.ToDescription();
							case EnumType.PermissionType:
								return explicitValue.PermissionTypeValue.ToDescription();
							case EnumType.JournalEventDescriptionType:
								return explicitValue.JournalEventDescriptionTypeValue.ToDescription();
							case EnumType.JournalEventNameType:
								return explicitValue.JournalEventNameTypeValue.ToDescription();
							case EnumType.JournalObjectType:
								return explicitValue.JournalObjectTypeValue.ToDescription();
							case EnumType.ColorType:
								return explicitValue.ColorValue.ToString();
						}
					}
					break;
				case ExplicitType.Object:
				default:
					return UidToObjectName(explicitValue.UidValue);
			}
			return "";
		}

		static string UidToObjectName(Guid uid)
		{
			if (uid == Guid.Empty)
				return "";
			var device = GKManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == uid);
			if (device != null)
				return device.PresentationName;
			var zone = GKManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.UID == uid);
			if (zone != null)
				return zone.PresentationName;
			var guardZone = GKManager.DeviceConfiguration.GuardZones.FirstOrDefault(x => x.UID == uid);
			if (guardZone != null)
				return guardZone.PresentationName;
			var camera = ClientManager.SystemConfiguration.Cameras.FirstOrDefault(x => x.UID == uid);
			if (camera != null)
				return camera.PresentationName;
			var gKDoor = GKManager.Doors.FirstOrDefault(x => x.UID == uid);
			if (gKDoor != null)
				return gKDoor.PresentationName;
			var direction = GKManager.DeviceConfiguration.Directions.FirstOrDefault(x => x.UID == uid);
			if (direction != null)
				return direction.PresentationName;
			var delay = GKManager.DeviceConfiguration.Delays.FirstOrDefault(x => x.UID == uid);
			if (delay != null)
				return delay.PresentationName;
			var pumpStation = GKManager.DeviceConfiguration.PumpStations.FirstOrDefault(x => x.UID == uid);
			if (pumpStation != null)
				return pumpStation.PresentationName;
			var mpt = GKManager.DeviceConfiguration.MPTs.FirstOrDefault(x => x.UID == uid);
			if (mpt != null)
				return mpt.PresentationName;
			var organisation = OrganisationHelper.GetSingle(uid);
			if (organisation != null)
				return organisation.Name;
			return "";
		}
	}
}
