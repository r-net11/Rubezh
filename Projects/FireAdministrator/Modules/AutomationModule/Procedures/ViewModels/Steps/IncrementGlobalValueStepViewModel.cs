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

		public ObservableCollection<GlobalVariableViewModel> GlobalVariables { get; private set; }

		GlobalVariableViewModel _selectedGlobalVariable;
		public GlobalVariableViewModel SelectedGlobalVariable
		{
			get { return _selectedGlobalVariable; }
			set
			{
				_selectedGlobalVariable = value;
				if (_selectedGlobalVariable != null)
					IncrementGlobalValueArguments.GlobalVariableUid = _selectedGlobalVariable.GlobalVariable.Uid;
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
			var automationChanged = ServiceFactory.SaveService.AutomationChanged;
			GlobalVariables = new ObservableCollection<GlobalVariableViewModel>();
			foreach (var globalVariable in FiresecClient.FiresecManager.SystemConfiguration.AutomationConfiguration.GlobalVariables)
			{
				var globalVariableViewModel = new GlobalVariableViewModel(globalVariable);
				GlobalVariables.Add(globalVariableViewModel);
			}
			if (GlobalVariables.Any(x => x.GlobalVariable.Uid == IncrementGlobalValueArguments.GlobalVariableUid))
				SelectedGlobalVariable = GlobalVariables.FirstOrDefault(x => x.GlobalVariable.Uid == IncrementGlobalValueArguments.GlobalVariableUid);
			else
				SelectedGlobalVariable = null;
			ServiceFactory.SaveService.AutomationChanged = automationChanged;
			OnPropertyChanged(()=>GlobalVariables);
		}

		public string Description { get { return ""; } }
	}
}
