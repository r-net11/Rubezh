﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using FiresecAPI.Automation;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class ArgumentViewModel : BaseViewModel
	{
		public const string EmptyText = "<пусто>";
		public Action UpdateVariableScopeHandler { get; set; }
		public Action UpdateVariableHandler { get; set; }
		Action UpdateContentHandler { get; set; }
		Action UpdateDescriptionHandler { get; set; }
		public ExplicitValueViewModel ExplicitValue { get; protected set; }
		public ObservableCollection<ExplicitValueViewModel> ExplicitValues { get; set; }
		public Argument Argument { get; private set; }

		public ArgumentViewModel(Argument argument, Action updateDescriptionHandler, Action updateContentHandler, bool allowExplicitValue = true, bool allowLocalValue = true, bool allowGlobalValue = true)
		{
			AddCommand = new RelayCommand(OnAdd);
			RemoveCommand = new RelayCommand<ExplicitValueViewModel>(OnRemove);
			EditCommand = new RelayCommand(OnEdit);
			ChangeCommand = new RelayCommand<ExplicitValueViewModel>(OnChange);
			EditStringCommand = new RelayCommand(OnEditString);
			AddVariableCommand = new RelayCommand(OnAddVariable);

			Argument = argument;
			UpdateDescriptionHandler = updateDescriptionHandler;
			UpdateContentHandler = updateContentHandler;
			ExplicitValue = new ExplicitValueViewModel(argument.ExplicitValue);
			ExplicitValue.UpdateObjectHandler += () => OnPropertyChanged(() => IsEmpty);
			ExplicitValues = new ObservableCollection<ExplicitValueViewModel>();
			foreach (var explicitValue in argument.ExplicitValues)
				ExplicitValues.Add(new ExplicitValueViewModel(explicitValue));
			ExplicitValue.UpdateDescriptionHandler = updateDescriptionHandler;
			Variables = new List<VariableViewModel>();
			VariableScopes = new ObservableCollection<VariableScope>(ProcedureHelper.GetEnumList<VariableScope>().FindAll(x => (allowExplicitValue || x != VariableScope.ExplicitValue) && (allowLocalValue || x != VariableScope.LocalVariable) && (allowGlobalValue || x != VariableScope.GlobalVariable)));
			OnPropertyChanged(() => VariableScopes);
			ExplicitTypes = new List<ExplicitTypeViewModel>();
		}

		public ExplicitType ExplicitType
		{
			get { return Argument.ExplicitType; }
			set
			{
				Argument.ExplicitType = value;
				OnPropertyChanged(() => ExplicitType);
			}
		}

		public EnumType EnumType
		{
			get { return Argument.EnumType; }
			set
			{
				Argument.EnumType = value;
				OnPropertyChanged(() => EnumType);
			}
		}

		public ObjectType ObjectType
		{
			get { return Argument.ObjectType; }
			set
			{
				Argument.ObjectType = value;
				OnPropertyChanged(() => ObjectType);
				OnPropertyChanged(() => IsEmpty);
			}
		}

		public bool IsEmpty
		{
			get
			{
				return (((ObjectType == ObjectType.Door) && (ExplicitValue.SKDDoor == null))
						|| ((ObjectType == ObjectType.GKDoor) && (ExplicitValue.GKDoor == null))
						|| ((ObjectType == ObjectType.Device) && (ExplicitValue.Device == null))
						|| ((ObjectType == ObjectType.Delay) && (ExplicitValue.Delay == null))
						|| ((ObjectType == ObjectType.Direction) && (ExplicitValue.Direction == null))
						|| ((ObjectType == ObjectType.SKDDevice) && (ExplicitValue.SKDDevice == null))
						|| ((ObjectType == ObjectType.SKDZone) && (ExplicitValue.SKDZone == null))
						|| ((ObjectType == ObjectType.VideoDevice) && (ExplicitValue.Camera == null))
						|| ((ObjectType == ObjectType.Delay) && (ExplicitValue.Delay == null))
						|| ((ObjectType == ObjectType.Organisation) && (ExplicitValue.Organisation == null)));
			}
		}

		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged(() => Name);
			}
		}

		bool _isList;
		public bool IsList
		{
			get { return _isList; }
			set
			{
				_isList = value;
				OnPropertyChanged(() => IsList);
			}
		}

		public RelayCommand AddVariableCommand { get; private set; }
		void OnAddVariable()
		{
			var variableDetailsViewModel = new VariableDetailsViewModel(null, SelectedVariableScope == VariableScope.LocalVariable ? "локальная переменная" : "глобальная переменная",
				SelectedVariableScope == VariableScope.LocalVariable ? "Добавить локальную переменную" : "Добавить глобальную переменную");
			variableDetailsViewModel.IsList = IsList;
			variableDetailsViewModel.ExplicitTypes = new ObservableCollection<ExplicitTypeViewModel>(ExplicitTypes);
			var explicitTypeViewModel = variableDetailsViewModel.ExplicitTypes.FirstOrDefault();
			if (explicitTypeViewModel != null)
				variableDetailsViewModel.SelectedExplicitType = explicitTypeViewModel.GetAllChildren().FirstOrDefault(x => x.IsRealType);
			variableDetailsViewModel.IsEditMode = true;
			if (variableDetailsViewModel.SelectedExplicitType != null)
				variableDetailsViewModel.SelectedExplicitType.ExpandToThis();

			if (DialogService.ShowModalWindow(variableDetailsViewModel))
			{
				if (SelectedVariableScope == VariableScope.LocalVariable)
				{
					ProceduresViewModel.Current.SelectedProcedure.VariablesViewModel.Variables.Add(new VariableViewModel(variableDetailsViewModel.Variable));
					ProceduresViewModel.Current.SelectedProcedure.Procedure.Variables.Add(variableDetailsViewModel.Variable);
				}
				else
				{
					variableDetailsViewModel.Variable.IsGlobal = true;
					FiresecManager.SystemConfiguration.AutomationConfiguration.GlobalVariables.Add(variableDetailsViewModel.Variable);
					GlobalVariablesViewModel.Current.GlobalVariables.Add(new VariableViewModel(variableDetailsViewModel.Variable));
				}

				SelectedVariable = new VariableViewModel(variableDetailsViewModel.Variable);
				Variables.Add(SelectedVariable);

				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => LocalVariables);
				OnPropertyChanged(() => GlobalVariables);
				if (UpdateContentHandler != null)
					UpdateContentHandler();
			}
		}

		public bool AddVariableVisibility
		{
			get
			{
				return (ExplicitTypes != null && ExplicitTypes.Count > 0);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var explicitValue = new ExplicitValueViewModel();
			if (ExplicitType == ExplicitType.Object)
				if (!ProcedureHelper.SelectObject(ObjectType, explicitValue))
					return;
			ExplicitValues.Add(explicitValue);
			Argument.ExplicitValues.Add(explicitValue.ExplicitValue);
			OnPropertyChanged(() => ExplicitValues);
		}

		public RelayCommand<ExplicitValueViewModel> RemoveCommand { get; private set; }
		void OnRemove(ExplicitValueViewModel explicitValueViewModel)
		{
			ExplicitValues.Remove(explicitValueViewModel);
			Argument.ExplicitValues.Remove(explicitValueViewModel.ExplicitValue);
			OnPropertyChanged(() => ExplicitValues);
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var argumentDetailsViewModel = new ArgumentDetailsViewModel(Argument, IsList);
			if (DialogService.ShowModalWindow(argumentDetailsViewModel))
			{
				PropertyCopy.Copy(argumentDetailsViewModel.Argument, Argument);
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => ValueDescription);
			}
		}

		public RelayCommand<ExplicitValueViewModel> ChangeCommand { get; private set; }
		void OnChange(ExplicitValueViewModel explicitValueViewModel)
		{
			if (IsList)
				ProcedureHelper.SelectObject(ObjectType, explicitValueViewModel);
			else
				ProcedureHelper.SelectObject(ObjectType, ExplicitValue);
			OnPropertyChanged(() => ExplicitValues);
			OnPropertyChanged(() => ExplicitValue);
		}

		public RelayCommand EditStringCommand { get; private set; }
		void OnEditString()
		{
			var stringDetailsViewModel = new StringDetailsViewModel(ExplicitValue.StringValue);
			if (DialogService.ShowModalWindow(stringDetailsViewModel))
			{
				ExplicitValue.StringValue = stringDetailsViewModel.StringValue;
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => ValueDescription);
			}
		}

		List<ExplicitTypeViewModel> ExplicitTypes { get; set; }
		public void Update(List<Variable> allVariables, List<ExplicitType> explicitTypes = null, List<EnumType> enumTypes = null, List<ObjectType> objectTypes = null, bool? isList = null)
		{
			if (explicitTypes == null)
				explicitTypes = ProcedureHelper.GetEnumList<ExplicitType>();
			if (objectTypes == null)
				objectTypes = ProcedureHelper.GetEnumList<ObjectType>();
			if (enumTypes == null)
				enumTypes = ProcedureHelper.GetEnumList<EnumType>();
			ExplicitTypes = ProcedureHelper.BuildExplicitTypes(explicitTypes, enumTypes, objectTypes);
			var variables = ProcedureHelper.GetAllVariables(allVariables, explicitTypes, enumTypes, objectTypes, isList);
			if (isList != null)
				IsList = isList.Value;
			Variables = new List<VariableViewModel>();
			foreach (var variable in variables)
			{
				var variableViewModel = new VariableViewModel(variable);
				Variables.Add(variableViewModel);
			}
			SelectedVariable = Variables.FirstOrDefault(x => x.Variable.Uid == Argument.VariableUid);
			SelectedVariableScope = Argument.VariableScope;
			ExplicitValue.Initialize(ExplicitValue.UidValue);
			foreach (var explicitValue in ExplicitValues)
			{
				explicitValue.Initialize(explicitValue.UidValue);
			}
			if (explicitTypes != null)
				ExplicitType = explicitTypes.FirstOrDefault();
			if (enumTypes != null)
				EnumType = enumTypes.FirstOrDefault();
			if (objectTypes != null)
				ObjectType = objectTypes.FirstOrDefault();
			OnPropertyChanged(() => ExplicitValue);
			OnPropertyChanged(() => ExplicitValues);
			OnPropertyChanged(() => LocalVariables);
			OnPropertyChanged(() => GlobalVariables);
			OnPropertyChanged(() => AddVariableVisibility);
		}

		public void Update(List<Variable> variables, ExplicitType explicitType = ExplicitType.Integer, EnumType enumType = EnumType.DriverType, ObjectType objectType = ObjectType.Device, bool? isList = null)
		{
			Update(variables, new List<ExplicitType> { explicitType }, new List<EnumType> { enumType }, new List<ObjectType> { objectType }, isList);
		}

		public void Update(Procedure procedure, ExplicitType explicitType, EnumType? enumType = null, ObjectType? objectType = null, bool? isList = null)
		{
			var variables = ProcedureHelper.GetAllVariables(procedure);
			Update(variables, new List<ExplicitType> { explicitType }, enumType != null ? new List<EnumType> { enumType.Value } : null, objectType != null ? new List<ObjectType> { objectType.Value } : null, isList);
		}

		public void Update(Procedure procedure, List<ExplicitType> explicitTypes = null, List<EnumType> enumTypes = null, List<ObjectType> objectTypes = null, bool? isList = null)
		{
			var variables = ProcedureHelper.GetAllVariables(procedure);
			Update(variables, explicitTypes, enumTypes, objectTypes, isList);
		}

		public void Update()
		{
			OnPropertyChanged(() => VariableScopes);
		}

		public ObservableCollection<VariableScope> VariableScopes { get; set; }
		public VariableScope SelectedVariableScope
		{
			get { return Argument.VariableScope; }
			set
			{
				Argument.VariableScope = value;
				if (value == VariableScope.ExplicitValue)
					SelectedVariable = null;
				if (value == VariableScope.LocalVariable)
					SelectedVariable = LocalVariables.FirstOrDefault(x => x.Variable.Uid == Argument.VariableUid);
				if (value == VariableScope.GlobalVariable)
					SelectedVariable = GlobalVariables.FirstOrDefault(x => x.Variable.Uid == Argument.VariableUid);
				if (UpdateVariableScopeHandler != null)
					UpdateVariableScopeHandler();
				OnPropertyChanged(() => SelectedVariableScope);
				OnPropertyChanged(() => Description);
			}
		}

		List<VariableViewModel> Variables { get; set; }
		public ObservableCollection<VariableViewModel> LocalVariables
		{
			get { return new ObservableCollection<VariableViewModel>(Variables.FindAll(x => !x.Variable.IsGlobal)); }
		}

		public ObservableCollection<VariableViewModel> GlobalVariables
		{
			get { return new ObservableCollection<VariableViewModel>(Variables.FindAll(x => x.Variable.IsGlobal)); }
		}

		VariableViewModel _selectedVariable;
		public VariableViewModel SelectedVariable
		{
			get { return _selectedVariable; }
			set
			{
				_selectedVariable = value;
				if (_selectedVariable != null)
				{
					Argument.VariableUid = value.Variable.Uid;
					ExplicitType = _selectedVariable.Variable.ExplicitType;
					EnumType = _selectedVariable.Variable.EnumType;
					ObjectType = _selectedVariable.Variable.ObjectType;
					if (UpdateVariableHandler != null)
						UpdateVariableHandler();
				}
				else
				{
					Argument.VariableUid = Guid.Empty;
				}
				OnPropertyChanged(() => SelectedVariable);
			}
		}

		public string ValueDescription
		{
			get
			{
				var description = "";
				if (!IsList)
					description = ProcedureHelper.GetStringValue(Argument.ExplicitValue, Argument.ExplicitType, Argument.EnumType);
				else
				{
					if (Argument.ExplicitValues.Count == 0)
						return "Пустой список";
					foreach (var explicitValue in Argument.ExplicitValues)
					{
						description += ProcedureHelper.GetStringValue(explicitValue, Argument.ExplicitType, Argument.EnumType) + ", ";
					}
				}
				description = description.TrimEnd(',', ' ');
				return description;
			}
		}

		public string Description
		{
			get
			{
				if (SelectedVariableScope != VariableScope.ExplicitValue)
				{
					if ((SelectedVariable == null) || (SelectedVariable.Variable.IsGlobal && SelectedVariableScope == VariableScope.LocalVariable)
						|| (!SelectedVariable.Variable.IsGlobal && SelectedVariableScope == VariableScope.GlobalVariable))
						return EmptyText;
					return "<" + SelectedVariable.Variable.Name + ">";
				}

				return !IsList ? ProcedureHelper.GetStringValue(ExplicitValue.ExplicitValue, ExplicitType, EnumType) : "Список";
			}
		}

		public new void OnPropertyChanged<T>(Expression<Func<T>> propertyExpression)
		{
			ServiceFactory.SaveService.AutomationChanged = true;
			base.OnPropertyChanged(propertyExpression);
			if (UpdateDescriptionHandler != null)
				UpdateDescriptionHandler();
		}
	}
}