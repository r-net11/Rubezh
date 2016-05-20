using Infrastructure.Automation;
using RubezhAPI;
using RubezhAPI.Automation;
using System.Collections.ObjectModel;

namespace AutomationModule.ViewModels
{
	public class PauseStepViewModel : BaseStepViewModel
	{
		public PauseStep PauseStep { get; private set; }
		public ArgumentViewModel PauseArgument { get; set; }

		public PauseStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			PauseStep = (PauseStep)stepViewModel.Step;
			TimeTypes = AutomationHelper.GetEnumObs<TimeType>();
			PauseArgument = new ArgumentViewModel(PauseStep.PauseArgument, stepViewModel.Update, null);
			PauseArgument.ExplicitValue.MinIntValue = 0;
		}

		public override void UpdateContent()
		{
			PauseArgument.Update(Procedure, ExplicitType.Integer, isList: false);
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
			get { return PauseStep.TimeType; }
			set
			{
				PauseStep.TimeType = value;
				OnPropertyChanged(() => SelectedTimeType);
			}
		}
	}
}