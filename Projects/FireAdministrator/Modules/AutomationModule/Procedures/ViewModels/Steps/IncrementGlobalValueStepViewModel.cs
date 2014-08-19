using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class IncrementGlobalValueStepViewModel: BaseViewModel, IStepViewModel
	{
		IncrementGlobalValueArguments IncrementGlobalValueArguments { get; set; }

		public IncrementGlobalValueStepViewModel(IncrementGlobalValueArguments incrementGlobalValueArguments)
		{
			IncrementGlobalValueArguments = incrementGlobalValueArguments;
			IncrementTypes = new ObservableCollection<IncrementType> { IncrementType.Inc, IncrementType.Dec };
			UpdateContent();
		}

		public ObservableCollection<VariableViewModel> GlobalVariables { get; private set; }

		VariableViewModel _selectedGlobalVariable;
		public VariableViewModel SelectedGlobalVariable
		{
			get { return _selectedGlobalVariable; }
			set
			{
				_selectedGlobalVariable = value;
				if (_selectedGlobalVariable != null)
					IncrementGlobalValueArguments.GlobalVariableUid = _selectedGlobalVariable.Variable.Uid;
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(()=>SelectedGlobalVariable);
			}
		}
		
		public ObservableCollection<IncrementType> IncrementTypes { get; private set; }
		public IncrementType SelectedIncrementType
		{
			get { return IncrementGlobalValueArguments.IncrementType; }
			set
			{
				IncrementGlobalValueArguments.IncrementType = value;
				OnPropertyChanged(() => SelectedIncrementType);
			}
		}

		public void UpdateContent()
		{
			GlobalVariables = new ObservableCollection<VariableViewModel>();
			foreach (var globalVariable in FiresecClient.FiresecManager.SystemConfiguration.AutomationConfiguration.GlobalVariables)
			{
				var globalVariableViewModel = new VariableViewModel(globalVariable);
				GlobalVariables.Add(globalVariableViewModel);
			}
			if (GlobalVariables.Any(x => x.Variable.Uid == IncrementGlobalValueArguments.GlobalVariableUid))
				SelectedGlobalVariable = GlobalVariables.FirstOrDefault(x => x.Variable.Uid == IncrementGlobalValueArguments.GlobalVariableUid);
			else
				SelectedGlobalVariable = null;
			OnPropertyChanged(()=>GlobalVariables);
		}

		public string Description { get { return ""; } }
	}
}
