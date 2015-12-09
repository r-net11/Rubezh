﻿using FiresecAPI.Automation;

namespace AutomationModule.ViewModels
{
	public class PtzStepViewModel : BaseStepViewModel
	{
		PtzArguments PtzArguments { get; set; }
		public ArgumentViewModel CameraArgument { get; private set; }
		public ArgumentViewModel PtzNumberArgument { get; private set; }

		public PtzStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			PtzArguments = stepViewModel.Step.PtzArguments;
			CameraArgument = new ArgumentViewModel(PtzArguments.CameraArgument, stepViewModel.Update, null);
			PtzNumberArgument = new ArgumentViewModel(PtzArguments.PtzNumberArgument, stepViewModel.Update, null)
			{
				ExplicitValue = {MinIntValue = 1, MaxIntValue = 100, IntValue = 1}
			};
		}

		public override void UpdateContent()
		{
			CameraArgument.Update(Procedure, ExplicitType.Object, objectType: ObjectType.VideoDevice, isList: false);
			PtzNumberArgument.Update(Procedure, ExplicitType.Integer);
		}

		public override string Description
		{
			get
			{
				return "Камера: " + CameraArgument.Description + " Номер команды: " + PtzNumberArgument.Description;
			}
		}
	}
}