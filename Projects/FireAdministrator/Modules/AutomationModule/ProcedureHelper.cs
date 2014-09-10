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
				return new List<Property> { Property.Description, Property.ShleifNo, Property.IntAddress, Property.DeviceState, Property.Type };
			if (objectType == ObjectType.Zone)
				return new List<Property> { Property.Description, Property.No, Property.Type };
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

		public static VariableItemViewModel SelectObject(ObjectType objectType, VariableItemViewModel currentVariableItem)
		{
			var variableItem = new VariableItem();
			variableItem.ExplicitType = ExplicitType.Object;
			variableItem.UidValue = currentVariableItem.VariableItem.UidValue;
			if (objectType == ObjectType.Device)
			{
				var deviceSelectationViewModel = new DeviceSelectionViewModel(currentVariableItem.Device != null ? currentVariableItem.Device : null);
				if (DialogService.ShowModalWindow(deviceSelectationViewModel))
					variableItem.UidValue = deviceSelectationViewModel.SelectedDevice.Device.UID;
			}

			if (objectType == ObjectType.Zone)
			{
				var zoneSelectationViewModel = new ZoneSelectionViewModel(currentVariableItem.Zone != null ? currentVariableItem.Zone : null);
				if (DialogService.ShowModalWindow(zoneSelectationViewModel))
					variableItem.UidValue = zoneSelectationViewModel.SelectedZone.Zone.UID;
			}

			if (objectType == ObjectType.GuardZone)
			{
				var guardZoneSelectationViewModel = new GuardZoneSelectionViewModel(currentVariableItem.GuardZone != null ? currentVariableItem.GuardZone : null);
				if (DialogService.ShowModalWindow(guardZoneSelectationViewModel))
					variableItem.UidValue = guardZoneSelectationViewModel.SelectedZone.GuardZone.UID;
			}

			if (objectType == ObjectType.SKDDevice)
			{
				var skdDeviceSelectationViewModel = new SKDDeviceSelectionViewModel(currentVariableItem.SKDDevice != null ? currentVariableItem.SKDDevice : null);
				if (DialogService.ShowModalWindow(skdDeviceSelectationViewModel))
					variableItem.UidValue = skdDeviceSelectationViewModel.SelectedDevice.SKDDevice.UID;
			}

			if (objectType == ObjectType.SKDZone)
			{
				var skdZoneSelectationViewModel = new SKDZoneSelectionViewModel(currentVariableItem.SKDZone != null ? currentVariableItem.SKDZone : null);
				if (DialogService.ShowModalWindow(skdZoneSelectationViewModel))
					variableItem.UidValue = skdZoneSelectationViewModel.SelectedZone.SKDZone.UID;
			}

			if (objectType == ObjectType.ControlDoor)
			{
				var doorSelectationViewModel = new DoorSelectionViewModel(currentVariableItem.SKDDoor != null ? currentVariableItem.SKDDoor : null);
				if (DialogService.ShowModalWindow(doorSelectationViewModel))
					variableItem.UidValue = doorSelectationViewModel.SelectedDoor.Door.UID;
			}

			if (objectType == ObjectType.Direction)
			{
				var directionSelectationViewModel = new DirectionSelectionViewModel(currentVariableItem.Direction != null ? currentVariableItem.Direction : null);
				if (DialogService.ShowModalWindow(directionSelectationViewModel))
					variableItem.UidValue = directionSelectationViewModel.SelectedDirection.Direction.UID;
			}

			if (objectType == ObjectType.VideoDevice)
			{
				var cameraSelectionViewModel = new CameraSelectionViewModel(currentVariableItem.Camera != null ? currentVariableItem.Camera : null);
				if (DialogService.ShowModalWindow(cameraSelectionViewModel))
					variableItem.UidValue = cameraSelectionViewModel.SelectedCamera.Camera.UID;
			}

			return new VariableItemViewModel(variableItem);
		}
	}
}