using FiresecAPI.Automation;
using FiresecAPI.Models.Automation;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using Localization.Automation;
using Localization.Automation.ViewModels;

namespace AutomationModule.ViewModels
{
	/// <summary>
	/// Добавляет значение параметров в функцию автоматизацию
	/// </summary>
	public class ArgumentViewModel : BaseViewModel
	{
		public static string EmptyText = CommonResources.Empty;
		#region Properties
		public Action UpdateVariableScopeHandler { get; set; }
		public Action UpdateVariableHandler { get; set; }
		Action UpdateContentHandler { get; set; }
		Action UpdateDescriptionHandler { get; set; }
		public ExplicitValueViewModel ExplicitValue { get; protected set; }
		public ObservableCollection<ExplicitValueViewModel> ExplicitValues { get; set; }
		public Argument Argument { get; private set; }

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
						|| ((ObjectType == ObjectType.SKDDevice) && (ExplicitValue.SKDDevice == null))
						|| ((ObjectType == ObjectType.SKDZone) && (ExplicitValue.SKDZone == null))
						|| ((ObjectType == ObjectType.VideoDevice) && (ExplicitValue.Camera == null))
						|| ((ObjectType == ObjectType.Organisation) && (ExplicitValue.Organisation == null))
						|| ((ObjectType == ObjectType.User) && (ExplicitValue.User == null))
						|| ((ObjectType == ObjectType.Employee) && (ExplicitValue.Employee == null))
						|| ((ObjectType == ObjectType.Visitor) && (ExplicitValue.Visitor == null)));
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

		public bool AddVariableVisibility
		{
			get
			{
				return (ExplicitTypes != null && ExplicitTypes.Any());
			}
		}

		List<VariableViewModel> Variables { get; set; }
		public ObservableCollection<VariableViewModel> LocalVariables
		{
			get { return new ObservableCollection<VariableViewModel>(Variables.FindAll(x => x.Variable is LocalVariable)); }
		}

