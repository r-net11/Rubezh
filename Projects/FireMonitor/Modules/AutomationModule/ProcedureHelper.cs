using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Automation;
using FiresecClient;
using FiresecAPI.GK;
using System.Collections.ObjectModel;
using Infrastructure.Common.Windows;
using AutomationModule.ViewModels;
using FiresecAPI;
using Infrastructure.Common;
using System.Threading;
using FiresecClient.SKDHelpers;

namespace AutomationModule
{
	public static class ProcedureHelper
	{
		public static List<Variable> GetAllVariables(Procedure procedure)
		{
			var allVariables = new List<Variable>(FiresecManager.SystemConfiguration.AutomationConfiguration.GlobalVariables);
			allVariables.AddRange(procedure.Variables);
			allVariables.AddRange(procedure.Arguments);
			return allVariables;
		}

		public static List<Variable> GetAllVariables(Procedure procedure, ExplicitType ExplicitType, ObjectType objectType, bool isList)
		{
			return GetAllVariables(procedure).FindAll(x => x.ExplicitType == ExplicitType && x.ObjectType == objectType && x.IsList == isList);
		}

		public static List<Property> ObjectTypeToProperiesList(ObjectType objectType)
		{
			if (objectType == ObjectType.Device)
				return new List<Property> { Property.Description, Property.ShleifNo, Property.IntAddress, Property.State, Property.Type };
			if (objectType == ObjectType.Zone)
				return new List<Property> { Property.Description, Property.No, Property.Type, Property.State };
			if (objectType == ObjectType.Direction)
				return new List<Property> { Property.Description, Property.No, Property.Delay, Property.Hold, Property.DelayRegime };
			return new List<Property>();
		}

		public static List<ConditionType> ObjectTypeToConditionTypesList(ExplicitType ExplicitType)
		{
			if ((ExplicitType == ExplicitType.Integer) || (ExplicitType == ExplicitType.DateTime) || (ExplicitType == ExplicitType.Object))
				return new List<ConditionType> { ConditionType.IsEqual, ConditionType.IsNotEqual, ConditionType.IsMore, ConditionType.IsNotMore, ConditionType.IsLess, ConditionType.IsNotLess };
			if (ExplicitType == ExplicitType.Boolean || ExplicitType == ExplicitType.Enum)
				return new List<ConditionType> { ConditionType.IsEqual, ConditionType.IsNotEqual };
			if (ExplicitType == ExplicitType.String)
				return new List<ConditionType> { ConditionType.IsEqual, ConditionType.IsNotEqual, ConditionType.StartsWith, ConditionType.EndsWith, ConditionType.Contains };
			return new List<ConditionType>();
		}

		public static ObservableCollection<T> GetEnumObs<T>()
		{
			return new ObservableCollection<T>(Enum.GetValues(typeof(T)).Cast<T>().ToList());
		}

		public static List<T> GetEnumList<T>()
		{
			return new List<T>(Enum.GetValues(typeof(T)).Cast<T>());
		}

		public static bool SelectObject(ObjectType objectType, ExplicitValueViewModel currentExplicitValue)
		{
			if (objectType == ObjectType.Device)
			{
				var deviceSelectationViewModel = new DeviceSelectionViewModel(currentExplicitValue.Device != null ? currentExplicitValue.Device : null);
				if (DialogService.ShowModalWindow(deviceSelectationViewModel))
				{
					currentExplicitValue.UidValue = deviceSelectationViewModel.SelectedDevice != null ? deviceSelectationViewModel.SelectedDevice.Device.UID : Guid.Empty;
					return true;
				}
			}

			if (objectType == ObjectType.Zone)
			{
				var zoneSelectationViewModel = new ZoneSelectionViewModel(currentExplicitValue.Zone != null ? currentExplicitValue.Zone : null);
				if (DialogService.ShowModalWindow(zoneSelectationViewModel))
				{
					currentExplicitValue.UidValue = zoneSelectationViewModel.SelectedZone != null ? zoneSelectationViewModel.SelectedZone.Zone.UID : Guid.Empty;
					return true;
				}
			}

			if (objectType == ObjectType.GuardZone)
			{
				var guardZoneSelectationViewModel = new GuardZoneSelectionViewModel(currentExplicitValue.GuardZone != null ? currentExplicitValue.GuardZone : null);
				if (DialogService.ShowModalWindow(guardZoneSelectationViewModel))
				{
					currentExplicitValue.UidValue = guardZoneSelectationViewModel.SelectedZone != null ? guardZoneSelectationViewModel.SelectedZone.GuardZone.UID : Guid.Empty;
					return true;
				}
			}

			if (objectType == ObjectType.Direction)
			{
				var directionSelectationViewModel = new DirectionSelectionViewModel(currentExplicitValue.Direction != null ? currentExplicitValue.Direction : null);
				if (DialogService.ShowModalWindow(directionSelectationViewModel))
				{
					currentExplicitValue.UidValue = directionSelectationViewModel.SelectedDirection != null ? directionSelectationViewModel.SelectedDirection.Direction.UID : Guid.Empty;
					return true;
				}
			}

			if (objectType == ObjectType.VideoDevice)
			{
				var cameraSelectionViewModel = new CameraSelectionViewModel(currentExplicitValue.Camera != null ? currentExplicitValue.Camera : null);
				if (DialogService.ShowModalWindow(cameraSelectionViewModel))
				{
					currentExplicitValue.UidValue = cameraSelectionViewModel.SelectedCamera != null ? cameraSelectionViewModel.SelectedCamera.Camera.UID : Guid.Empty;
					return true;
				}
			}

			if (objectType == ObjectType.Delay)
			{
				var delaySelectionViewModel = new DelaySelectionViewModel(currentExplicitValue.Delay);
				if (DialogService.ShowModalWindow(delaySelectionViewModel))
				{
					currentExplicitValue.UidValue = delaySelectionViewModel.SelectedDelay != null ? delaySelectionViewModel.SelectedDelay.Delay.UID : Guid.Empty;
					return true;
				}
			}
			return false;
		}

		public static string GetStringValue(object obj)
		{
			if (obj == null)
				return "";

			var objType = obj.GetType(); 
			if (objType == typeof(bool))
				return (bool)obj ? "Да" : "Нет";
			
			if (objType.IsEnum)
				return ((Enum)obj).ToDescription();

			if (objType == typeof(Guid))
				return UidToObjectName((Guid)obj);

			return obj.ToString();
		}
		public static string GetStringValue(ExplicitValue explicitValue, ExplicitType explicitType, EnumType enumType)
		{
			switch (explicitType)
			{
				case ExplicitType.Boolean:
					return explicitValue.BoolValue.ToString();
				case ExplicitType.DateTime:
					return explicitValue.DateTimeValue.ToString();
				case ExplicitType.Integer:
					return explicitValue.IntValue.ToString();
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
			var camera = FiresecManager.SystemConfiguration.Cameras.FirstOrDefault(x => x.UID == uid);
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
			var organisation = OrganisationHelper.GetSingle(uid);
			if (organisation != null)
				return organisation.Name;
			return "";
		}

		public static void Run(Procedure procedure, List<Argument> args = null)
		{
			if (args == null)
				args = new List<Argument>();
			using (new WaitWrapper())
			{
				var thread = new Thread(() => FiresecManager.FiresecService.RunProcedure(procedure.Uid, args))
				{
					Name = "Run Procedure",
				};
				thread.Start();
				while (!thread.Join(50))
					ApplicationService.DoEvents();
			}
		}
	}
}