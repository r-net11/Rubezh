using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.Generic;
using FiresecClient;
using System.Linq.Expressions;

namespace AutomationModule.ViewModels
{
	public class ConditionStepViewModel : BaseStepViewModel
	{
		public ConditionArguments ConditionArguments { get; private set; }
		public ObservableCollection<ConditionViewModel> Conditions { get; private set; }

		public ConditionStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			ConditionArguments = stepViewModel.Step.ConditionArguments;
			Conditions = new ObservableCollection<ConditionViewModel>();
			ConditionArguments.Conditions.ForEach(condition => Conditions.Add(new ConditionViewModel(condition, Procedure, stepViewModel.Update)));
			if (Conditions.Count == 0)
			{
				var condition = new Condition();
				ConditionArguments.Conditions.Add(condition);
				var conditionViewModel = new ConditionViewModel(condition, Procedure, stepViewModel.Update);
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
		
		public override void UpdateContent()
		{
			foreach (var conditionViewModel in Conditions)
			{
				conditionViewModel.UpdateContent();
			}
			OnPropertyChanged(() => IsJoinOperatorVisible);
		}

		public override string Description
		{
			get
			{
				var conditionViewModel = Conditions.FirstOrDefault();
				if (conditionViewModel == null)
					return "";

				var var1 = conditionViewModel.Parameter1.Description;
				var var2 = conditionViewModel.Parameter2.Description;

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
	}

	public class ConditionViewModel : BaseViewModel
	{
		public Condition Condition { get; private set; }
		public ArgumentViewModel Parameter1 { get; set; }
		public ArgumentViewModel Parameter2 { get; set; }
		Procedure Procedure { get; set; }
		public Action UpdateDescriptionHandler { get; set; }

		public ConditionViewModel(Condition condition, Procedure procedure, Action updateDescriptionHandler)
		{
			Condition = condition;
			Procedure = procedure;
			UpdateDescriptionHandler = updateDescriptionHandler;
			ExplicitTypes = new ObservableCollection<ExplicitType>(ProcedureHelper.GetEnumList<ExplicitType>().FindAll(x => x != ExplicitType.Object));
			Parameter1 = new ArgumentViewModel(Condition.Parameter1, updateDescriptionHandler, false);
			Parameter1.UpdateVariableHandler += UpdateParameter2;
			Parameter2 = new ArgumentViewModel(Condition.Parameter2, updateDescriptionHandler);
			SelectedExplicitType = Condition.ExplicitType;
		}

		public ObservableCollection<ExplicitType> ExplicitTypes { get; private set; }
		public ExplicitType SelectedExplicitType
		{
			get { return Condition.ExplicitType; }
			set
			{
				Condition.ExplicitType = value;
				ConditionTypes = new ObservableCollection<ConditionType>(ProcedureHelper.ObjectTypeToConditionTypesList(SelectedExplicitType));
				OnPropertyChanged(() => ConditionTypes);
				UpdateContent();
				OnPropertyChanged(() => SelectedExplicitType);
			}
		}

		public void UpdateContent()
		{
			var allVariables = ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ExplicitType == SelectedExplicitType && !x.IsList);
			Parameter1.Update(allVariables);
			Parameter2.Update(allVariables);
			Parameter1.ExplicitType = SelectedExplicitType;
			Parameter2.ExplicitType = SelectedExplicitType;
			SelectedConditionType = ConditionTypes.Contains(Condition.ConditionType) ? Condition.ConditionType : ConditionTypes.FirstOrDefault();
		}

		void UpdateParameter2()
		{
			if (Parameter1.ExplicitType == ExplicitType.Enum)
			{
				var allVariables = ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ExplicitType == SelectedExplicitType && !x.IsList && x.EnumType == Parameter1.SelectedEnumType);
				Parameter2.Update(allVariables);
				Parameter2.SelectedEnumType = Parameter1.SelectedEnumType;
			}
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
