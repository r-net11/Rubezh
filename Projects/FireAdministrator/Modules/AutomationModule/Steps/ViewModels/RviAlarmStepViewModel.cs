using FiresecAPI.Automation;
using System.Collections.ObjectModel;
using FiresecAPI;

namespace AutomationModule.ViewModels
{
	public class RviAlarmStepViewModel : BaseStepViewModel
	{
		public RviAlarmArguments RviAlarmArguments { get; private set; }
		public ArgumentViewModel NameArgument { get; set; }

		public RviAlarmStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			RviAlarmArguments = stepViewModel.Step.RviAlarmArguments;
			NameArgument = new ArgumentViewModel(RviAlarmArguments.NameArgument, stepViewModel.Update, null);
			NameArgument.ExplicitValue.MinIntValue = 0;
		}

		public override void UpdateContent()
		{
			NameArgument.Update(Procedure, ExplicitType.String, isList: false);
		}

		public override string Description
		{
			get 
			{
				return "Значение: " + NameArgument.Description;
			}
		}
	}
}