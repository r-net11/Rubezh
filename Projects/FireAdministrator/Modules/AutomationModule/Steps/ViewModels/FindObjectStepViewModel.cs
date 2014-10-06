using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;
using System.Linq.Expressions;

namespace AutomationModule.ViewModels
{
	public class FindObjectStepViewModel : BaseStepViewModel
	{
		FindObjectArguments FindObjectArguments { get; set; }
		public ObservableCollection<FindObjectConditionViewModel> FindObjectConditions { get; private set; }
		public ArgumentViewModel ResultArgument { get; private set; }

		public FindObjectStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			FindObjectArguments = stepViewModel.Step.FindObjectArguments;
			ResultArgument = new ArgumentViewModel(FindObjectArguments.ResultArgument, stepViewModel.Update, false);
			ResultArgument.ExplicitType = ExplicitType.Object;
			ResultArgument.UpdateVariableHandler = UpdateConditions;
			JoinOperator = FindObjectArguments.JoinOperator;
			FindObjectConditions = new ObservableCollection<FindObjectConditionViewModel>();
			FindObjectConditionViewModel.Properties = new ObservableCollection<Property>(ProcedureHelper.ObjectTypeToProperiesList(ResultArgument.ObjectType));
			foreach (var findObjectCondition in FindObjectArguments.FindObjectConditions)
			{
				var findObjectConditionViewModel = new FindObjectConditionViewModel(findObjectCondition, Procedure, UpdateDescriptionHandler);
				FindObjectConditions.Add(findObjectConditionViewModel);
			}
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
			OnPropertyChanged(() => IsJoinOperatorVisible);
		}

		public RelayCommand<FindObjectConditionViewModel> RemoveCommand { get; private set; }
		void OnRemove(FindObjectConditionViewModel findObjectConditionViewModel)
		{
			FindObjectConditions.Remove(findObjectConditionViewModel);
			FindObjectArguments.FindObjectConditions.Remove(findObjectConditionViewModel.FindObjectCondition);
			OnPropertyChanged(() => FindObjectConditions);
			OnPropertyChanged(() => IsJoinOperatorVisible);
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
			var allVariables = ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ExplicitType == ExplicitType.Object && x.IsList);
			variableUidValidator = ResultArgument.Argument.VariableUid;
			ResultArgument.Update(allVariables);
			OnPropertyChanged(() => IsJoinOperatorVisible);
		}

		public override string Description
		{
			get 
			{

				var conditionViewModel = FindObjectConditions.FirstOrDefault();
				if (conditionViewModel == null)
					return "Результат: " + ResultArgument.Description + " Условие поиска: <пусто>";

				var var2 = conditionViewModel.SourceArgument.Description;
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
				return "Результат: " + ResultArgument.Description + " Условие поиска: " + conditionViewModel.SelectedProperty.ToDescription() + " " + op + " " + var2 + " " + end;
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

		Guid variableUidValidator;
		void UpdateConditions()
		{
			if (ResultArgument.Argument.VariableUid != variableUidValidator)
			{
				variableUidValidator = ResultArgument.Argument.VariableUid;
				FindObjectConditions = new ObservableCollection<FindObjectConditionViewModel>();
				FindObjectArguments.FindObjectConditions = new List<FindObjectCondition>();
				FindObjectConditionViewModel.Properties = new ObservableCollection<Property>(ProcedureHelper.ObjectTypeToProperiesList(ResultArgument.ObjectType));
				OnPropertyChanged(() => FindObjectConditions);
			}
		}
	}

	public class FindObjectConditionViewModel : BaseViewModel
	{
		public FindObjectCondition FindObjectCondition { get; private set; }
		public ArgumentViewModel SourceArgument { get; private set; }
		Procedure Procedure { get; set; }
		Action UpdateDescriptionHandler { get; set; }

		public FindObjectConditionViewModel(FindObjectCondition findObjectCondition, Procedure procedure, Action updateDescriptionHandler)
		{
			UpdateDescriptionHandler = updateDescriptionHandler;
			FindObjectCondition = findObjectCondition;
			Procedure = procedure;
			SourceArgument = new ArgumentViewModel(findObjectCondition.SourceArgument, updateDescriptionHandler);
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
				SourceArgument.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => !x.IsList && x.ExplicitType == ExplicitType && x.EnumType == EnumType));
				SourceArgument.ExplicitType = ExplicitType;
				SourceArgument.EnumType = EnumType;
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
