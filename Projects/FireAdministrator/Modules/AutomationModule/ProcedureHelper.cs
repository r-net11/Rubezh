using AutomationModule.ViewModels;
using StrazhAPI;
using StrazhAPI.Automation;
using StrazhAPI.Journal;
using StrazhAPI.Models;
using StrazhAPI.Models.Automation;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows;
using StrazhAPI.Plans.Elements;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Property = StrazhAPI.Automation.Property;

namespace AutomationModule
{
	public static class ProcedureHelper
	{
		public static List<IVariable> GetAllVariables(Procedure procedure)
		{
			var globalVariables = FiresecManager.FiresecService.GetInitialGlobalVariables().Result;
			var allVariables = globalVariables.ToList<IVariable>();
			allVariables.AddRange(procedure.Variables);
			allVariables.AddRange(procedure.Arguments);
			return allVariables;
		}

		public static List<IVariable> GetAllVariables(Procedure procedure, ExplicitType explicitType, ObjectType objectType = ObjectType.SKDDevice, EnumType enumType = EnumType.DriverType)
		{
			var allVariables = GetAllVariables(procedure).FindAll(x => x.VariableValue.ExplicitType == explicitType);
			if (explicitType == ExplicitType.Enum)
				allVariables = allVariables.FindAll(x => x.VariableValue.EnumType == enumType);
			if (explicitType == ExplicitType.Object)
				allVariables = allVariables.FindAll(x => x.VariableValue.ObjectType == objectType);
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

		public static List<IVariable> GetAllVariables(Procedure procedure, ExplicitType explicitType, ObjectType objectType)
		{
			return GetAllVariables(procedure).FindAll(x => x.VariableValue.ExplicitType == explicitType && x.VariableValue.ObjectType == objectType);
		}

		public static List<IVariable> GetAllVariables(List<IVariable> allVariables, List<ExplicitType> explicitTypes, List<EnumType> enumTypes, List<ObjectType> objectTypes)
		{
			var variables = new List<IVariable>(allVariables);
			if (explicitTypes == null) return variables;

			variables = variables.FindAll(x => explicitTypes.Contains(x.VariableValue.ExplicitType));

			if (explicitTypes.Contains(ExplicitType.Enum))
			{
				variables = variables.FindAll(x => enumTypes.Contains(x.VariableValue.EnumType));
			}

			if (explicitTypes.Contains(ExplicitType.Object))
			{
				variables = variables.FindAll(x => objectTypes.Contains(x.VariableValue.ObjectType));
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
			if (explicitType == ExplicitType.Integer
				|| explicitType == ExplicitType.DateTime
				|| explicitType == ExplicitType.Object
				|| explicitType == ExplicitType.Enum
				|| explicitType == ExplicitType.Time)
				return new List<ConditionType>
				{
					ConditionType.IsEqual,
					ConditionType.IsNotEqual,
					ConditionType.IsMore,
					ConditionType.IsNotMore,
					ConditionType.IsLess,
					ConditionType.IsNotLess
				};

			if (explicitType == ExplicitType.Boolean
				|| explicitType == ExplicitType.Object)
				return new List<ConditionType>
				{
					ConditionType.IsEqual,
					ConditionType.IsNotEqual
				};

			if (explicitType == ExplicitType.String)
				return new List<ConditionType>
				{
					ConditionType.IsEqual,
					ConditionType.IsNotEqual,
					ConditionType.StartsWith,
					ConditionType.EndsWith,
					ConditionType.Contains
				};

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
			if (explicitValue == null) return null;

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
						switch (enumType)
						{
							case EnumType.StateType:
								result = explicitValue.StateTypeValue.ToDescription();
								break;
							case EnumType.PermissionType:
								result = explicitValue.PermissionTypeValue.ToDescription();
								break;
							case EnumType.JournalEventDescriptionType:
								result = explicitValue.JournalEventDescriptionTypeValue.ToDescription();
								break;
							case EnumType.JournalEventNameType:
								result = explicitValue.JournalEventNameTypeValue.ToDescription();
								break;
							case EnumType.JournalObjectType:
								result = explicitValue.JournalObjectTypeValue.ToDescription();
								break;
							case EnumType.ColorType:
								result = explicitValue.ColorValue.ToString();
								break;
							case EnumType.CardType:
								result = explicitValue.CardTypeValue.ToDescription();
								break;
							case EnumType.AccessState:
								result = explicitValue.AccessStateValue.HasValue ? explicitValue.AccessStateValue.ToDescription() : "NULL";
								break;
							case EnumType.DoorStatus:
								result = explicitValue.DoorStatusValue.HasValue ? explicitValue.DoorStatusValue.ToDescription() : "NULL";
								break;
							case EnumType.BreakInStatus:
								result = explicitValue.BreakInStatusValue.HasValue ? explicitValue.BreakInStatusValue.ToDescription() : "NULL";
								break;
							case EnumType.ConnectionStatus:
								result = explicitValue.ConnectionStatusValue.HasValue ? explicitValue.ConnectionStatusValue.ToDescription() : "NULL";
								break;
						}
					}
					break;
				case ExplicitType.Object:
					{
						var automationChanged = ServiceFactory.SaveService.AutomationChanged;
						result = new ExplicitValueViewModel(explicitValue).PresentationName;
						ServiceFactory.SaveService.AutomationChanged = automationChanged;
					}
					break;
				case ExplicitType.Time:
					result = explicitValue.TimeSpanValue.ToString();
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
				case ProcedureStepType.SendOPCScript:
				case ProcedureStepType.ExecuteFiresecScript:
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