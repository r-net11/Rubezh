using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using System.Linq.Expressions;
using FiresecAPI;
using FiresecAPI.GK;
using Infrastructure.Common;
using Infrastructure.Common.Windows;

namespace AutomationModule.ViewModels
{
	public class ArgumentViewModel : VariableViewModel
	{
		public Action UpdateVariableScopeHandler { get; set; }
		public Action UpdateVariableHandler { get; set; }
		Action UpdateDescriptionHandler { get; set; }

		public ArgumentViewModel(Variable argument, Action updateDescriptionHandler, bool allowExplicitValue = true, bool allowLocalValue = true) : base (argument)
		{
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
		}

		public new RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var explicitValueViewModel = new ExplicitValueViewModel(new ExplicitValue());
			if (ExplicitType == ExplicitType.Object)
				ProcedureHelper.SelectObject(SelectedObjectType, explicitValueViewModel);
			ExplicitValues.Add(explicitValueViewModel);
			Variable.ExplicitValues.Add(explicitValueViewModel.ExplicitValue);
			OnPropertyChanged(() => ExplicitValues);
		}

		public new RelayCommand<ExplicitValueViewModel> RemoveCommand { get; private set; }
		void OnRemove(ExplicitValueViewModel explicitValueViewModel)
		{
			ExplicitValues.Remove(explicitValueViewModel);
			Variable.ExplicitValues.Remove(explicitValueViewModel.ExplicitValue);
			OnPropertyChanged(() => ExplicitValues);
		}

		public new RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var variableDetailsViewModel = new ArgumentDetailsViewModel(Variable);
			if (DialogService.ShowModalWindow(variableDetailsViewModel))
			{
				PropertyCopy.Copy<Variable, Variable>(variableDetailsViewModel.Variable, Variable);
				ServiceFactory.SaveService.AutomationChanged = true;
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
			SelectedVariable = Variables.FirstOrDefault(x => x.Variable.Uid == Variable.VariableUid);
			SelectedVariableScope = Variable.VariableScope;
			OnPropertyChanged(() => LocalVariables);
			OnPropertyChanged(() => GlobalVariables);
		}

		public ObservableCollection<VariableScope> VariableScopes { get; set; }
		public VariableScope SelectedVariableScope
		{
			get { return Variable.VariableScope; }
			set
			{
				Variable.VariableScope = value;
				if (value == VariableScope.ExplicitValue)
					SelectedVariable = null;
				if (value == VariableScope.LocalVariable)
					SelectedVariable = LocalVariables.FirstOrDefault(x => x.Variable.Uid == Variable.VariableUid);
				if (value == VariableScope.GlobalVariable)
					SelectedVariable = GlobalVariables.FirstOrDefault(x => x.Variable.Uid == Variable.VariableUid);
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
					Variable.VariableUid = value.Variable.Uid;
					SelectedEnumType = value.SelectedEnumType;
					SelectedObjectType = value.SelectedObjectType;
					if (UpdateVariableHandler != null)
						UpdateVariableHandler();
				}
				else
				{
					Variable.VariableUid = Guid.Empty;
				}
				OnPropertyChanged(() => SelectedVariable);
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
						return "<" + SelectedVariable.Name + ">";
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
								if (SelectedEnumType == EnumType.StateType)
									description = ExplicitValue.StateTypeValue.ToDescription();
								if (SelectedEnumType == EnumType.DriverType)
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