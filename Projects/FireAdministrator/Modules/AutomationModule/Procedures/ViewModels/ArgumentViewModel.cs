using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using System.Linq.Expressions;
using FiresecAPI;
using Infrastructure.Common;
using Infrastructure.Common.Windows;

namespace AutomationModule.ViewModels
{
	public class ArgumentViewModel : BaseViewModel
	{
		public Action UpdateVariableScopeHandler { get; set; }
		public Action UpdateVariableHandler { get; set; }
		Action UpdateDescriptionHandler { get; set; }
		public ExplicitValueViewModel ExplicitValue { get; protected set; }
		public ObservableCollection<ExplicitValueViewModel> ExplicitValues { get; set; }
		public Argument Argument { get; private set; }

		public ArgumentViewModel(Argument argument, Action updateDescriptionHandler, bool allowExplicitValue = true, bool allowLocalValue = true)
		{
			Argument = argument;
			UpdateDescriptionHandler = updateDescriptionHandler;
			ExplicitValue = new ExplicitValueViewModel(argument.ExplicitValue);
			ExplicitValues = new ObservableCollection<ExplicitValueViewModel>();
			foreach (var explicitValue in argument.ExplicitValues)
				ExplicitValues.Add(new ExplicitValueViewModel(explicitValue));
			ExplicitValue.UpdateDescriptionHandler = updateDescriptionHandler;
			Variables = new List<VariableViewModel>();
			VariableScopes = new ObservableCollection<VariableScope>(ProcedureHelper.GetEnumList<VariableScope>().FindAll(x => (allowExplicitValue || x != VariableScope.ExplicitValue) && (allowLocalValue || x != VariableScope.LocalVariable)));
			OnPropertyChanged(() => VariableScopes);
			AddCommand = new RelayCommand(OnAdd);
			RemoveCommand = new RelayCommand<ExplicitValueViewModel>(OnRemove);
			EditCommand = new RelayCommand(OnEdit);
			ChangeCommand = new RelayCommand<ExplicitValueViewModel>(OnChange);
			EditStringCommand = new RelayCommand(OnEditString);
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
				PropertyCopy.Copy<Argument, Argument>(argumentDetailsViewModel.Argument, Argument);
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
				//PropertyCopy.Copy<Argument, Argument>(argumentDetailsViewModel.Argument, Argument);
				ExplicitValue.StringValue = stringDetailsViewModel.StringValue;
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => ValueDescription);
			}
		}

		public void Update(List<Variable> variables)
		{
			Variables = new List<VariableViewModel>();
			foreach (var variable in variables)
			{
				var variableViewModel = new VariableViewModel(variable);
				Variables.Add(variableViewModel);
			}
			SelectedVariable = Variables.FirstOrDefault(x => x.Variable.Uid == Argument.VariableUid);
			SelectedVariableScope = Argument.VariableScope;
			OnPropertyChanged(() => LocalVariables);
			OnPropertyChanged(() => GlobalVariables);
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
						return "<пусто>";
					else
						return "<" + SelectedVariable.Variable.Name + ">";
				}
								
				var description = "";
				if (!IsList)
				{
					switch (ExplicitType)
					{
						case ExplicitType.Boolean:
							description = ExplicitValue.BoolValue.ToString();
							break;
						case ExplicitType.DateTime:
							description = ExplicitValue.DateTimeValue.ToString();
							break;
						case ExplicitType.Integer:
							description = ExplicitValue.IntValue.ToString();
							break;
						case ExplicitType.String:
							description = ExplicitValue.StringValue.ToString();
							break;
						case ExplicitType.Enum:
							{
								if (EnumType == EnumType.StateType)
									description = ExplicitValue.StateTypeValue.ToDescription();
								if (EnumType == EnumType.DriverType)
									description = ExplicitValue.DriverTypeValue.ToDescription();
							}
							break;
						case ExplicitType.Object:
							{
								description = ExplicitValue.PresentationName;
							}
							break;
					}
					return description;
				}
				else return "Список";
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