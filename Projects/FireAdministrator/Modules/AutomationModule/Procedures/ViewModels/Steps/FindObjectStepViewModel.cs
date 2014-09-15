using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using FiresecAPI.Automation;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.GK;
using System.Reflection.Emit;
using FiresecAPI;
using System.Linq.Expressions;

namespace AutomationModule.ViewModels
{
	public class FindObjectStepViewModel : BaseStepViewModel
	{
		FindObjectArguments FindObjectArguments { get; set; }
		public ObservableCollection<FindObjectConditionViewModel> FindObjectConditions { get; private set; }
		Action UpdateDescriptionHandler { get; set; }

		public FindObjectStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			FindObjectArguments = stepViewModel.Step.FindObjectArguments;
			UpdateDescriptionHandler = stepViewModel.Update;
			UpdateContent();
			AddCommand = new RelayCommand(OnAdd);
			RemoveCommand = new RelayCommand<FindObjectConditionViewModel>(OnRemove);
			ChangeJoinOperatorCommand = new RelayCommand(OnChangeJoinOperator);
		}

		public RelayCommand AddCommand { get; private set; }
		public void OnAdd()
		{
			var findObjectCondition = new FindObjectCondition();
			var findObjectConditionViewModel = new FindObjectConditionViewModel(findObjectCondition, Procedure, UpdateDescriptionHandler);
			FindObjectArguments.FindObjectConditions.Add(findObjectCondition);
			FindObjectConditions.Add(findObjectConditionViewModel);			
			OnPropertyChanged(() => FindObjectConditions);
			UpdateContent();
		}

		public RelayCommand<FindObjectConditionViewModel> RemoveCommand { get; private set; }
		void OnRemove(FindObjectConditionViewModel findObjectConditionViewModel)
		{
			FindObjectConditions.Remove(findObjectConditionViewModel);
			FindObjectArguments.FindObjectConditions.Remove(findObjectConditionViewModel.FindObjectCondition);
			OnPropertyChanged(() => FindObjectConditions);
			UpdateContent();
		}

		public RelayCommand ChangeJoinOperatorCommand { get; private set; }
		void OnChangeJoinOperator()
		{
			JoinOperator = JoinOperator == JoinOperator.And ? JoinOperator.Or : JoinOperator.And;
		}

		public bool IsJoinOperatorVisible
		{
			get { return FindObjectConditions.Count > 1; }
		}

		public override void UpdateContent()
		{
			var allVariables = ProcedureHelper.GetAllVariables(Procedure);
			Variables = new ObservableCollection<VariableViewModel>();
			foreach (var variable in allVariables.FindAll(x => ((x.ExplicitType == ExplicitType.Object) && (x.IsList))))
			{
				var variableViewModel = new VariableViewModel(variable);
				Variables.Add(variableViewModel);
			}
			SelectedVariable = Variables.FirstOrDefault(x => x.Variable.Uid == FindObjectArguments.ResultUid);
			JoinOperator = FindObjectArguments.JoinOperator;
			FindObjectConditions = new ObservableCollection<FindObjectConditionViewModel>();
			if (SelectedVariable != null)
				foreach (var findObjectCondition in FindObjectArguments.FindObjectConditions)
				{
					var findObjectConditionViewModel = new FindObjectConditionViewModel(findObjectCondition, Procedure, UpdateDescriptionHandler);
					FindObjectConditions.Add(findObjectConditionViewModel);
				}
			else
				FindObjectArguments.FindObjectConditions = new List<FindObjectCondition>();
			OnPropertyChanged(() => Variables);
			OnPropertyChanged(() => SelectedVariable);
			OnPropertyChanged(() => FindObjectConditions);
			OnPropertyChanged(() => IsJoinOperatorVisible);
		}

		public override string Description
		{
			get 
			{

				var result = SelectedVariable != null ? "<" + SelectedVariable.Name + ">": "<пусто>";
				var conditionViewModel = FindObjectConditions.FirstOrDefault();
				if (conditionViewModel == null)
					return "Результат: " + result + " Условие поиска: <пусто>";

				var var2 = conditionViewModel.Variable2.Description;
				var op = "";
				switch (conditionViewModel.SelectedConditionType)
				{
					case ConditionType.IsEqual:
						op = "==";
						break;
					case ConditionType.IsLess:
						op = "<";
						break;
					case ConditionType.IsMore:
						op = ">";
						break;
					case ConditionType.IsNotEqual:
						op = "!=";
						break;
					case ConditionType.IsNotLess:
						op = "≥";
						break;
					case ConditionType.IsNotMore:
						op = "≤";
						break;
				}
				var end = "";
				if (FindObjectConditions.Count > 1)
					end = JoinOperator == JoinOperator.And ? "и ..." : "или ...";

				return "Результат: " + result + " Условие поиска: " + conditionViewModel.SelectedProperty.ToDescription() + " " + op + " " + var2 + " " + end;
			}
		}

