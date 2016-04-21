using FiresecAPI.Automation;
using FiresecAPI.Models.Automation;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.Generic;

namespace AutomationModule.ViewModels
{
	public class ArgumentsViewModel : BaseViewModel
	{
		public Procedure Procedure { get; private set; }
		public List<ArgumentViewModel> Arguments { get; private set; }

		public ArgumentsViewModel(Procedure procedure)
		{
			Procedure = procedure;
			UpdateArguments();
		}

		ArgumentViewModel _selectedArgument;
		public ArgumentViewModel SelectedArgument
		{
			get { return _selectedArgument; }
			set
			{
				_selectedArgument = value;
				OnPropertyChanged(() => SelectedArgument);
			}
		}

		public void UpdateArguments()
		{
			Arguments = new List<ArgumentViewModel>();

			foreach (var variable in Procedure.Arguments)
			{
				var argument = InitializeArgumemt(variable);
				argument.VariableScope = VariableScope.ExplicitValue;
				var argumentViewModel = new ArgumentViewModel(argument) { Name = variable.Name };
				Arguments.Add(argumentViewModel);
			}

			OnPropertyChanged(() => Arguments);
		}

		private static Argument InitializeArgumemt(IVariable variable)
		{
			var argument = new Argument
			{
				ExplicitType = variable.VariableValue.ExplicitType,
				EnumType = variable.VariableValue.EnumType,
				ObjectType = variable.VariableValue.ObjectType
			};
			PropertyCopy.Copy(variable.VariableValue.ExplicitValue, argument.ExplicitValue);
			argument.ExplicitValues = new List<ExplicitValue>();
			foreach (var explicitValues in variable.VariableValue.ExplicitValues)
			{
				var newExplicitValue = new ExplicitValue();
				PropertyCopy.Copy(explicitValues, newExplicitValue);
				argument.ExplicitValues.Add(newExplicitValue);
			}
			return argument;
		}
	}
}