using Infrastructure.Common.Windows.Windows.ViewModels;
using RubezhAPI.Automation;
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
				var argumentViewModel = new ArgumentViewModel(variable);
				Arguments.Add(argumentViewModel);
			}
			OnPropertyChanged(() => Arguments);
		}
	}
}