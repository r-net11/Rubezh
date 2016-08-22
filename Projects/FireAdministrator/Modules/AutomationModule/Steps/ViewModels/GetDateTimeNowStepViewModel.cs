using StrazhAPI;
using StrazhAPI.Automation;
using StrazhAPI.Enums;
using Localization.Automation.ViewModels;

namespace AutomationModule.ViewModels
{
	public class GetDateTimeNowStepViewModel : BaseStepViewModel
	{
		#region Properties
		private GetDateTimeNowArguments GetDateTimeNowArguments { get; set; }

		public ArgumentViewModel Result { get; private set; }

		public RoundingType SelectedRoundingType
		{
			get { return GetDateTimeNowArguments.RoundingType; }
			set
			{
				GetDateTimeNowArguments.RoundingType = value;
				OnPropertyChanged(() => SelectedRoundingType);
			}
		}
		#endregion

		public GetDateTimeNowStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			GetDateTimeNowArguments = stepViewModel.Step.GetDateTimeNowArguments;
			Result = new ArgumentViewModel(GetDateTimeNowArguments.Result, stepViewModel.Update, UpdateContent, false);
		}

		public override void UpdateContent()
		{
			Result.Update(Procedure, ExplicitType.Time);
		}

		public override string Description
		{
			get { return string.Format(StepCommonViewModel.GetDateTimeNow, Result.Description, SelectedRoundingType.ToDescription()); }
		}
	}
}
