using AutomationModule.ViewModels;
using StrazhAPI;
using StrazhAPI.Automation;
using StrazhAPI.Models.Automation;
using FiresecClient;
using Infrastructure.Common.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace AutomationModule
{
	public static class ProcedureHelper
	{
		public static List<IVariable> GetAllVariables(Procedure procedure)
		{
			var globalVariables = FiresecManager.FiresecService.GetInitialGlobalVariables().Result;
			var allVariables = globalVariables.ToList<IVariable>();//new List<IVariable>();//(FiresecManager.SystemConfiguration.AutomationConfiguration.GlobalVariables); TODO: Get Global Variables
			allVariables.AddRange(procedure.Variables);
			allVariables.AddRange(procedure.Arguments);
			return allVariables;
		}

		public static List<IVariable> GetAllVariables(Procedure procedure, ExplicitType explicitType, ObjectType objectType)
		{
			return GetAllVariables(procedure).FindAll(x => x.VariableValue.ExplicitType == explicitType && x.VariableValue.ObjectType == objectType);
		}

		public static List<Property> ObjectTypeToProperiesList(ObjectType objectType)
		{
			if (objectType == ObjectType.SKDDevice)
				return new List<Property> { Property.Description, Property.ShleifNo, Property.IntAddress, Property.State, Property.Type };
			return new List<Property>();
		}

		public static List<ConditionType> ObjectTypeToConditionTypesList(ExplicitType explicitType)
		{
			if ((explicitType == ExplicitType.Integer) || (explicitType == ExplicitType.DateTime) || (explicitType == ExplicitType.Object))
				return new List<ConditionType> { ConditionType.IsEqual, ConditionType.IsNotEqual, ConditionType.IsMore, ConditionType.IsNotMore, ConditionType.IsLess, ConditionType.IsNotLess };
			if (explicitType == ExplicitType.Boolean || explicitType == ExplicitType.Enum)
				return new List<ConditionType> { ConditionType.IsEqual, ConditionType.IsNotEqual };
			if (explicitType == ExplicitType.String)
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
			if (objectType == ObjectType.SKDDevice)
			{
				var skdDeviceSelectationViewModel = new SKDDeviceSelectionViewModel(currentExplicitValue.SKDDevice);
				if (DialogService.ShowModalWindow(skdDeviceSelectationViewModel))
				{
					currentExplicitValue.UidValue = skdDeviceSelectationViewModel.SelectedDevice != null ? skdDeviceSelectationViewModel.SelectedDevice.SKDDevice.UID : Guid.Empty;
					return true;
				}
			}

			if (objectType == ObjectType.SKDZone)
			{
				var skdZoneSelectationViewModel = new SKDZoneSelectionViewModel(currentExplicitValue.SKDZone);
				if (DialogService.ShowModalWindow(skdZoneSelectationViewModel))
				{
					currentExplicitValue.UidValue = skdZoneSelectationViewModel.SelectedZone != null ? skdZoneSelectationViewModel.SelectedZone.SKDZone.UID : Guid.Empty;
					return true;
				}
			}

			if (objectType == ObjectType.Door)
			{
				var doorSelectationViewModel = new DoorSelectionViewModel(currentExplicitValue.SKDDoor);
				if (DialogService.ShowModalWindow(doorSelectationViewModel))
				{
					currentExplicitValue.UidValue = doorSelectationViewModel.SelectedDoor != null ? doorSelectationViewModel.SelectedDoor.Door.UID : Guid.Empty;
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
			return false;
		}

		public static string GetStringValue(ExplicitValue explicitValue, ExplicitType explicitType, EnumType enumType)
		{
			var result = string.Empty;

			switch (explicitType)
			{
				case ExplicitType.Boolean:
					result = explicitValue.BoolValue.ToString();
					break;
				case ExplicitType.DateTime:
					result = explicitValue.DateTimeValue.ToString();
					break;
				case ExplicitType.Integer:
					result = explicitValue.IntValue.ToString();
					break;
				case ExplicitType.String:
					result = explicitValue.StringValue;
					break;
				case ExplicitType.Enum:
					{
						if (enumType == EnumType.StateType)
							result = explicitValue.StateTypeValue.ToDescription();
					}
					break;
				case ExplicitType.Object:
					{
						result = new ExplicitValueViewModel(explicitValue).PresentationName;
					}
					break;
			}

			return result;
		}

		public static void Run(Procedure procedure, List<Argument> args = null)
		{
			if (args == null)
				args = new List<Argument>();

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