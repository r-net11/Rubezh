using StrazhAPI.Automation;
using Localization.Automation.ViewModels;

namespace AutomationModule.ViewModels
{
	public class ExportOrganisationListStepViewModel : BaseStepViewModel
	{
		ImportOrganisationArguments ImportOrganisationListArguments { get; set; }
		public ArgumentViewModel IsWithDeleted { get; private set; }
		public ArgumentViewModel PathArgument { get; private set; }

		public ExportOrganisationListStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			ImportOrganisationListArguments = stepViewModel.Step.ImportOrganisationArguments;
			IsWithDeleted = new ArgumentViewModel(ImportOrganisationListArguments.IsWithDeleted, stepViewModel.Update, UpdateContent);
			PathArgument = new ArgumentViewModel(ImportOrganisationListArguments.PathArgument, stepViewModel.Update, UpdateContent);
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
				var result = StepCommonViewModel.ExportOrganisationList;
				if (IsWithDeleted.ExplicitValue.BoolValue)
                    result += StepCommonViewModel.Archive;
				if (!PathArgument.IsEmpty)
                    result += StepCommonViewModel.In + PathArgument.Description;
				return result;
			}
		}
	}
}