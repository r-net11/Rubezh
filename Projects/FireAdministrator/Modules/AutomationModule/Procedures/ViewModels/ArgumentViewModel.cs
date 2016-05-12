using Infrastructure;
using Infrastructure.Automation;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.Automation;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

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
		public Argument Argument { get; private set; }

		public ArgumentViewModel(Argument argument, Action updateDescriptionHandler, Action updateContentHandler, bool allowExplicitValue = true, bool allowLocalValue = true, bool allowGlobalValue = true)
		{
			AddVariableCommand = new RelayCommand(OnAddVariable);

			Argument = argument;
			UpdateDescriptionHandler = updateDescriptionHandler;
			UpdateContentHandler = updateContentHandler;
			ExplicitValue = new ExplicitValueViewModel((ExplicitValue)argument, true);
			ExplicitValue.UpdateObjectHandler += () => OnPropertyChanged(() => IsEmpty);
			ExplicitValue.UpdateDescriptionHandler = updateDescriptionHandler;
			Variables = new List<VariableViewModel>();
			VariableScopes = new ObservableCollection<VariableScope>(AutomationHelper.GetEnumList<VariableScope>().FindAll(x => (allowExplicitValue || x != VariableScope.ExplicitValue) && (allowLocalValue || x != VariableScope.LocalVariable) && (allowGlobalValue || x != VariableScope.GlobalVariable)));
			OnPropertyChanged(() => VariableScopes);
			ExplicitTypes = new List<ExplicitTypeViewModel>();
		}

		public ExplicitType ExplicitType
		{
			get { return Argument.ExplicitType; }
			set
			{
				if (Argument.ExplicitType != value)
				{
					ExplicitValue.ExplicitType = value;
					OnPropertyChanged(() => ExplicitType);
				}
			}
		}

		public EnumType EnumType
		{
			get { return Argument.EnumType; }
			set
			{
				ExplicitValue.EnumType = value;
				OnPropertyChanged(() => EnumType);
			}
		}

		public ObjectType ObjectType
		{
			get { return Argument.ObjectType; }
			set
			{
				if (Argument.ObjectType != value)
				{
					ExplicitValue.ObjectType = value;
					OnPropertyChanged(() => ObjectType);
					OnPropertyChanged(() => IsEmpty);
				}
			}
		}

		public bool IsEmpty
		{
			get
			{
				return (
						   ((ObjectType == ObjectType.GKDoor) && (ExplicitValue.GKDoor == null))
						|| ((ObjectType == ObjectType.Device) && (ExplicitValue.Device == null))
						|| ((ObjectType == ObjectType.Delay) && (ExplicitValue.Delay == null))
						|| ((ObjectType == ObjectType.Direction) && (ExplicitValue.Direction == null))
						|| ((ObjectType == ObjectType.GuardZone) && (ExplicitValue.GuardZone == null))
						|| ((ObjectType == ObjectType.VideoDevice) && (ExplicitValue.Camera == null))
						|| ((ObjectType == ObjectType.Zone) && (ExplicitValue.Zone == null))
						|| ((ObjectType == ObjectType.Delay) && (ExplicitValue.Delay == null))
						|| ((ObjectType == ObjectType.PumpStation) && (ExplicitValue.PumpStation == null))
						|| ((ObjectType == ObjectType.MPT) && (ExplicitValue.MPT == null))
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
			var variableDetailsViewModel = new VariableDetailsViewModel(null,
				SelectedVariableScope == VariableScope.LocalVariable ? AutomationHelper.GetLocalVariables(ProceduresViewModel.Current.SelectedProcedure.Procedure) : ClientManager.SystemConfiguration.AutomationConfiguration.GlobalVariables,
				SelectedVariableScope == VariableScope.LocalVariable ? "Добавить локальную переменную" : "Добавить глобальную переменную",
				SelectedVariableScope == VariableScope.GlobalVariable);
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
					ClientManager.SystemConfiguration.AutomationConfiguration.GlobalVariables.Add(variableDetailsViewModel.Variable);
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

		List<ExplicitTypeViewModel> ExplicitTypes { get; set; }
		public void Update(List<Variable> allVariables, List<ExplicitType> explicitTypes = null, List<EnumType> enumTypes = null, List<ObjectType> objectTypes = null, bool? isList = null)
		{
			if (explicitTypes == null)
				explicitTypes = AutomationHelper.GetEnumList<ExplicitType>();
			if (objectTypes == null)
				objectTypes = AutomationHelper.GetEnumList<ObjectType>();
			if (enumTypes == null)
				enumTypes = AutomationHelper.GetEnumList<EnumType>();
			ExplicitTypes = ProcedureHelper.BuildExplicitTypes(explicitTypes, enumTypes, objectTypes);

			if (ExplicitTypes.Count == 1)
			{
				ExplicitType = ExplicitTypes[0].ExplicitType;
				ExplicitValue.UpdateObjectHandler();
			}

			var variables = AutomationHelper.GetAllVariables(allVariables, explicitTypes, enumTypes, objectTypes, isList);
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
			if (explicitTypes != null)
				ExplicitType = explicitTypes.FirstOrDefault();
			if (enumTypes != null)
				EnumType = enumTypes.FirstOrDefault();
			if (objectTypes != null)
				ObjectType = objectTypes.FirstOrDefault();

			OnPropertyChanged(() => ExplicitValue);
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
			var variables = AutomationHelper.GetAllVariables(procedure);
			Update(variables, new List<ExplicitType> { explicitType }, enumType != null ? new List<EnumType> { enumType.Value } : null, objectType != null ? new List<ObjectType> { objectType.Value } : null, isList);
		}

		public void Update(Procedure procedure, List<ExplicitType> explicitTypes = null, List<EnumType> enumTypes = null, List<ObjectType> objectTypes = null, bool? isList = null)
		{
			var variables = AutomationHelper.GetAllVariables(procedure);
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
				if (Argument.VariableScope != value)
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
				}
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
				if (_selectedVariable != value)
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
					OnPropertyChanged(() => Description);
				}
			}
		}

		public string ValueDescription
		{
			get
			{
				var description = Argument.ToString();
				return string.IsNullOrEmpty(description) ? "Пустой список" : description;
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

				return IsList ?
					"Список" :
					ExplicitValue.ExplicitValue == null ? "" : ExplicitValue.ExplicitValue.ToString();
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