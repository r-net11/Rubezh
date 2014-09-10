using FiresecAPI.Automation;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using FiresecAPI;

namespace AutomationModule.ViewModels
{
	public class PauseStepViewModel : BaseStepViewModel
	{
		public PauseArguments PauseArguments { get; private set; }
		public ArithmeticParameterViewModel Pause { get; set; }

		public PauseStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			PauseArguments = stepViewModel.Step.PauseArguments;
			TimeTypes = ProcedureHelper.GetEnumObs<TimeType>();
			Pause = new ArithmeticParameterViewModel(PauseArguments.Pause, stepViewModel.Update);
			Pause.ExplicitType = ExplicitType.Integer;
			UpdateContent();
		}

		public override void UpdateContent()
		{
			Pause.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ExplicitType == ExplicitType.Integer && !x.IsList));
		}

		public override string Description
		{
			get 
			{
				return "Значение: " + Pause.Description + " " + SelectedTimeType.ToDescription(); 
			}
		}

		public ObservableCollection<TimeType> TimeTypes { get; private set; }

		public TimeType SelectedTimeType
		{
			get { return PauseArguments.TimeType; }
			set
			{
				PauseArguments.TimeType = value;
				OnPropertyChanged(() => SelectedTimeType);
			}
		}
	}
}