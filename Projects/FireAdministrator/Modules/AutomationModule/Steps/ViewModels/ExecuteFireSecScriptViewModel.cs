using AutomationModule.Properties;
using AutomationModule.ViewModels;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using StrazhAPI.Automation;

namespace AutomationModule.Steps.ViewModels
{
	public class ExecuteFireSecScriptViewModel : BaseStepViewModel
	{
		private string _currentScript;

		public string CurrentScript
		{
			get { return _currentScript; }
			set
			{
				if (string.Equals(_currentScript, value)) return;
				_currentScript = value;
				OnPropertyChanged(() => CurrentScript);
			}
		}

		ExecuteFireSecScriptArguments ExecuteFireSecScriptArguments { get; set; }

		public ExecuteFireSecScriptViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			ShowScriptListCommand = new RelayCommand(OnShowScriptList);
			ExecuteFireSecScriptArguments = stepViewModel.Step.ExecuteFireSecScriptArguments;
			SetCurrentScriptName(ExecuteFireSecScriptArguments);
		}

		public RelayCommand ShowScriptListCommand { get; set; }

		public void OnShowScriptList()
		{
			var dialog = new ScriptListViewModel();
			if (DialogService.ShowModalWindow(dialog) && dialog.SelectedScript != null)
			{
				ExecuteFireSecScriptArguments.CurrentScript = dialog.SelectedScript.ToDTO();
				SetCurrentScriptName(ExecuteFireSecScriptArguments);
			}
		}

		public override string Description
		{
			get
			{
				return ExecuteFireSecScriptArguments.CurrentScript == null
					? null
					: string.Format(Resources.ScenarioInfoText, ExecuteFireSecScriptArguments.CurrentScript.Id, ExecuteFireSecScriptArguments.CurrentScript.Name);
			}
		}

		private void SetCurrentScriptName(ExecuteFireSecScriptArguments arguments)
		{
			CurrentScript = arguments.CurrentScript != null
				? string.Format("{0}. {1}", ExecuteFireSecScriptArguments.CurrentScript.Id, ExecuteFireSecScriptArguments.CurrentScript.Name)
				: Resources.PressToSelectOPCScenarioText;
		}
	}
}
