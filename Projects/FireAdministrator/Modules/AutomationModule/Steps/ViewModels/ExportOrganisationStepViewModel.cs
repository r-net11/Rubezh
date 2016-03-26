using RubezhAPI.Automation;

namespace AutomationModule.ViewModels
{
	public class ExportOrganisationStepViewModel : BaseStepViewModel
	{
		ExportOrganisationStep ExportOrganisationStep { get; set; }
		public ArgumentViewModel IsWithDeleted { get; private set; }
		public ArgumentViewModel Organisation { get; private set; }
		public ArgumentViewModel PathArgument { get; private set; }

		public ExportOrganisationStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			ExportOrganisationStep = (ExportOrganisationStep)stepViewModel.Step;
			IsWithDeleted = new ArgumentViewModel(ExportOrganisationStep.IsWithDeleted, stepViewModel.Update, UpdateContent);
			Organisation = new ArgumentViewModel(ExportOrganisationStep.Organisation, stepViewModel.Update, UpdateContent);
			PathArgument = new ArgumentViewModel(ExportOrganisationStep.PathArgument, stepViewModel.Update, UpdateContent);
		}

		public override void UpdateContent()
		{
			IsWithDeleted.Update(Procedure, ExplicitType.Boolean, isList: false);
			Organisation.Update(Procedure, ExplicitType.Object, objectType: ObjectType.Organisation, isList: false);
			PathArgument.Update(Procedure, ExplicitType.String, isList: false);
		}

		public override string Description
		{
			get
			{
				var result = "Экспортировать " + Organisation.Description;
				if (IsWithDeleted.ExplicitValue.BoolValue)
					result += " с архивированными объектами ";
				if (!PathArgument.IsEmpty)
					result += "в " + PathArgument.Description;
				return result;
			}
		}
	}
}