		public ObservableCollection<VariableViewModel> GlobalVariables
		{
			get { return new ObservableCollection<VariableViewModel>(Variables.FindAll(x => x.Variable is GlobalVariable)); }
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
					Argument.VariableUid = value.Variable.UID;
					ExplicitType = _selectedVariable.Variable.VariableValue.ExplicitType;
					EnumType = _selectedVariable.Variable.VariableValue.EnumType;
					ObjectType = _selectedVariable.Variable.VariableValue.ObjectType;
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
				var description = ProcedureHelper.GetStringValue(Argument.ExplicitValue, Argument.ExplicitType, Argument.EnumType);
				description = description.TrimEnd(',', ' ');
				return description;
			}
		}

		public string Description
		{
			get
			{
				if (SelectedVariableScope == VariableScope.ExplicitValue)
					return ProcedureHelper.GetStringValue(ExplicitValue.ExplicitValue, ExplicitType, EnumType);

				if ((SelectedVariable == null) || (SelectedVariable.Variable is GlobalVariable && SelectedVariableScope == VariableScope.LocalVariable)
					|| (SelectedVariable.Variable is LocalVariable && SelectedVariableScope == VariableScope.GlobalVariable))
					return EmptyText;

				return "<" + SelectedVariable.Variable.Name + ">";
			}
		}
		#endregion

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

		public RelayCommand AddVariableCommand { get; private set; }
		void OnAddVariable()
		{
			if (SelectedVariableScope == VariableScope.GlobalVariable)
				AddGlobalVariable();
			else
				AddLocalVariable();
		}

		private void AddGlobalVariable()
		{
			var variableDetailsViewModel = new AddGlobalVariableDialogViewModel(null);
			var explicitTypeViewModel = variableDetailsViewModel.ExplicitTypes.FirstOrDefault();

			if (explicitTypeViewModel != null)
				variableDetailsViewModel.SelectedExplicitType = explicitTypeViewModel.GetAllChildren().FirstOrDefault(x => x.IsRealType);

			if (variableDetailsViewModel.SelectedExplicitType != null)
				variableDetailsViewModel.SelectedExplicitType.ExpandToThis();

			if (!DialogService.ShowModalWindow(variableDetailsViewModel)) return;

			FiresecManager.FiresecService.SaveGlobalVariable(variableDetailsViewModel.Variable);
			GlobalVariablesViewModel.Current.GlobalVariables.Add(new VariableViewModel(variableDetailsViewModel.Variable));

			SelectedVariable = new VariableViewModel(variableDetailsViewModel.Variable);
			Variables.Add(SelectedVariable);

			OnPropertyChanged(() => GlobalVariables);
			if (UpdateContentHandler != null)
				UpdateContentHandler();
		}

		private void AddLocalVariable()
		{
			string defaultName = argumentViewModel.DefaultName;
            string title = argumentViewModel.Title;

			var variableDetailsViewModel = new VariableDetailsViewModel(null, defaultName, title)
			{
				ExplicitTypes = new ObservableCollection<ExplicitTypeViewModel>(ExplicitTypes)
			};
			var explicitTypeViewModel = variableDetailsViewModel.ExplicitTypes.FirstOrDefault();

			if (explicitTypeViewModel != null)
				variableDetailsViewModel.SelectedExplicitType = explicitTypeViewModel.GetAllChildren().FirstOrDefault(x => x.IsRealType);

			if (variableDetailsViewModel.SelectedExplicitType != null)
				variableDetailsViewModel.SelectedExplicitType.ExpandToThis();

			if (!DialogService.ShowModalWindow(variableDetailsViewModel)) return;

			ProceduresViewModel.Current.SelectedProcedure.VariablesViewModel.Variables.Add(new VariableViewModel(variableDetailsViewModel.Variable));
			ProceduresViewModel.Current.SelectedProcedure.Procedure.Variables.Add(variableDetailsViewModel.Variable);

			SelectedVariable = new VariableViewModel(variableDetailsViewModel.Variable);
			Variables.Add(SelectedVariable);

			ServiceFactory.SaveService.AutomationChanged = true;
			OnPropertyChanged(() => LocalVariables);

			if (UpdateContentHandler != null)
				UpdateContentHandler();
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
			var argumentDetailsViewModel = new ArgumentDetailsViewModel(Argument);
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
		public void Update(List<IVariable> allVariables, List<ExplicitType> explicitTypes = null, List<EnumType> enumTypes = null, List<ObjectType> objectTypes = null)
		{
			if (explicitTypes == null)
				explicitTypes = ProcedureHelper.GetEnumList<ExplicitType>();
			if (objectTypes == null)
				objectTypes = ProcedureHelper.GetEnumList<ObjectType>();
			if (enumTypes == null)
				enumTypes = ProcedureHelper.GetEnumList<EnumType>();
			ExplicitTypes = ProcedureHelper.BuildExplicitTypes(explicitTypes, enumTypes, objectTypes);
			var variables = ProcedureHelper.GetAllVariables(allVariables, explicitTypes, enumTypes, objectTypes);

			Variables = new List<VariableViewModel>();
			foreach (var variable in variables)
			{
				var variableViewModel = new VariableViewModel(variable);
				Variables.Add(variableViewModel);
			}
			SelectedVariable = Variables.FirstOrDefault(x => x.Variable.UID == Argument.VariableUid);
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

		public void Update(List<IVariable> variables, ExplicitType explicitType = ExplicitType.Integer, EnumType enumType = EnumType.DriverType, ObjectType objectType = ObjectType.SKDDevice)
		{
			Update(variables, new List<ExplicitType> { explicitType }, new List<EnumType> { enumType }, new List<ObjectType> { objectType });
		}

		public void Update(Procedure procedure, ExplicitType explicitType, EnumType? enumType = null, ObjectType? objectType = null)
		{
			var variables = ProcedureHelper.GetAllVariables(procedure);
			Update(variables, new List<ExplicitType> { explicitType }, enumType != null ? new List<EnumType> { enumType.Value } : null, objectType != null ? new List<ObjectType> { objectType.Value } : null);
		}

		public void Update(Procedure procedure, List<ExplicitType> explicitTypes = null, List<EnumType> enumTypes = null, List<ObjectType> objectTypes = null)
		{
			var variables = ProcedureHelper.GetAllVariables(procedure);
			Update(variables, explicitTypes, enumTypes, objectTypes);
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
					SelectedVariable = LocalVariables.FirstOrDefault(x => x.Variable.UID == Argument.VariableUid);
				if (value == VariableScope.GlobalVariable)
					SelectedVariable = GlobalVariables.FirstOrDefault(x => x.Variable.UID == Argument.VariableUid);
				if (UpdateVariableScopeHandler != null)
					UpdateVariableScopeHandler();
				OnPropertyChanged(() => SelectedVariableScope);
				OnPropertyChanged(() => Description);
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