using System.Linq;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Automation;
using System.Collections.ObjectModel;

namespace AutomationModule.ViewModels
{
	public class VariableDetailsViewModel : SaveCancelDialogViewModel
	{
		public VariableDetailsViewModel(string title = "Добавить переменную", string defaultName = "Локальная переменная")
		{
			Title = title;
			Initialize(defaultName);
		}

		public VariableDetailsViewModel(VariableViewModel variableViewModel, string title = "Добавить переменную", string defaultName = "Локальная переменная")
		{
			Title = title;
			Initialize(variableViewModel, defaultName);
		}

		void Initialize(VariableViewModel variableViewModel, string name)
		{
			Variables = new ObservableCollection<VariableViewModel>
			{
				variableViewModel.VariableType != VariableType.Boolean
					? new VariableViewModel(name) {VariableType = VariableType.Boolean} : new VariableViewModel(variableViewModel.Variable),
				variableViewModel.VariableType != VariableType.DateTime
					? new VariableViewModel(name) {VariableType = VariableType.DateTime} : new VariableViewModel(variableViewModel.Variable),
				variableViewModel.VariableType != VariableType.Integer
					? new VariableViewModel(name) {VariableType = VariableType.Integer} : new VariableViewModel(variableViewModel.Variable),
				variableViewModel.VariableType != VariableType.Object
					? new VariableViewModel(name) {VariableType = VariableType.Object} : new VariableViewModel(variableViewModel.Variable),
				variableViewModel.VariableType != VariableType.String
					? new VariableViewModel(name) {VariableType = VariableType.String} : new VariableViewModel(variableViewModel.Variable)
			};
			SelectedVariable = Variables.FirstOrDefault(x => x.VariableType == variableViewModel.VariableType);
		}

		void Initialize(string name)
		{
			Variables = new ObservableCollection<VariableViewModel>
			{
				new VariableViewModel(name) { VariableType = VariableType.Boolean },
				new VariableViewModel(name) { VariableType = VariableType.DateTime },
				new VariableViewModel(name) { VariableType = VariableType.Integer },
				new VariableViewModel(name) { VariableType = VariableType.Object },
				new VariableViewModel(name) { VariableType = VariableType.String }
			};
			SelectedVariable = Variables.FirstOrDefault();
		}
		
		VariableViewModel _selectedVariable;
		public VariableViewModel SelectedVariable
		{
			get { return _selectedVariable; }
			set
			{
				_selectedVariable = value;
				OnPropertyChanged(()=>SelectedVariable);
			}
		}

		ObservableCollection<VariableViewModel> _variables;
		public ObservableCollection<VariableViewModel> Variables
		{
			get { return _variables; }
			set
			{
				_variables = value;
				OnPropertyChanged(()=>Variables);
			}
		}

		protected override bool Save()
		{
			return base.Save();
		}
	}	
}