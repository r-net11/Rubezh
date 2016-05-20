using RubezhAPI.Automation;
using System;

namespace AutomationModule.ViewModels
{
	public class CreateColorStepViewModel : BaseStepViewModel
	{
		public CreateColorStep CreateColorStep { get; private set; }
		public ArgumentViewModel AArgument { get; set; }
		public ArgumentViewModel RArgument { get; set; }
		public ArgumentViewModel GArgument { get; set; }
		public ArgumentViewModel BArgument { get; set; }
		public ArgumentViewModel ResultArgument { get; set; }

		public CreateColorStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			CreateColorStep = (CreateColorStep)stepViewModel.Step;
			ResultArgument = new ArgumentViewModel(CreateColorStep.ResultArgument, stepViewModel.Update, UpdateContent, false);
			AArgument = new ArgumentViewModel(CreateColorStep.AArgument, stepViewModel.Update, UpdateContent);
			RArgument = new ArgumentViewModel(CreateColorStep.RArgument, stepViewModel.Update, UpdateContent);
			GArgument = new ArgumentViewModel(CreateColorStep.GArgument, stepViewModel.Update, UpdateContent);
			BArgument = new ArgumentViewModel(CreateColorStep.BArgument, stepViewModel.Update, UpdateContent);
			ResultArgument.Update(Procedure, ExplicitType.Enum, EnumType.ColorType);
			AArgument.Update(Procedure, ExplicitType.Integer);
			RArgument.Update(Procedure, ExplicitType.Integer);
			GArgument.Update(Procedure, ExplicitType.Integer);
			BArgument.Update(Procedure, ExplicitType.Integer);
		}

		public override string Description
		{
			get
			{
				return String.Format("{0} = [A:{1} R:{2} G:{3} B:{4}]", ResultArgument.Description, AArgument.Description, RArgument.Description, GArgument.Description, BArgument.Description);
			}
		}
	}
}