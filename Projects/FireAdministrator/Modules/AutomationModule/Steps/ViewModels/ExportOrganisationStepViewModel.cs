using FiresecAPI.Automation;
using Localization.Automation.ViewModels;

namespace AutomationModule.ViewModels
{
	public class ExportOrganisationStepViewModel : BaseStepViewModel
	{
		ExportOrganisationArguments ExportOrganisationArguments { get; set; }
		public ArgumentViewModel IsWithDeleted { get; private set; }
		public ArgumentViewModel Organisation { get; private set; }
		public ArgumentViewModel PathArgument { get; private set; }

		public ExportOrganisationStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			ExportOrganisationArguments = stepViewModel.Step.ExportOrganisationArguments;
			IsWithDeleted = new ArgumentViewModel(ExportOrganisationArguments.IsWithDeleted, stepViewModel.Update, UpdateContent);
			Organisation = new ArgumentViewModel(ExportOrganisationArguments.Organisation, stepViewModel.Update, UpdateContent);
			PathArgument = new ArgumentViewModel(ExportOrganisationArguments.PathArgument, stepViewModel.Update, UpdateContent);
		}

		public override void UpdateContent()
		{
			IsWithDeleted.Update(Procedure, ExplicitType.Boolean);
			Organisation.Update(Procedure, ExplicitType.Object, objectType: ObjectType.Organisation);
			PathArgument.Update(Procedure, ExplicitType.String);
		}

		public override string Description
		{
			get
			{
				var result = StepCommonViewModel.ExportOrganisation + Organisation.Description;
				if (IsWithDeleted.ExplicitValue.BoolValue)
                    result += StepCommonViewModel.Archive;
				if (!PathArgument.IsEmpty)
                    result += StepCommonViewModel.In + PathArgument.Description;
				return result;
			}
		}
	}
}