using RubezhAPI.Automation;

namespace AutomationModule.ViewModels
{
	public class CheckPermissionStepViewModel : BaseStepViewModel
	{
		CheckPermissionStep CheckPermissionStep { get; set; }
		public ArgumentViewModel PermissionArgument { get; private set; }
		public ArgumentViewModel ResultArgument { get; private set; }

		public CheckPermissionStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			CheckPermissionStep = (CheckPermissionStep)stepViewModel.Step;
			PermissionArgument = new ArgumentViewModel(CheckPermissionStep.PermissionArgument, stepViewModel.Update, UpdateContent);
			ResultArgument = new ArgumentViewModel(CheckPermissionStep.ResultArgument, stepViewModel.Update, UpdateContent, false);
		}

		public override void UpdateContent()
		{
			PermissionArgument.Update(Procedure, ExplicitType.Enum, EnumType.PermissionType, isList: false);
			ResultArgument.Update(Procedure, ExplicitType.Boolean, isList: false);
		}

		public override string Description
		{
			get
			{
				return "Проверка прав: " + PermissionArgument.Description + " Результат: " + ResultArgument.Description;
			}
		}
	}
}
