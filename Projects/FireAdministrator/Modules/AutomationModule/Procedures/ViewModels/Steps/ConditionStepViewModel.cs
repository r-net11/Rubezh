using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.Generic;
using FiresecClient;
using ValueType = FiresecAPI.Automation.ValueType;
using System.Linq.Expressions;

namespace AutomationModule.ViewModels
{
	public class ConditionStepViewModel : BaseViewModel, IStepViewModel
	{
		public ConditionArguments ConditionArguments { get; private set; }
		public ObservableCollection<ConditionViewModel> Conditions { get; private set; }
		Procedure Procedure { get; set; }
		public Action UpdateDescriptionHandler { get; set; }

		public ConditionStepViewModel(ConditionArguments conditionArguments, Procedure procedure, Action updateDescriptionHandler)
		{
			UpdateDescriptionHandler = updateDescriptionHandler;
			ConditionArguments = conditionArguments;
			Procedure = procedure;
			Conditions = new ObservableCollection<ConditionViewModel>();
			conditionArguments.Conditions.ForEach (condition => Conditions.Add(new ConditionViewModel(condition, procedure, updateDescriptionHandler)));
			if (Conditions.Count == 0)
			{
				var condition = new Condition();
				ConditionArguments.Conditions.Add(condition);
				var conditionViewModel = new ConditionViewModel(condition, procedure, updateDescriptionHandler);
				Conditions.Add(conditionViewModel);
			}
			JoinOperator = ConditionArguments.JoinOperator;

			AddCommand = new RelayCommand(OnAdd);
			RemoveCommand = new RelayCommand<ConditionViewModel>(OnRemove, CanRemove);
			ChangeJoinOperatorCommand = new RelayCommand(OnChangeJoinOperator);
		}

		public RelayCommand ChangeJoinOperatorCommand { get; private set; }
		void OnChangeJoinOperator()
		{
			JoinOperator = JoinOperator == JoinOperator.And ? JoinOperator.Or : JoinOperator.And;
		}

		public JoinOperator JoinOperator
		{
			get { return ConditionArguments.JoinOperator; }
			set
			{
				ConditionArguments.JoinOperator = value;
				OnPropertyChanged(()=>JoinOperator);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		public void OnAdd()
		{
			var condition = new Condition();
			var conditionViewModel = new ConditionViewModel(condition, Procedure ,UpdateDescriptionHandler);
			ConditionArguments.Conditions.Add(condition);
			Conditions.Add(conditionViewModel);
			UpdateContent();
		}

		public RelayCommand<ConditionViewModel> RemoveCommand { get; private set; }
		void OnRemove(ConditionViewModel conditionViewModel)
		{
			Conditions.Remove(conditionViewModel);
			ConditionArguments.Conditions.Remove(conditionViewModel.Condition);
			UpdateContent();
		}

		bool CanRemove(ConditionViewModel conditionViewModel)
		{
			return Conditions.Count > 1;
		}

		public bool IsJoinOperatorVisible
		{
			get { return Conditions.Count > 1; }
		}
		
		public void UpdateContent()
		{
			foreach (var conditionViewModel in Conditions)
			{
				conditionViewModel.UpdateContent();
			}
			OnPropertyChanged(() => IsJoinOperatorVisible);
		}

		public string Description
		{
			get
			{
				var conditionViewModel = Conditions.FirstOrDefault();
				if (conditionViewModel == null)
					return "";

				var var1 = "пусто";
				var var2 = "пусто";
				var Variable1 = conditionViewModel.Variable1;
				var Variable2 = conditionViewModel.Variable2;

				var1 = Variable1.SelectedVariable != null ? Variable1.SelectedVariable.Name : "пусто";
				switch (conditionViewModel.SelectedValueType)
				{
					case ValueType.Boolean:						
						var2 = Variable2.SelectedVariableType == VariableType.IsValue ? Variable2.BoolValue.ToString() : (Variable2.SelectedVariable != null ? Variable2.SelectedVariable.Name : "пусто");
						break;
					case ValueType.DateTime:
						var2 = Variable2.SelectedVariableType == VariableType.IsValue ? Variable2.DateTimeValue.ToString() : (Variable2.SelectedVariable != null ? Variable2.SelectedVariable.Name : "пусто");
						break;
					case ValueType.Integer:
						var2 = Variable2.SelectedVariableType == VariableType.IsValue ? Variable2.IntValue.ToString() : (Variable2.SelectedVariable != null ? Variable2.SelectedVariable.Name : "пусто");
						break;
					case ValueType.String:
						var2 = Variable2.SelectedVariableType == VariableType.IsValue ? Variable2.StringValue.ToString() : (Variable2.SelectedVariable != null ? Variable2.SelectedVariable.Name : "пусто");
						break;
				}

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
				if (Conditions.Count > 1)
					end = JoinOperator == JoinOperator.And ? "и ..." : "или ...";

				return var1 + " " + op + " " + var2 + " " + end;
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

	public class ConditionViewModel : BaseViewModel
	{
		public Condition Condition { get; private set; }
		public ArithmeticParameterViewModel Variable1 { get; set; }
		public ArithmeticParameterViewModel Variable2 { get; set; }
		Procedure Procedure { get; set; }
		public Action UpdateDescriptionHandler { get; set; }

		public ConditionViewModel(Condition condition, Procedure procedure, Action updateDescriptionHandler)
		{
			Condition = condition;
			Procedure = procedure;
			UpdateDescriptionHandler = updateDescriptionHandler;
			ValueTypes = new ObservableCollection<ValueType>(ProcedureHelper.GetEnumList<ValueType>().FindAll(x => x != ValueType.Object));
			Variable1 = new ArithmeticParameterViewModel(Condition.Variable1, ProcedureHelper.GetEnumList<VariableType>().FindAll(x => x != VariableType.IsValue));
			Variable1.UpdateDescriptionHandler = updateDescriptionHandler;
			Variable2 = new ArithmeticParameterViewModel(Condition.Variable2, ProcedureHelper.GetEnumList<VariableType>());
			Variable2.UpdateDescriptionHandler = updateDescriptionHandler;
			SelectedValueType = Condition.ValueType;
		}

		public ObservableCollection<ValueType> ValueTypes { get; private set; }
		public ValueType SelectedValueType
		{
			get { return Condition.ValueType; }
			set
			{
				Condition.ValueType = value;
				ConditionTypes = new ObservableCollection<ConditionType>(ProcedureHelper.ObjectTypeToConditionTypesList(SelectedValueType));
				OnPropertyChanged(() => ConditionTypes);
				UpdateContent();
				OnPropertyChanged(() => SelectedValueType);
			}
		}

		public void UpdateContent()
		{
			var allVariables = ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ValueType == SelectedValueType && !x.IsList);
			Variable1.Update(allVariables);
			Variable2.Update(allVariables);
			SelectedConditionType = ConditionTypes.Contains(Condition.ConditionType) ? Condition.ConditionType : ConditionTypes.FirstOrDefault();
		}

		public ObservableCollection<ConditionType> ConditionTypes { get; private set; }
		public ConditionType SelectedConditionType
		{
			get { return Condition.ConditionType; }
			set
			{
				Condition.ConditionType = value;
				OnPropertyChanged(() => SelectedConditionType);
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
