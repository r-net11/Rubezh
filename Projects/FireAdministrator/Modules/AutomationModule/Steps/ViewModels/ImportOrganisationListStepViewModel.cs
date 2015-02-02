using FiresecAPI.Automation;

namespace AutomationModule.ViewModels
{
	public class ImportOrganisationListStepViewModel : BaseStepViewModel
	{
		ImportOrganisationArguments ImportOrganisationListArguments { get; set; }
		public ArgumentViewModel IsWithDeleted { get; private set; }
		public ArgumentViewModel PathArgument { get; private set; }

		public ImportOrganisationListStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			ImportOrganisationListArguments = stepViewModel.Step.ImportOrganisationArguments;
			IsWithDeleted = new ArgumentViewModel(ImportOrganisationListArguments.IsWithDeleted, stepViewModel.Update, UpdateContent);
			PathArgument = new ArgumentViewModel(ImportOrganisationListArguments.PathArgument, stepViewModel.Update, UpdateContent);
		}

		public override void UpdateContent()
		{
			IsWithDeleted.Update(Procedure, ExplicitType.Boolean, isList: false);
			PathArgument.Update(Procedure, ExplicitType.String, isList: false);
		}

		public override string Description
		{
			get
			{
				var result = "Экспортировать список организаций";
				if (IsWithDeleted.ExplicitValue.BoolValue)
					result += " с архивированными объектами ";
				if (!PathArgument.IsEmpty)
					result += "в " + PathArgument.Description;
				return result;
			}
		}
	}
}