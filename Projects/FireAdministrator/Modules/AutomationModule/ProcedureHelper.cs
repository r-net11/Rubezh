using AutomationModule.ViewModels;
using FiresecAPI;
using FiresecAPI.Automation;
using FiresecAPI.Journal;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrustructure.Plans.Elements;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Property = FiresecAPI.Automation.Property;

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

		public static List<Variable> GetAllVariables(Procedure procedure, ExplicitType explicitType, ObjectType objectType = ObjectType.SKDDevice, EnumType enumType = EnumType.DriverType)
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
			foreach (var elementRectangle in allElements)
			{
				elements.Add(new ElementViewModel(elementRectangle));
			}
			return elements;
		}

		public static List<Variable> GetAllVariables(Procedure procedure, ExplicitType explicitType, ObjectType objectType, bool isList)
		{
			return GetAllVariables(procedure).FindAll(x => x.ExplicitType == explicitType && x.ObjectType == objectType && x.IsList == isList);
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
			if (objectType == ObjectType.SKDDevice
				|| objectType == ObjectType.Door)
				return new List<Property>
				{
					Property.AccessState,
					Property.DoorStatus,
					Property.BreakInStatus,
					Property.ConnectionStatus
				};
			if (objectType == ObjectType.SKDZone)
				return new List<Property>
				{
					Property.DoorStatus,
					Property.BreakInStatus,
					Property.ConnectionStatus
				};
			return new List<Property>();
		}

		public static List<ConditionType> ObjectTypeToConditionTypesList(ExplicitType explicitType)
		{
			if ((explicitType == ExplicitType.Integer) || (explicitType == ExplicitType.DateTime) || (explicitType == ExplicitType.Object) || explicitType == ExplicitType.Enum)
				return new List<ConditionType> { ConditionType.IsEqual, ConditionType.IsNotEqual, ConditionType.IsMore, ConditionType.IsNotMore, ConditionType.IsLess, ConditionType.IsNotLess };
			if ((explicitType == ExplicitType.Boolean) || (explicitType == ExplicitType.Object))
				return new List<ConditionType> { ConditionType.IsEqual, ConditionType.IsNotEqual };
			if (explicitType == ExplicitType.String)
				return new List<ConditionType> { ConditionType.IsEqual, ConditionType.IsNotEqual, ConditionType.StartsWith, ConditionType.EndsWith, ConditionType.Contains };
			return new List<ConditionType>();
		}

		public static ObservableCollection<T> GetEnumObs<T>()
		{
			// Убираем "мертвые" поля для EnumType
			if (typeof(T) == typeof(EnumType))
				return new ObservableCollection<T>(GetEnumList<T>());

			// Из JournalEventDescriptionType нам нужно не все
			if (typeof(T) == typeof(JournalEventDescriptionType))
				return new ObservableCollection<T>(GetEnumList<T>());

			return new ObservableCollection<T>(Enum.GetValues(typeof(T)).Cast<T>().ToList());
		}

		public static List<T> GetEnumList<T>()
		{
			// Убираем "мертвые" поля для EnumType
			if (typeof(T) == typeof(EnumType))
			{
				var enumTypes = Enum.GetValues(typeof(EnumType)).Cast<EnumType>().Where(e =>
					e != EnumType.StateType
					&& e != EnumType.DriverType
					&& e != EnumType.JournalObjectType);
				return new List<T>(enumTypes.Cast<T>().ToList());
			}

			// Из JournalEventDescriptionType нам нужно не все
			if (typeof (T) == typeof (JournalEventDescriptionType))
			{
				var journalEventDescriptionTypes = Enum.GetValues(typeof(JournalEventDescriptionType)).Cast<JournalEventDescriptionType>().Where(e =>
					e == JournalEventDescriptionType.NULL
					|| e == JournalEventDescriptionType.Метод_открытия_Пароль
					|| e == JournalEventDescriptionType.Метод_открытия_Карта
					|| e == JournalEventDescriptionType.Метод_открытия_Сначала_карта
					|| e == JournalEventDescriptionType.Метод_открытия_Удаленно
					|| e == JournalEventDescriptionType.Метод_открытия_Кнопка);
				return new List<T>(journalEventDescriptionTypes.Cast<T>().ToList());
			}
			
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

			if (objectType == ObjectType.Organisation)
			{
				var organisationSelectionViewModel = new OrganisationSelectionViewModel(currentExplicitValue.Organisation);
				if (DialogService.ShowModalWindow(organisationSelectionViewModel))
				{
					currentExplicitValue.UidValue = organisationSelectionViewModel.SelectedOrganisation != null ? organisationSelectionViewModel.SelectedOrganisation.Organisation.UID : Guid.Empty;
					return true;
				}
			}

			if (objectType == ObjectType.User)
			{
				var userSelectionViewModel = new UserSelectionViewModel(currentExplicitValue.User);
				if (DialogService.ShowModalWindow(userSelectionViewModel))
				{
					currentExplicitValue.UidValue = userSelectionViewModel.SelectedUser == null
						? Guid.Empty
						: userSelectionViewModel.SelectedUser.User.UID;
					return true;
				}
			}

			if (objectType == ObjectType.Employee)
			{
				var employeeSelectionViewModel = new EmployeeSelectionViewModel(currentExplicitValue.Employee);
				if (DialogService.ShowModalWindow(employeeSelectionViewModel))
				{
					currentExplicitValue.UidValue = employeeSelectionViewModel.SelectedEmployee == null
						? Guid.Empty
						: employeeSelectionViewModel.SelectedEmployee.Uid;
					return true;
				}
			}

			if (objectType == ObjectType.Visitor)
			{
				var visitorSelectionViewModel = new VisitorSelectionViewModel(currentExplicitValue.Visitor);
				if (DialogService.ShowModalWindow(visitorSelectionViewModel))
				{
					currentExplicitValue.UidValue = visitorSelectionViewModel.SelectedVisitor == null
						? Guid.Empty
						: visitorSelectionViewModel.SelectedVisitor.Uid;
					return true;
				}
			}

			return false;
		}

		public static string GetStringValue(ExplicitValue explicitValue, ExplicitType explicitType, EnumType enumType)
		{
			var result = "";
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
						if (enumType == EnumType.PermissionType)
							result = explicitValue.PermissionTypeValue.ToDescription();
						if (enumType == EnumType.JournalEventDescriptionType)
							result = explicitValue.JournalEventDescriptionTypeValue.ToDescription();
						if (enumType == EnumType.JournalEventNameType)
							result = explicitValue.JournalEventNameTypeValue.ToDescription();
						if (enumType == EnumType.JournalObjectType)
							result = explicitValue.JournalObjectTypeValue.ToDescription();
						if (enumType == EnumType.ColorType)
							result = explicitValue.ColorValue.ToString();
						if (enumType == EnumType.CardType)
							result = explicitValue.CardTypeValue.ToDescription();
						if (enumType == EnumType.AccessState)
							result = explicitValue.AccessStateValue.ToDescription();
						if (enumType == EnumType.DoorStatus)
							result = explicitValue.DoorStatusValue.ToDescription();
						if (enumType == EnumType.BreakInStatus)
							result = explicitValue.BreakInStatusValue.ToDescription();
						if (enumType == EnumType.ConnectionStatus)
							result = explicitValue.ConnectionStatusValue.ToDescription();
					}
					break;
				case ExplicitType.Object:
					{
						var automationChanged = ServiceFactory.SaveService.AutomationChanged;
						result = new ExplicitValueViewModel(explicitValue).PresentationName;
						ServiceFactory.SaveService.AutomationChanged = automationChanged;
					}
					break;
			}
			return result;
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

		public static string GetIconForProcedure(ProcedureStepType procedureStepType)
		{
			switch (procedureStepType)
			{
				case ProcedureStepType.RviAlarm:
				case ProcedureStepType.StopRecord:
				case ProcedureStepType.StartRecord:
				case ProcedureStepType.Ptz:
				case ProcedureStepType.ControlSKDZone:
				case ProcedureStepType.ControlSKDDevice:
				case ProcedureStepType.ControlDoor:
					return "/Controls;component/StepIcons/Control.png";
				case ProcedureStepType.ExportReport:
				case ProcedureStepType.ExportOrganisationList:
				case ProcedureStepType.ExportConfiguration:
				case ProcedureStepType.ExportOrganisation:
				case ProcedureStepType.ExportJournal:
					return "/Controls;component/StepIcons/Export.png";
				case ProcedureStepType.ImportOrganisationList:
				case ProcedureStepType.ImportOrganisation:
					return "/Controls;component/StepIcons/Import.png";
				default:
					return "/Controls;component/StepIcons/" + procedureStepType + ".png";
			}
		}
	}
}