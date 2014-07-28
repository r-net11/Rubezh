using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class SetGlobalValueStepViewModel: BaseViewModel, IStepViewModel
	{
		SetGlobalValueArguments SetGlobalValueArguments { get; set; }

		public SetGlobalValueStepViewModel(SetGlobalValueArguments setGlobalValueArguments)
		{
			SetGlobalValueArguments = setGlobalValueArguments;
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
					SetGlobalValueArguments.GlobalVariableUid = _selectedGlobalVariable.GlobalVariable.Uid;
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(()=>SelectedGlobalVariable);
			}
		}

		public int Value
		{
			get { return SetGlobalValueArguments.Value; }
			set
			{
				SetGlobalValueArguments.Value = value;
				OnPropertyChanged(()=>Value);
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
			if (GlobalVariables.Any(x => x.GlobalVariable.Uid == SetGlobalValueArguments.GlobalVariableUid))
				SelectedGlobalVariable = GlobalVariables.FirstOrDefault(x => x.GlobalVariable.Uid == SetGlobalValueArguments.GlobalVariableUid);
			else
				SelectedGlobalVariable = null;
			ServiceFactory.SaveService.AutomationChanged = automationChanged;
			OnPropertyChanged(()=>GlobalVariables);
		}

		public string Description { get { return ""; } }
	}
}
