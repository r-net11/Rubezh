using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Localization.Automation.ViewModels;
using StrazhAPI;
using StrazhAPI.Enums;
using StrazhAPI.Models.Automation.StepArguments;

namespace AutomationModule.ViewModels
{
	public class GuardZoneControlStepViewModel : BaseStepViewModel
	{
		private readonly GuardZoneControlArguments _guardZoneControlArguments;
		private string _currentGuardZone;

		public string CurrentGuardZone
		{
			get { return _currentGuardZone; }
			set
			{
				if (string.Equals(_currentGuardZone, value)) return;
				_currentGuardZone = value;
				OnPropertyChanged(() => CurrentGuardZone);
			}
		}

		public GuardZoneCommand SelectedCommand
		{
			get { return _guardZoneControlArguments.CommandType; }
			set
			{
				if (_guardZoneControlArguments.CommandType == value) return;
				_guardZoneControlArguments.CommandType = value;
				OnPropertyChanged(() => SelectedCommand);
			}
		}

		public GuardZoneControlStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			ShowGuardListCommand = new RelayCommand(OnShowGuardListCommand);
			_guardZoneControlArguments = stepViewModel.Step.GuardZoneControlArguments;
			SetCurrentGuardZoneName(_guardZoneControlArguments);
		}

		public RelayCommand ShowGuardListCommand { get; set; }

		public void OnShowGuardListCommand()
		{
			var dialog = new GuardZonesListViewModel();
			if (DialogService.ShowModalWindow(dialog) && dialog.SelectedZone != null)
			{
				_guardZoneControlArguments.CurrentGuardZone = dialog.SelectedZone;
				SetCurrentGuardZoneName(_guardZoneControlArguments);
			}
		}

		private void SetCurrentGuardZoneName(GuardZoneControlArguments arguments)
		{
			CurrentGuardZone = arguments.CurrentGuardZone != null
				? string.Format("{0}. {1}", arguments.CurrentGuardZone.No, arguments.CurrentGuardZone.Name)
				: StepCommonViewModel.LabelPressToSelectOPCZone;
		}

		public override string Description
		{
			get
			{
				if (_guardZoneControlArguments == null || _guardZoneControlArguments.CurrentGuardZone == null) return null;

                return string.Format(StepCommonViewModel.LabelGuardZoneInfo,
					_guardZoneControlArguments.CurrentGuardZone.No,
					_guardZoneControlArguments.CurrentGuardZone.Name,
					SelectedCommand.ToDescription());
			}
		}
	}
}
