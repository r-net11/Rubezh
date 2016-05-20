using RubezhAPI.Automation;

namespace AutomationModule.ViewModels
{
	public class ExportOrganisationListStepViewModel : BaseStepViewModel
	{
		ExportOrganisationListStep ExportOrganisationListStep { get; set; }
		public ArgumentViewModel IsWithDeleted { get; private set; }
		public ArgumentViewModel PathArgument { get; private set; }

		public ExportOrganisationListStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			ExportOrganisationListStep = (ExportOrganisationListStep)stepViewModel.Step;
			IsWithDeleted = new ArgumentViewModel(ExportOrganisationListStep.IsWithDeleted, stepViewModel.Update, UpdateContent);
			PathArgument = new ArgumentViewModel(ExportOrganisationListStep.PathArgument, stepViewModel.Update, UpdateContent);
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