		public JoinOperator JoinOperator
		{
			get { return FindObjectArguments.JoinOperator; }
			set
			{
				FindObjectArguments.JoinOperator = value;
				OnPropertyChanged(() => JoinOperator);
			}
		}

		public ObservableCollection<VariableViewModel> Variables { get; private set; }
		VariableViewModel _selectedVariable;
		public VariableViewModel SelectedVariable
		{
			get { return _selectedVariable; }
			set
			{
				_selectedVariable = value;
				if (value != null)
				{
					if (FindObjectArguments.ResultUid != value.Variable.Uid)
					{
						FindObjectConditions = new ObservableCollection<FindObjectConditionViewModel>();
						FindObjectArguments.FindObjectConditions = new List<FindObjectCondition>();
						OnPropertyChanged(() => FindObjectConditions);
					}
					FindObjectConditionViewModel.Properties = new ObservableCollection<Property>(ProcedureHelper.ObjectTypeToProperiesList(value.Variable.ObjectType));
					FindObjectArguments.ResultUid = value.Variable.Uid;
				}
				OnPropertyChanged(() => SelectedVariable);
			}
		}
	}

	public class FindObjectConditionViewModel : BaseViewModel
	{
		public FindObjectCondition FindObjectCondition { get; private set; }
		public ArithmeticParameterViewModel Variable2 { get; private set; }
		Procedure Procedure { get; set; }
		Action UpdateDescriptionHandler { get; set; }

		public FindObjectConditionViewModel(FindObjectCondition findObjectCondition, Procedure procedure, Action updateDescriptionHandler)
		{
			UpdateDescriptionHandler = updateDescriptionHandler;
			FindObjectCondition = findObjectCondition;
			Procedure = procedure;
			Variable2 = new ArithmeticParameterViewModel(findObjectCondition.Variable2, updateDescriptionHandler);
			SelectedProperty = FindObjectCondition.Property;			
			SelectedConditionType = FindObjectCondition.ConditionType;
		}

		public static ObservableCollection<Property> Properties { get; set; }
		public Property SelectedProperty
		{
			get { return FindObjectCondition.Property; }
			set
			{							
				FindObjectCondition.Property = value;
				if (value == Property.IntAddress)
				{ MinValue = 1; MaxValue = 255; }
				if (value == Property.ShleifNo)
				{ MinValue = 1; MaxValue = 8; }
				ConditionTypes = new ObservableCollection<ConditionType>(ProcedureHelper.ObjectTypeToConditionTypesList(ExplicitType));
				Variable2.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => !x.IsList && x.ExplicitType == ExplicitType && x.EnumType == EnumType));
				Variable2.ExplicitType = ExplicitType;
				Variable2.EnumType = EnumType;
				OnPropertyChanged(() => SelectedProperty);
				OnPropertyChanged(() => ConditionTypes);
				OnPropertyChanged(() => MinValue);
				OnPropertyChanged(() => MaxValue);
			}
		}

		public ObservableCollection<ConditionType> ConditionTypes { get; private set; }
		public ConditionType SelectedConditionType
		{
			get { return FindObjectCondition.ConditionType; }
			set
			{
				FindObjectCondition.ConditionType = value;
				OnPropertyChanged(() => SelectedConditionType);
			}
		}

		int _minValue;
		public int MinValue
		{
			get { return _minValue; }
			set
			{
				_minValue = value;
				OnPropertyChanged(() => MinValue);
			}
		}

		int _maxValue;
		public int MaxValue
		{ 
			get { return _maxValue; }
			set
			{
				_maxValue = value;
				OnPropertyChanged(() => MaxValue);
			}
		}

		public EnumType EnumType
		{
			get
			{
				if (SelectedProperty == Property.Type)
					return EnumType.DriverType;
				if (SelectedProperty == Property.State)
					return EnumType.StateType;
				return EnumType.StateType;
			}
		}

		public ExplicitType ExplicitType
		{
			get
			{
				if (SelectedProperty == Property.Description)
					return ExplicitType.String;
				if ((SelectedProperty == Property.Type)||(SelectedProperty == Property.State))
					return ExplicitType.Enum;
				return ExplicitType.Integer;
			}
		}

		public new void OnPropertyChanged<T>(Expression<Func<T>> propertyExpression)
		{
			base.OnPropertyChanged(propertyExpression);
			ServiceFactory.SaveService.AutomationChanged = true;
			if (UpdateDescriptionHandler != null)
				UpdateDescriptionHandler();
		}
	}
}
