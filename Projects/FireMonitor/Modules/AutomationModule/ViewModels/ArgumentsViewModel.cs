using System.Linq;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Automation;
using System.Collections.ObjectModel;

namespace AutomationModule.ViewModels
{
	public class ArgumentsViewModel : BaseViewModel
	{
		public Procedure Procedure { get; private set; }

		public ArgumentsViewModel(Procedure procedure)
		{
			Procedure = procedure;
			Arguments = new ObservableCollection<ArgumentViewModel>();
			foreach (var variable in procedure.Arguments)
			{
				var argument = new Argument(variable);
				var argumentViewModel = new ArgumentViewModel(argument);
				Arguments.Add(argumentViewModel);
			}
			SelectedArgument = Arguments.FirstOrDefault();
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

		ObservableCollection<ArgumentViewModel> _arguments;
		public ObservableCollection<ArgumentViewModel> Arguments
		{
			get { return _arguments; }
			set
			{
				_arguments = value;
				OnPropertyChanged(() => Arguments);
			}
		}
	}
}