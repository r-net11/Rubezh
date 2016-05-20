using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.ViewModels;
using Integration.OPC.Properties;
using Microsoft.Practices.Prism;
using StrazhAPI.SKD;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using OPCSettings = Integration.OPC.Models.OPCSettings;
using OPCZone = Integration.OPC.Models.OPCZone;

namespace Integration.OPC.ViewModels
{
	public class ZonesOPCViewModel : MenuViewPartViewModel
	{
		public void Initialize(IEnumerable<OPCZone> existingZones)
		{
			ZonesOPC = new ObservableCollection<OPCZone>(existingZones);
			Menu = new MenuViewModel(this);
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, () => SelectedZoneOPC != null);
			EditCommand = new RelayCommand(OnEdit, () => SelectedZoneOPC != null);
			SettingsCommand = new RelayCommand(OnSettings);
		}

		public ObservableCollection<OPCZone> ZonesOPC { get; set; }

		private OPCZone _selectedZoneOPC;

		public OPCZone SelectedZoneOPC
		{
			get { return _selectedZoneOPC; }
			set
			{
				if (_selectedZoneOPC == value) return;
				_selectedZoneOPC = value;
				OnPropertyChanged(() => SelectedZoneOPC);
			}
		}

		public RelayCommand AddCommand { get; set; }
		public RelayCommand DeleteCommand { get; set; }
		public RelayCommand EditCommand { get; set; }
		public RelayCommand SettingsCommand { get; set; }

		public void OnAdd()
		{
			var zones = FiresecManager.FiresecService.GetOPCZones();
			var addZoneDialog = new AddZoneDialogViewModel(zones.Result.Select(x => new OPCZone(x, ZonesOPC.Any(existingZone => existingZone.No == x.No))));

			if (DialogService.ShowModalWindow(addZoneDialog))
			{
				ZonesOPC.AddRange(addZoneDialog.Zones.Where(x => x.IsChecked && x.IsEnabled));
				SKDManager.SKDConfiguration.OPCZones.AddRange(addZoneDialog.Zones.Where(x => x.IsChecked && x.IsEnabled).Select(x => x.ToDTO()));
				ServiceFactory.SaveService.SKDChanged = true;
			}
		}

		public void OnDelete()
		{
			if (!MessageBoxService.ShowConfirmation(string.Format(Resources.MessageRemoveOPCZoneContent, SelectedZoneOPC.Name))) return;

			//TODO: Replace this comment by remove from server command. If remove result is True than go next.

			ZonesOPC.Remove(SelectedZoneOPC);
			SelectedZoneOPC = ZonesOPC.FirstOrDefault();
			ServiceFactory.SaveService.SKDChanged = true;
		}

		public void OnEdit()
		{
			//TODO: Edit selected integration Zone
		}

		public void OnSettings()
		{
			var settingsView = new SettingsViewModel(SKDManager.SKDConfiguration.OPCSettings.IsActive)
			{
				Settings = new OPCSettings(SKDManager.SKDConfiguration.OPCSettings)
			};

			if (DialogService.ShowModalWindow(settingsView))
			{
				SKDManager.SKDConfiguration.OPCSettings = settingsView.Settings.ToDTO();
				ServiceFactory.SaveService.SKDChanged = true;
			}
		}
	}
}
