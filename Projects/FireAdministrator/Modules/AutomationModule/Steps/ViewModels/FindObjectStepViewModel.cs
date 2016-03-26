using Infrastructure;
using Infrastructure.Automation;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI;
using RubezhAPI.Automation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace AutomationModule.ViewModels
{
	public class FindObjectStepViewModel : BaseStepViewModel
	{
		FindObjectStep FindObjectStep { get; set; }
		public ObservableCollection<FindObjectConditionViewModel> FindObjectConditions { get; private set; }
		public ArgumentViewModel ResultArgument { get; private set; }

		public FindObjectStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			FindObjectStep = (FindObjectStep)stepViewModel.Step;
			ResultArgument = new ArgumentViewModel(FindObjectStep.ResultArgument, stepViewModel.Update, UpdateContent, false);
			ResultArgument.UpdateVariableHandler = UpdateConditions;
			JoinOperator = FindObjectStep.JoinOperator;
			FindObjectConditions = new ObservableCollection<FindObjectConditionViewModel>();
			FindObjectConditionViewModel.Properties = new ObservableCollection<Property>(AutomationHelper.ObjectTypeToProperiesList(ResultArgument.ObjectType));
			foreach (var findObjectCondition in FindObjectStep.FindObjectConditions)
			{
				var findObjectConditionViewModel = new FindObjectConditionViewModel(findObjectCondition, Procedure, UpdateDescriptionHandler, UpdateContent);
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
			var findObjectConditionViewModel = new FindObjectConditionViewModel(findObjectCondition, Procedure, UpdateDescriptionHandler, UpdateContent);
			FindObjectStep.FindObjectConditions.Add(findObjectCondition);
			FindObjectConditions.Add(findObjectConditionViewModel);
			OnPropertyChanged(() => FindObjectConditions);
			OnPropertyChanged(() => IsJoinOperatorVisible);
		}

		public RelayCommand<FindObjectConditionViewModel> RemoveCommand { get; private set; }
		void OnRemove(FindObjectConditionViewModel findObjectConditionViewModel)
		{
			FindObjectConditions.Remove(findObjectConditionViewModel);
			FindObjectStep.FindObjectConditions.Remove(findObjectConditionViewModel.FindObjectCondition);
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
			variableUidValidator = ResultArgument.Argument.VariableUid;
			ResultArgument.Update(Procedure, ExplicitType.Object, isList: true);
			foreach (var findObjectCondition in FindObjectConditions)
			{
				findObjectCondition.UpdateContent();
			}
			UpdateConditions();
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
			get { return FindObjectStep.JoinOperator; }
			set
			{
				FindObjectStep.JoinOperator = value;
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
				FindObjectStep.FindObjectConditions = new List<FindObjectCondition>();
				FindObjectConditionViewModel.Properties = new ObservableCollection<Property>(AutomationHelper.ObjectTypeToProperiesList(ResultArgument.ObjectType));
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

		public FindObjectConditionViewModel(FindObjectCondition findObjectCondition, Procedure procedure, Action updateDescriptionHandler, Action updateContentHandler)
		{
			UpdateDescriptionHandler = updateDescriptionHandler;
			FindObjectCondition = findObjectCondition;
			Procedure = procedure;
			SourceArgument = new ArgumentViewModel(findObjectCondition.SourceArgument, updateDescriptionHandler, updateContentHandler);
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
				ConditionTypes = new ObservableCollection<ConditionType>(AutomationHelper.ObjectTypeToConditionTypesList(ExplicitType));
				SourceArgument.Update(Procedure, ExplicitType, EnumType, isList: false);
				OnPropertyChanged(() => SelectedProperty);
				OnPropertyChanged(() => ConditionTypes);
				OnPropertyChanged(() => MinValue);
				OnPropertyChanged(() => MaxValue);
			}
		}

		public void UpdateContent()
		{
			SourceArgument.Update(Procedure, ExplicitType, EnumType, isList: false);
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
				if (SelectedProperty == Property.Description || SelectedProperty == Property.Name || SelectedProperty == Property.Uid)
					return ExplicitType.String;
				if ((SelectedProperty == Property.Type) || (SelectedProperty == Property.State))
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
