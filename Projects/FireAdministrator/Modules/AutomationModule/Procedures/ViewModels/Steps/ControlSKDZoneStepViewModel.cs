using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using FiresecAPI.SKD;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class ControlSKDZoneStepViewModel: BaseViewModel, IStepViewModel
	{
		ControlSKDZoneArguments ControlSKDZoneArguments { get; set; }
		public ControlSKDZoneStepViewModel(ControlSKDZoneArguments controlSKDZoneArguments)
		{
			ControlSKDZoneArguments = controlSKDZoneArguments;
			Commands = new ObservableCollection<SKDZoneCommandType>
			{
				SKDZoneCommandType.Open, SKDZoneCommandType.Close, SKDZoneCommandType.OpenForever,
				SKDZoneCommandType.CloseForever, SKDZoneCommandType.DetectEmployees
			};
			OnPropertyChanged(() => Commands);
			SelectZoneCommand = new RelayCommand(OnSelectZone);
			UpdateContent();
		}

		public ObservableCollection<SKDZoneCommandType> Commands { get; private set; }

		SKDZoneCommandType _selectedCommand;
		public SKDZoneCommandType SelectedCommand
		{
			get { return _selectedCommand; }
			set
			{
				_selectedCommand = value;
				ControlSKDZoneArguments.SKDZoneCommandType = value;
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
				ControlSKDZoneArguments.ZoneUid = Guid.Empty;
				if (_selectedZone != null)
				{
					ControlSKDZoneArguments.ZoneUid = _selectedZone.SKDZone.UID;
				}
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => SelectedZone);
			}
		}
		
		public RelayCommand SelectZoneCommand { get; private set; }
		private void OnSelectZone()
		{
			var zoneSelectationViewModel = new SKDZoneSelectionViewModel(SelectedZone != null ? SelectedZone.SKDZone : null);
			if (DialogService.ShowModalWindow(zoneSelectationViewModel))
			{
				SelectedZone = zoneSelectationViewModel.SelectedZone;
			}
		}

		public void UpdateContent()
		{
			if (ControlSKDZoneArguments.ZoneUid != Guid.Empty)
			{
				var zone = SKDManager.Zones.FirstOrDefault(x => x.UID == ControlSKDZoneArguments.ZoneUid);
				SelectedZone = zone != null ? new ZoneViewModel(zone) : null;
				SelectedCommand = ControlSKDZoneArguments.SKDZoneCommandType;
			}
		}

		public string Description
		{
			get { return ""; }
		}
	}
}
