using StrazhAPI.Automation;

namespace AutomationModule.ViewModels
{
	public class CheckPermissionStepViewModel : BaseStepViewModel
	{
		CheckPermissionArguments CheckPermissionArguments { get; set; }
		public ArgumentViewModel PermissionArgument { get; private set; }
		public ArgumentViewModel ResultArgument { get; private set; }

		public CheckPermissionStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			CheckPermissionArguments = stepViewModel.Step.CheckPermissionArguments;
			PermissionArgument = new ArgumentViewModel(CheckPermissionArguments.PermissionArgument, stepViewModel.Update, UpdateContent);
			ResultArgument = new ArgumentViewModel(CheckPermissionArguments.ResultArgument, stepViewModel.Update, UpdateContent, false);
		}

		public override void UpdateContent()
		{
			PermissionArgument.Update(Procedure, ExplicitType.Enum, EnumType.PermissionType);
			ResultArgument.Update(Procedure, ExplicitType.Boolean);
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
