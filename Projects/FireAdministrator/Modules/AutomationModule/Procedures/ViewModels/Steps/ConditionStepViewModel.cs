using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.Generic;
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
			foreach (var condition in conditionArguments.Conditions)
			{
				var conditionViewModel = new ConditionViewModel(condition, procedure, updateDescriptionHandler);
				Conditions.Add(conditionViewModel);
			}
			if (Conditions.Count == 0)
			{
				var condition = new Condition();
				var conditionViewModel = new ConditionViewModel(condition, procedure, updateDescriptionHandler);
				conditionViewModel.UpdateDescriptionHandler = updateDescriptionHandler;
				ConditionArguments.Conditions.Add(condition);
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
				if (UpdateDescriptionHandler != null)
					UpdateDescriptionHandler();
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}

		public RelayCommand AddCommand { get; private set; }
		public void OnAdd()
		{
			var condition = new Condition();
			var conditionViewModel = new ConditionViewModel(condition, Procedure ,UpdateDescriptionHandler);
			ConditionArguments.Conditions.Add(condition);
			Conditions.Add(conditionViewModel);
			if (UpdateDescriptionHandler != null)
				UpdateDescriptionHandler();
			ServiceFactory.SaveService.AutomationChanged = true;
		}

		public RelayCommand<ConditionViewModel> RemoveCommand { get; private set; }
		void OnRemove(ConditionViewModel conditionViewModel)
		{
			Conditions.Remove(conditionViewModel);
			ConditionArguments.Conditions.Remove(conditionViewModel.Condition);
			if (UpdateDescriptionHandler != null)
				UpdateDescriptionHandler();
			ServiceFactory.SaveService.AutomationChanged = true;
		}

		bool CanRemove(ConditionViewModel conditionViewModel)
		{
			return Conditions.Count > 1;
		}

		public void UpdateContent()
		{
			foreach (var conditionViewModel in Conditions)
			{
				conditionViewModel.UpdateContent();
			}
		}

		public string Description
		{
			get
			{
				var conditionViewModel = Conditions.FirstOrDefault();
				if (conditionViewModel == null)
					return "";

				string var1 = conditionViewModel.Variable1.DescriptionValue;
				if (String.IsNullOrEmpty(var1))
					var1 = "пусто";
				string var2 = conditionViewModel.Variable2.DescriptionValue;
				if (String.IsNullOrEmpty(var2))
					var2 = "пусто";
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
		public ArithmeticParameterViewModel Variable1 { get; set; }
		public ArithmeticParameterViewModel Variable2 { get; set; }
		Procedure Procedure { get; set; }
		public Action UpdateDescriptionHandler { get; set; }

		public ConditionViewModel(Condition condition, Procedure procedure, Action updateDescriptionHandler)
		{
			Condition = condition;
			Procedure = procedure;
			var variablesAndArguments = new List<Variable>(Procedure.Variables);
			variablesAndArguments.AddRange(Procedure.Arguments);
			Variable1 = new ArithmeticParameterViewModel(Condition.Variable1, variablesAndArguments, true);
			Variable1.UpdateDescriptionHandler = updateDescriptionHandler;
			Variable2 = new ArithmeticParameterViewModel(Condition.Variable2, variablesAndArguments);
			Variable2.UpdateDescriptionHandler = updateDescriptionHandler;
			ConditionTypes = new ObservableCollection<ConditionType> { ConditionType.IsEqual, ConditionType.IsLess, ConditionType.IsMore, ConditionType.IsNotEqual, ConditionType.IsNotLess, ConditionType.IsNotMore};
		}

		public void UpdateContent()
		{
			var variablesAndArguments = new List<Variable>(Procedure.Variables);
			variablesAndArguments.AddRange(Procedure.Arguments);
			Variable1.Update(variablesAndArguments);
			Variable2.Update(variablesAndArguments);
		}

		public ObservableCollection<ConditionType> ConditionTypes { get; private set; }
		public ConditionType SelectedConditionType
		{
			get { return Condition.ConditionType; }
			set
			{
				Condition.ConditionType = value;
				OnPropertyChanged(() => SelectedConditionType);
				if (UpdateDescriptionHandler != null)
					UpdateDescriptionHandler();
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}
	}
}
