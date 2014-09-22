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

		public static void SelectObject(ObjectType objectType, ExplicitValueViewModel currentExplicitValue)
		{
			if (objectType == ObjectType.Device)
			{
				var deviceSelectationViewModel = new DeviceSelectionViewModel(currentExplicitValue.Device != null ? currentExplicitValue.Device : null);
				if (DialogService.ShowModalWindow(deviceSelectationViewModel))
					currentExplicitValue.UidValue = deviceSelectationViewModel.SelectedDevice != null ? deviceSelectationViewModel.SelectedDevice.Device.UID : Guid.Empty;
			}

			if (objectType == ObjectType.Zone)
			{
				var zoneSelectationViewModel = new ZoneSelectionViewModel(currentExplicitValue.Zone != null ? currentExplicitValue.Zone : null);
				if (DialogService.ShowModalWindow(zoneSelectationViewModel))
					currentExplicitValue.UidValue = zoneSelectationViewModel.SelectedZone != null ? zoneSelectationViewModel.SelectedZone.Zone.UID : Guid.Empty;
			}

			if (objectType == ObjectType.GuardZone)
			{
				var guardZoneSelectationViewModel = new GuardZoneSelectionViewModel(currentExplicitValue.GuardZone != null ? currentExplicitValue.GuardZone : null);
				if (DialogService.ShowModalWindow(guardZoneSelectationViewModel))
					currentExplicitValue.UidValue = guardZoneSelectationViewModel.SelectedZone != null ? guardZoneSelectationViewModel.SelectedZone.GuardZone.UID : Guid.Empty;
			}

			if (objectType == ObjectType.SKDDevice)
			{
				var skdDeviceSelectationViewModel = new SKDDeviceSelectionViewModel(currentExplicitValue.SKDDevice != null ? currentExplicitValue.SKDDevice : null);
				if (DialogService.ShowModalWindow(skdDeviceSelectationViewModel))
					currentExplicitValue.UidValue = skdDeviceSelectationViewModel.SelectedDevice != null ? skdDeviceSelectationViewModel.SelectedDevice.SKDDevice.UID : Guid.Empty;
			}

			if (objectType == ObjectType.SKDZone)
			{
				var skdZoneSelectationViewModel = new SKDZoneSelectionViewModel(currentExplicitValue.SKDZone != null ? currentExplicitValue.SKDZone : null);
				if (DialogService.ShowModalWindow(skdZoneSelectationViewModel))
					currentExplicitValue.UidValue = skdZoneSelectationViewModel.SelectedZone != null ? skdZoneSelectationViewModel.SelectedZone.SKDZone.UID : Guid.Empty;
			}

			if (objectType == ObjectType.ControlDoor)
			{
				var doorSelectationViewModel = new DoorSelectionViewModel(currentExplicitValue.SKDDoor != null ? currentExplicitValue.SKDDoor : null);
				if (DialogService.ShowModalWindow(doorSelectationViewModel))
					currentExplicitValue.UidValue = doorSelectationViewModel.SelectedDoor != null ? doorSelectationViewModel.SelectedDoor.Door.UID : Guid.Empty;
			}

			if (objectType == ObjectType.Direction)
			{
				var directionSelectationViewModel = new DirectionSelectionViewModel(currentExplicitValue.Direction != null ? currentExplicitValue.Direction : null);
				if (DialogService.ShowModalWindow(directionSelectationViewModel))
					currentExplicitValue.UidValue = directionSelectationViewModel.SelectedDirection != null ? directionSelectationViewModel.SelectedDirection.Direction.UID : Guid.Empty;
			}

			if (objectType == ObjectType.VideoDevice)
			{
				var cameraSelectionViewModel = new CameraSelectionViewModel(currentExplicitValue.Camera != null ? currentExplicitValue.Camera : null);
				if (DialogService.ShowModalWindow(cameraSelectionViewModel))
					currentExplicitValue.UidValue = cameraSelectionViewModel.SelectedCamera != null ? cameraSelectionViewModel.SelectedCamera.Camera.UID : Guid.Empty;
			}
		}
	}
}