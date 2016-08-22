using System;
using System.Collections.ObjectModel;
using System.Linq;
using StrazhAPI.Automation;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using System.Linq.Expressions;
using Localization.Automation.ViewModels;

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
			ConditionArguments.Conditions.ForEach(condition => Conditions.Add(new ConditionViewModel(condition, Procedure, stepViewModel.Update, UpdateContent)));
			if (!Conditions.Any())
			{
				var condition = new Condition();
				ConditionArguments.Conditions.Add(condition);
				var conditionViewModel = new ConditionViewModel(condition, Procedure, stepViewModel.Update, UpdateContent);
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
			var conditionViewModel = new ConditionViewModel(condition, Procedure, UpdateDescriptionHandler, UpdateContent);
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
					return string.Empty;

				var var1 = conditionViewModel.Argument1.Description;
				var var2 = conditionViewModel.Argument2.Description;

				var op = string.Empty;
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
				var end = string.Empty;
				if (Conditions.Count > 1)
                    end = JoinOperator == JoinOperator.And ? StepCommonViewModel.Condition_AND : StepCommonViewModel.Condition_OR;

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
		private EnumType _selectedEnumType { get; set; }
		private ObjectType _selectedObjectType { get; set; }

		public ConditionViewModel(Condition condition, Procedure procedure, Action updateDescriptionHandler, Action updateContentHandler)
		{
			Condition = condition;
			Procedure = procedure;
			UpdateDescriptionHandler = updateDescriptionHandler;
			ExplicitTypes = ProcedureHelper.GetEnumObs<ExplicitType>();
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
				ConditionTypes = new ObservableCollection<ConditionType>(ProcedureHelper.ObjectTypeToConditionTypesList(SelectedExplicitType));
				OnPropertyChanged(() => ConditionTypes);
				UpdateContent();
				OnPropertyChanged(() => SelectedExplicitType);
			}
		}

		public void UpdateContent()
		{
			_selectedEnumType = Argument1.EnumType;
			_selectedObjectType = Argument1.ObjectType;
			Argument1.Update(Procedure, SelectedExplicitType);
			Argument1.ExplicitType = SelectedExplicitType;
			Argument1.EnumType = _selectedEnumType;
			Argument1.ObjectType = _selectedObjectType;
			UpdateArgument2();
			SelectedConditionType = ConditionTypes.Contains(Condition.ConditionType) ? Condition.ConditionType : ConditionTypes.FirstOrDefault();
		}

		void UpdateArgument2()
		{
			Argument2.Update(Procedure, SelectedExplicitType, Argument1.EnumType, Argument1.ObjectType);
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
