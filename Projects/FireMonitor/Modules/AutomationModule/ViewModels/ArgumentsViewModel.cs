using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
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
				var argument = new Argument();
				argument = InitializeArgumemt(variable);
				var argumentViewModel = new ArgumentViewModel(argument);
				argumentViewModel.Name = variable.Name;
				argumentViewModel.IsList = variable.IsList;
				Arguments.Add(argumentViewModel);
			}
			OnPropertyChanged(() => Arguments);
		}

		Argument InitializeArgumemt(Variable variable)
		{
			var argument = new Argument();
			argument.ExplicitType = variable.ExplicitType;
			argument.EnumType = variable.EnumType;
			argument.ObjectType = variable.ObjectType;
			PropertyCopy.Copy<ExplicitValue, ExplicitValue>(variable.DefaultExplicitValue, argument.ExplicitValue);
			argument.ExplicitValues = new List<ExplicitValue>();
			foreach (var defaultExplicitValues in variable.DefaultExplicitValues)
			{
				var explicitValue = new ExplicitValue();
				PropertyCopy.Copy<ExplicitValue, ExplicitValue>(defaultExplicitValues, explicitValue);
				argument.ExplicitValues.Add(explicitValue);
			}
			return argument;
		}
	}
}