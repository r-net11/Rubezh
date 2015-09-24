﻿using FiresecAPI.Automation;
using System.Collections.ObjectModel;
using FiresecAPI;
using Infrastructure.Automation;

namespace AutomationModule.ViewModels
{
	public class PauseStepViewModel : BaseStepViewModel
	{
		public PauseArguments PauseArguments { get; private set; }
		public ArgumentViewModel PauseArgument { get; set; }

		public PauseStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			PauseArguments = stepViewModel.Step.PauseArguments;
			TimeTypes = AutomationHelper.GetEnumObs<TimeType>();
			PauseArgument = new ArgumentViewModel(PauseArguments.PauseArgument, stepViewModel.Update, null);
			PauseArgument.ExplicitValue.MinIntValue = 0;
		}

		public override void UpdateContent()
		{
			PauseArgument.Update(Procedure, ExplicitType.Integer, isList:false);
		}

		public override string Description
		{
			get 
			{
				return "Значение: " + PauseArgument.Description + " " + SelectedTimeType.ToDescription(); 
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