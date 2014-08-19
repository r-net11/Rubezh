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

		public ObservableCollection<VariableViewModel> GlobalVariables { get; private set; }

		VariableViewModel _selectedGlobalVariable;
		public VariableViewModel SelectedGlobalVariable
		{
			get { return _selectedGlobalVariable; }
			set
			{
				_selectedGlobalVariable = value;
				if (_selectedGlobalVariable != null)
					SetGlobalValueArguments.GlobalVariableUid = _selectedGlobalVariable.Variable.Uid;
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
			GlobalVariables = new ObservableCollection<VariableViewModel>();
			foreach (var globalVariable in FiresecClient.FiresecManager.SystemConfiguration.AutomationConfiguration.GlobalVariables)
			{
				var globalVariableViewModel = new VariableViewModel(globalVariable);
				GlobalVariables.Add(globalVariableViewModel);
			}
			if (GlobalVariables.Any(x => x.Variable.Uid == SetGlobalValueArguments.GlobalVariableUid))
				SelectedGlobalVariable = GlobalVariables.FirstOrDefault(x => x.Variable.Uid == SetGlobalValueArguments.GlobalVariableUid);
			else
				SelectedGlobalVariable = null;
			OnPropertyChanged(()=>GlobalVariables);
		}

		public string Description { get { return ""; } }
	}
}
