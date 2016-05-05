using FiresecAPI.Automation;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;

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
			Title = Resources.Language.Procedures.ViewModels.ArgumentDetailsViewModel.Title;
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
