using Infrastructure;
using Infrastructure.Automation;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.Automation;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace AutomationModule.ViewModels
{
	public class ConditionStepViewModel : BaseStepViewModel
	{
		public ConditionStep ConditionStep { get; private set; }
		public ObservableCollection<ConditionViewModel> Conditions { get; private set; }

		public ConditionStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			ConditionStep = (ConditionStep)stepViewModel.Step;
			Conditions = new ObservableCollection<ConditionViewModel>();
			ConditionStep.Conditions.ForEach(condition => Conditions.Add(new ConditionViewModel(condition, Procedure, stepViewModel.Update, UpdateContent)));
			if (Conditions.Count == 0)
			{
				var condition = new Condition();
				ConditionStep.Conditions.Add(condition);
				var conditionViewModel = new ConditionViewModel(condition, Procedure, stepViewModel.Update, UpdateContent);
				Conditions.Add(conditionViewModel);
			}
			JoinOperator = ConditionStep.JoinOperator;
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
			get { return ConditionStep.JoinOperator; }
			set
			{
				ConditionStep.JoinOperator = value;
				OnPropertyChanged(() => JoinOperator);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		public void OnAdd()
		{
			var condition = new Condition();
			var conditionViewModel = new ConditionViewModel(condition, Procedure, UpdateDescriptionHandler, UpdateContent);
			ConditionStep.Conditions.Add(condition);
			Conditions.Add(conditionViewModel);
			UpdateContent();
		}

		public RelayCommand<ConditionViewModel> RemoveCommand { get; private set; }
		void OnRemove(ConditionViewModel conditionViewModel)
		{
			Conditions.Remove(conditionViewModel);
			ConditionStep.Conditions.Remove(conditionViewModel.Condition);
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

				var var1 = conditionViewModel.Argument1.Description;
				var var2 = conditionViewModel.Argument2.Description;

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
		public ArgumentViewModel Argument1 { get; set; }
		public ArgumentViewModel Argument2 { get; set; }
		Procedure Procedure { get; set; }
		public Action UpdateDescriptionHandler { get; set; }
		public EnumType SelectedEnumType { get; set; }

		public ConditionViewModel(Condition condition, Procedure procedure, Action updateDescriptionHandler, Action updateContentHandler)
		{
			Condition = condition;
			Procedure = procedure;
			UpdateDescriptionHandler = updateDescriptionHandler;
			ExplicitTypes = AutomationHelper.GetEnumObs<ExplicitType>();
			Argument1 = new ArgumentViewModel(Condition.Argument1, updateDescriptionHandler, updateContentHandler, false);
			Argument1.UpdateVariableHandler += UpdateArgument2;
			Argument2 = new ArgumentViewModel(Condition.Argument2, updateDescriptionHandler, updateContentHandler);
			SelectedExplicitType = Condition.ExplicitType;
		}

		public ObservableCollection<ExplicitType> ExplicitTypes { get; private set; }
		public ExplicitType SelectedExplicitType
		{
			get { return Condition.ExplicitType; }
			set
			{
				Condition.ExplicitType = value;
				ConditionTypes = new ObservableCollection<ConditionType>(AutomationHelper.ObjectTypeToConditionTypesList(SelectedExplicitType));
				OnPropertyChanged(() => ConditionTypes);
				Argument1.ExplicitType = value;
				UpdateContent();
				OnPropertyChanged(() => SelectedExplicitType);
			}
		}

		public void UpdateContent()
		{
			SelectedEnumType = Argument1.EnumType;
			Argument1.Update(Procedure, SelectedExplicitType, isList: false);
			Argument1.EnumType = SelectedEnumType;
			UpdateArgument2();
			SelectedConditionType = ConditionTypes.Contains(Condition.ConditionType) ? Condition.ConditionType : ConditionTypes.FirstOrDefault();
		}

		void UpdateArgument2()
		{
			Argument2.Update(Procedure, SelectedExplicitType, SelectedEnumType, Argument1.ObjectType, false);
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
