using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class ControlGKGuardZoneStepViewModel: BaseViewModel, IStepViewModel
	{
		ControlGKGuardZoneArguments ControlGKGuardZoneArguments { get; set; }
		public ControlGKGuardZoneStepViewModel(ControlGKGuardZoneArguments controlGKGuardZoneArguments)
		{
			ControlGKGuardZoneArguments = controlGKGuardZoneArguments;
			Commands = new ObservableCollection<GuardZoneCommandType>
			{
				GuardZoneCommandType.Automatic, GuardZoneCommandType.Manual, GuardZoneCommandType.Ignore, GuardZoneCommandType.TurnOn,
				GuardZoneCommandType.TurnOnNow, GuardZoneCommandType.TurnOff, GuardZoneCommandType.Reset
			};
			OnPropertyChanged(() => Commands);
			SelectZoneCommand = new RelayCommand(OnSelectZone);
			UpdateContent();
		}

		public ObservableCollection<GuardZoneCommandType> Commands { get; private set; }

		GuardZoneCommandType _selectedCommand;
		public GuardZoneCommandType SelectedCommand
		{
			get { return _selectedCommand; }
			set
			{
				_selectedCommand = value;
				ControlGKGuardZoneArguments.GuardZoneCommandType = value;
				OnPropertyChanged(()=>SelectedCommand);
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}

		ZoneViewModel _selectedZone;
		public ZoneViewModel SelectedZone
		{
			get { return _selectedZone; }
			set
			{
				_selectedZone = value;
				ControlGKGuardZoneArguments.ZoneUid = Guid.Empty;
				if (_selectedZone != null)
				{
					ControlGKGuardZoneArguments.ZoneUid = _selectedZone.GuardZone.UID;
				}
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => SelectedZone);
			}
		}
		
		public RelayCommand SelectZoneCommand { get; private set; }
		private void OnSelectZone()
		{
			var zoneSelectationViewModel = new GuardZoneSelectionViewModel(SelectedZone != null ? SelectedZone.GuardZone : null);
			if (DialogService.ShowModalWindow(zoneSelectationViewModel))
			{
				SelectedZone = zoneSelectationViewModel.SelectedZone;
			}
		}

		public void UpdateContent()
		{
			if (ControlGKGuardZoneArguments.ZoneUid != Guid.Empty)
			{
				var zone = XManager.DeviceConfiguration.GuardZones.FirstOrDefault(x => x.UID == ControlGKGuardZoneArguments.ZoneUid);
				SelectedZone = zone != null ? new ZoneViewModel(zone) : null;
				SelectedCommand = ControlGKGuardZoneArguments.GuardZoneCommandType;
			}
		}

		public string Description
		{
			get { return ""; }
		}
	}
}
