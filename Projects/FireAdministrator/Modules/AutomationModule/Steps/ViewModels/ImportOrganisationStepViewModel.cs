using FiresecAPI.Automation;
using Localization.Automation.ViewModels;

namespace AutomationModule.ViewModels
{
	public class ImportOrganisationStepViewModel : BaseStepViewModel
	{
		ImportOrganisationArguments ImportOrganisationArguments { get; set; }
		public ArgumentViewModel IsWithDeleted { get; private set; }
		public ArgumentViewModel PathArgument { get; private set; }

		public ImportOrganisationStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			ImportOrganisationArguments = stepViewModel.Step.ImportOrganisationArguments;
			IsWithDeleted = new ArgumentViewModel(ImportOrganisationArguments.IsWithDeleted, stepViewModel.Update, UpdateContent);
			PathArgument = new ArgumentViewModel(ImportOrganisationArguments.PathArgument, stepViewModel.Update, UpdateContent);
		}

		public override void UpdateContent()
		{
			IsWithDeleted.Update(Procedure, ExplicitType.Boolean);
			PathArgument.Update(Procedure, ExplicitType.String);
		}

		public override string Description
		{
			get
			{
				var result = StepCommonViewModel.ImportOrganisation;
				if (IsWithDeleted.ExplicitValue.BoolValue)
					result += StepCommonViewModel.Archive;
				if (!PathArgument.IsEmpty)
					result += StepCommonViewModel.In + PathArgument.Description;
				return result;
			}
		}
	}
}
