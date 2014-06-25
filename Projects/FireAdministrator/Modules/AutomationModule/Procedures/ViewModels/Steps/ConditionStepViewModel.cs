using System.Collections.ObjectModel;
using FiresecAPI.Automation;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
namespace AutomationModule.ViewModels
{
	public class ConditionStepViewModel : BaseViewModel, IStepViewModel
	{
		public ConditionArguments ConditionArguments { get; private set; }
		public ObservableCollection<ConditionViewModel> Conditions { get; private set; }
		Procedure Procedure { get; set; }

		public ConditionStepViewModel(ConditionArguments conditionArguments, Procedure procedure)
		{
			ConditionArguments = conditionArguments;
			Procedure = procedure;

			Conditions = new ObservableCollection<ConditionViewModel>();
			foreach (var condition in conditionArguments.Conditions)
			{
				var conditionViewModel = new ConditionViewModel(condition, procedure);
				Conditions.Add(conditionViewModel);
			}
			var automationChanged = ServiceFactory.SaveService.AutomationChanged;
			JoinOperator = ConditionArguments.JoinOperator;
			ServiceFactory.SaveService.AutomationChanged = automationChanged;

			AddCommand = new RelayCommand(OnAdd);
			RemoveCommand = new RelayCommand<ConditionViewModel>(OnRemove);
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
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}

		public RelayCommand<ConditionViewModel> RemoveCommand { get; private set; }
		void OnRemove(ConditionViewModel conditionViewModel)
		{
			Conditions.Remove(conditionViewModel);
			ConditionArguments.Conditions.Remove(conditionViewModel.Condition);
			ServiceFactory.SaveService.AutomationChanged = true;
		}

		public RelayCommand AddCommand { get; private set; }
		public void OnAdd()
		{
			var condition = new Condition();
			var conditionViewModel = new ConditionViewModel(condition, Procedure);
			ConditionArguments.Conditions.Add(condition);
			Conditions.Add(conditionViewModel);
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
				return "";
			}
		}
	}

	public class ConditionViewModel : BaseViewModel
	{
		public Condition Condition { get; private set; }
		public ArithmeticParameterViewModel Variable1 { get; set; }
		public ArithmeticParameterViewModel Variable2 { get; set; }
		Procedure Procedure { get; set; }

		public ConditionViewModel(Condition condition, Procedure procedure)
		{
			Condition = condition;
			Procedure = procedure;
			Variable1 = new ArithmeticParameterViewModel(Condition.Variable1, procedure.Variables, true);
			Variable2 = new ArithmeticParameterViewModel(Condition.Variable2, procedure.Variables);
			ConditionTypes = new ObservableCollection<ConditionType> { ConditionType.IsEqual, ConditionType.IsLess, ConditionType.IsMore, ConditionType.IsNotEqual, ConditionType.IsNotLess, ConditionType.IsNotMore};
		}

		public void UpdateContent()
		{
			Variable1.Update(Procedure.Variables);
			Variable2.Update(Procedure.Variables);
		}

		public ObservableCollection<ConditionType> ConditionTypes { get; private set; }
		public ConditionType SelectedConditionType
		{
			get { return Condition.ConditionType; }
			set
			{
				Condition.ConditionType = value;
				OnPropertyChanged(() => SelectedConditionType);
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}
	}
}
