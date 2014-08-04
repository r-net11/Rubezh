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
	public class ControlGKFireZoneStepViewModel: BaseViewModel, IStepViewModel
	{
		ControlGKFireZoneArguments ControlGKFireZoneArguments { get; set; }
		public ControlGKFireZoneStepViewModel(ControlGKFireZoneArguments controlGKFireZoneArguments)
		{
			ControlGKFireZoneArguments = controlGKFireZoneArguments;
			Commands = new ObservableCollection<ZoneCommandType> { ZoneCommandType.Ignore, ZoneCommandType.ResetIgnore, ZoneCommandType.ResetIgnore };
			OnPropertyChanged(() => Commands);
			SelectZoneCommand = new RelayCommand(OnSelectZone);
			UpdateContent();
		}

		public ObservableCollection<ZoneCommandType> Commands { get; private set; }

		ZoneCommandType _selectedCommand;
		public ZoneCommandType SelectedCommand
		{
			get { return _selectedCommand; }
			set
			{
				_selectedCommand = value;
				ControlGKFireZoneArguments.ZoneCommandType = value;
				OnPropertyChanged(()=>SelectedCommand);
			}
		}

		ZoneViewModel _selectedZone;
		public ZoneViewModel SelectedZone
		{
			get { return _selectedZone; }
			set
			{
				_selectedZone = value;
				ControlGKFireZoneArguments.ZoneUid = Guid.Empty;
				if (_selectedZone != null)
				{
					ControlGKFireZoneArguments.ZoneUid = _selectedZone.Zone.UID;
				}
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => SelectedZone);
			}
		}
		
		public RelayCommand SelectZoneCommand { get; private set; }
		private void OnSelectZone()
		{
			var zoneSelectationViewModel = new ZoneSelectionViewModel(SelectedZone != null ? SelectedZone.Zone : null);
			if (DialogService.ShowModalWindow(zoneSelectationViewModel))
			{
				SelectedZone = zoneSelectationViewModel.SelectedZone;
			}
		}

		public void UpdateContent()
		{
			if (ControlGKFireZoneArguments.ZoneUid != Guid.Empty)
			{
				var zone = XManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.UID == ControlGKFireZoneArguments.ZoneUid);
				SelectedZone = zone != null ? new ZoneViewModel(zone) : null;
				SelectedCommand = ControlGKFireZoneArguments.ZoneCommandType;
			}
		}

		public string Description
		{
			get { return ""; }
		}
	}
}
