﻿using FiresecAPI.Automation;

namespace AutomationModule.ViewModels
{
	public class NowStepViewModel : BaseStepViewModel
	{
		public ArgumentViewModel ResultArgument { get; private set; }
		public NowArguments NowArguments { get; private set; }

		public NowStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			NowArguments = stepViewModel.Step.NowArguments;
			ResultArgument = new ArgumentViewModel(NowArguments.ResultArgument, stepViewModel.Update, UpdateContent, false);
		}

		public override void UpdateContent()
		{
			ResultArgument.Update(Procedure, ExplicitType.DateTime, isList: false);
		}

		public override string Description
		{
			get
			{
				return "Результат: " + ResultArgument.Description;
			}
		}
	}
}
