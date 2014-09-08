using FiresecAPI.Automation;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using ValueType = FiresecAPI.Automation.ValueType;

namespace AutomationModule.ViewModels
{
	public class PauseStepViewModel : BaseStepViewModel
	{
		private Procedure Procedure { get; set; }
		public PauseArguments PauseArguments { get; private set; }
		public ArgumentItemViewModel Pause { get; set; }

		public PauseStepViewModel(PauseArguments pauseArguments, Procedure procedure, Action updateDescriptionHandler)
			: base(updateDescriptionHandler)
		{
			PauseArguments = pauseArguments;
			Procedure = procedure;
			TimeTypes = ProcedureHelper.GetEnumObs<TimeType>();
			Pause = new ArgumentItemViewModel(procedure, PauseArguments.Pause, new List<FiresecAPI.Automation.ValueType>() { FiresecAPI.Automation.ValueType.Integer });
			UpdateContent();
		}

		public override void UpdateContent()
		{
			Pause.Update();
		}

		public override string Description
		{
			get { return ""; }
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