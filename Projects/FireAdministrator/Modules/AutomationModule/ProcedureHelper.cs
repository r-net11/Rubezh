using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Automation;
using FiresecClient;
using System.Collections.ObjectModel;
using Infrastructure;
using Infrastructure.Common.Windows;
using AutomationModule.ViewModels;
using FiresecAPI;
using System.Drawing;
using Infrustructure.Plans.Elements;
using FiresecAPI.Models;
using Property = FiresecAPI.Automation.Property;
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

		public static List<Variable> GetAllVariables(Procedure procedure, ExplicitType explicitType, ObjectType objectType = ObjectType.Device, EnumType enumType = EnumType.DriverType)
		{
			var allVariables = GetAllVariables(procedure).FindAll(x => x.ExplicitType == explicitType);
			if (explicitType == ExplicitType.Enum)
				allVariables = allVariables.FindAll(x => x.EnumType == enumType);
			if (explicitType == ExplicitType.Object)
				allVariables = allVariables.FindAll(x => x.ObjectType == objectType);
			return allVariables;
		}

		public static ObservableCollection<ElementViewModel> GetAllElements(Plan plan)
		{
			var elements = new ObservableCollection<ElementViewModel>();
			var allElements = new List<ElementBase>(plan.ElementRectangles);
			allElements.AddRange(plan.ElementEllipses);
			allElements.AddRange(plan.ElementPolylines);
			allElements.AddRange(plan.ElementTextBlocks);
			allElements.AddRange(plan.ElementPolygons);
			allElements.AddRange(plan.ElementExtensions);
			foreach (var elementRectangle in allElements)
			{
				elements.Add(new ElementViewModel(elementRectangle));
			}
			return elements;
		}

		public static List<Variable> GetAllVariables(Procedure procedure, ExplicitType ExplicitType, ObjectType objectType, bool isList)
		{
			return GetAllVariables(procedure).FindAll(x => x.ExplicitType == ExplicitType && x.ObjectType == objectType && x.IsList == isList);
		}

		public static List<Variable> GetAllVariables(List<Variable> allVariables, List<ExplicitType> explicitTypes, List<EnumType> enumTypes, List<ObjectType> objectTypes, bool? isList = null)
		{
			var variables = new List<Variable>(allVariables);
			if (explicitTypes != null)
			{
				variables = variables.FindAll(x => explicitTypes.Contains(x.ExplicitType));
				if (explicitTypes.Contains(ExplicitType.Enum))
				{
					variables = variables.FindAll(x => enumTypes.Contains(x.EnumType));
				}
				if (explicitTypes.Contains(ExplicitType.Object))
				{
					variables = variables.FindAll(x => objectTypes.Contains(x.ObjectType));
				}
			}
			if (isList != null)
			{
				variables = variables.FindAll(x => x.IsList == isList);
			}
			return variables;
		}

		public static List<Property> ObjectTypeToProperiesList(ObjectType objectType)
		{
			if (objectType == ObjectType.Device)
				return new List<Property> { Property.Description, Property.ShleifNo, Property.IntAddress, Property.State, Property.Type, Property.Uid };
			if (objectType == ObjectType.Zone)
				return new List<Property> { Property.Description, Property.No, Property.Uid, Property.Name, Property.State };
			if (objectType == ObjectType.Direction)
				return new List<Property> { Property.Description, Property.No, Property.Delay, Property.CurrentDelay, Property.Hold, Property.CurrentHold, Property.DelayRegime, Property.Uid, Property.Name, Property.State };
			if (objectType == ObjectType.Delay)
				return new List<Property> { Property.Description, Property.No, Property.Delay, Property.CurrentDelay, Property.Hold, Property.CurrentHold, Property.DelayRegime, Property.Uid, Property.Name, Property.State };
			if (objectType == ObjectType.GuardZone)
				return new List<Property> { Property.Description, Property.No, Property.Uid, Property.Name, Property.State };
			return new List<Property>();
		}

		public static List<ConditionType> ObjectTypeToConditionTypesList(ExplicitType ExplicitType)
		{
			if ((ExplicitType == ExplicitType.Integer) || (ExplicitType == ExplicitType.DateTime) || (ExplicitType == ExplicitType.Object) || ExplicitType == ExplicitType.Enum)
				return new List<ConditionType> { ConditionType.IsEqual, ConditionType.IsNotEqual, ConditionType.IsMore, ConditionType.IsNotMore, ConditionType.IsLess, ConditionType.IsNotLess };
			if ((ExplicitType == ExplicitType.Boolean) || (ExplicitType == ExplicitType.Object))
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
				var deviceSelectationViewModel = new DeviceSelectionViewModel(currentExplicitValue.Device);
				if (DialogService.ShowModalWindow(deviceSelectationViewModel))
				{
					currentExplicitValue.UidValue = deviceSelectationViewModel.SelectedDevice != null ? deviceSelectationViewModel.SelectedDevice.Device.UID : Guid.Empty;
					return true;
				}
			}

			if (objectType == ObjectType.Zone)
			{
				var zoneSelectationViewModel = new ZoneSelectionViewModel(currentExplicitValue.Zone);
				if (DialogService.ShowModalWindow(zoneSelectationViewModel))
				{
					currentExplicitValue.UidValue = zoneSelectationViewModel.SelectedZone != null ? zoneSelectationViewModel.SelectedZone.Zone.UID : Guid.Empty;
					return true;
				}
			}

			if (objectType == ObjectType.GuardZone)
			{
				var guardZoneSelectationViewModel = new GuardZoneSelectionViewModel(currentExplicitValue.GuardZone);
				if (DialogService.ShowModalWindow(guardZoneSelectationViewModel))
				{
					currentExplicitValue.UidValue = guardZoneSelectationViewModel.SelectedZone != null ? guardZoneSelectationViewModel.SelectedZone.GuardZone.UID : Guid.Empty;
					return true;
				}
			}

			if (objectType == ObjectType.GKDoor)
			{
				var doorSelectationViewModel = new GKDoorSelectionViewModel(currentExplicitValue.GKDoor);
				if (DialogService.ShowModalWindow(doorSelectationViewModel))
				{
					currentExplicitValue.UidValue = doorSelectationViewModel.SelectedDoor != null ? doorSelectationViewModel.SelectedDoor.GKDoor.UID : Guid.Empty;
					return true;
				}
			}

			if (objectType == ObjectType.Direction)
			{
				var directionSelectationViewModel = new DirectionSelectionViewModel(currentExplicitValue.Direction);
				if (DialogService.ShowModalWindow(directionSelectationViewModel))
				{
					currentExplicitValue.UidValue = directionSelectationViewModel.SelectedDirection != null ? directionSelectationViewModel.SelectedDirection.Direction.UID : Guid.Empty;
					return true;
				}
			}

			if (objectType == ObjectType.VideoDevice)
			{
				var cameraSelectionViewModel = new CameraSelectionViewModel(currentExplicitValue.Camera);
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

			if (objectType == ObjectType.Organisation)
			{
				var organisationSelectionViewModel = new OrganisationSelectionViewModel(currentExplicitValue.Organisation);
				if (DialogService.ShowModalWindow(organisationSelectionViewModel))
				{
					currentExplicitValue.UidValue = organisationSelectionViewModel.SelectedOrganisation != null ? organisationSelectionViewModel.SelectedOrganisation.Organisation.UID : Guid.Empty;
					return true;
				}
			}
			return false;
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
					{
						if (explicitValue.UidValue == Guid.Empty)
							break;
						var device = GKManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == explicitValue.UidValue);
						if (device != null)
							return device.PresentationName;
						var zone = GKManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.UID == explicitValue.UidValue);
						if (zone != null)
							return zone.PresentationName;
						var guardZone = GKManager.DeviceConfiguration.GuardZones.FirstOrDefault(x => x.UID == explicitValue.UidValue);
						if (guardZone != null)
							return guardZone.PresentationName;
						var camera = FiresecManager.SystemConfiguration.Cameras.FirstOrDefault(x => x.UID == explicitValue.UidValue);
						if (camera != null)
							return camera.PresentationName;
						var gKDoor = GKManager.Doors.FirstOrDefault(x => x.UID == explicitValue.UidValue);
						if (gKDoor != null)
							return gKDoor.PresentationName;
						var direction = GKManager.DeviceConfiguration.Directions.FirstOrDefault(x => x.UID == explicitValue.UidValue);
						if (direction != null)
							return direction.PresentationName;
						var delay = GKManager.DeviceConfiguration.Delays.FirstOrDefault(x => x.UID == explicitValue.UidValue);
						if (delay != null)
							return delay.PresentationName;
						var organisation = OrganisationHelper.GetSingle(explicitValue.UidValue);
						if (organisation != null)
							return organisation.Name;
					}
					break;
			}
			return "";
		}

		public static List<ExplicitTypeViewModel> BuildExplicitTypes(List<ExplicitType> explicitTypes, List<EnumType> enumTypes, List<ObjectType> objectTypes)
		{
			var ExplicitTypes = new List<ExplicitTypeViewModel>();
			if (explicitTypes != null)
				ExplicitTypes.AddRange(explicitTypes.Select(explicitType => new ExplicitTypeViewModel(explicitType)));
			foreach (var enumType in enumTypes)
			{
				var explicitTypeViewModel = new ExplicitTypeViewModel(enumType);
				var parent = ExplicitTypes.FirstOrDefault(x => x.ExplicitType == ExplicitType.Enum);
				if (parent != null)
				{
					parent.AddChild(explicitTypeViewModel);
				}
			}
			foreach (var objectType in objectTypes)
			{
				var explicitTypeViewModel = new ExplicitTypeViewModel(objectType);
				var parent = ExplicitTypes.FirstOrDefault(x => x.ExplicitType == ExplicitType.Object);
				if (parent != null)
				{
					parent.AddChild(explicitTypeViewModel);
				}
			}
			return ExplicitTypes;
		}

		public static string GetProcedureName(Guid procedureUid)
		{
			var procedure = FiresecManager.SystemConfiguration.AutomationConfiguration.Procedures.FirstOrDefault(x => x.Uid == procedureUid);
			return procedure == null ? "" : procedure.Name;
		}
	}
}