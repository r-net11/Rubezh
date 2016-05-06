using StrazhAPI.Automation;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class ArgumentDetailsViewModel : SaveCancelDialogViewModel
	{
		public Argument Argument { get; private set; }
		public ExplicitValuesViewModel ExplicitValuesViewModel { get; protected set; }

		public ArgumentDetailsViewModel(Argument argument)
		{
			Title = "Редактировать аргумент";
			ExplicitValuesViewModel = new ExplicitValuesViewModel(argument.ExplicitValue, argument.ExplicitValues, argument.ExplicitType, argument.EnumType, argument.ObjectType);
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
