using StrazhAPI.Automation;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using Localization.Automation.ViewModels;

namespace AutomationModule.ViewModels
{
	public class ArgumentDetailsViewModel : SaveCancelDialogViewModel
	{
		private readonly bool _automationChanged;
		public Argument Argument { get; private set; }
		public ExplicitValuesViewModel ExplicitValuesViewModel { get; protected set; }

		public ArgumentDetailsViewModel(Argument argument)
		{
			_automationChanged = ServiceFactory.SaveService.AutomationChanged;
			Title = CommonViewModel.ArgumentDetailsViewModel_Title;
			ExplicitValuesViewModel = new ExplicitValuesViewModel(argument.ExplicitValue, argument.ExplicitValues, argument.ExplicitType, argument.EnumType, argument.ObjectType);
		}

		public override bool OnClosing(bool isCanceled)
		{
			ServiceFactory.SaveService.AutomationChanged = _automationChanged;
			return base.OnClosing(isCanceled);
		}

		protected override bool Save()
		{
			Argument = new Argument
			{
				VariableScope = VariableScope.ExplicitValue,
				ExplicitType = ExplicitValuesViewModel.ExplicitType,
				EnumType = ExplicitValuesViewModel.EnumType,
				ObjectType = ExplicitValuesViewModel.ObjectType,
				ExplicitValue = ExplicitValuesViewModel.ExplicitValue.ExplicitValue
			};

			foreach (var explicitValue in ExplicitValuesViewModel.ExplicitValues)
				Argument.ExplicitValues.Add(explicitValue.ExplicitValue);

			return base.Save();
		}
	}
}
