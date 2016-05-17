using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.ViewModels;
using Integration.OPC.Models;
using Integration.OPC.Properties;
using StrazhAPI.Enums;
using StrazhAPI.Integration.OPC;
using StrazhAPI.SKD;
using OPCSettings = Integration.OPC.Models.OPCSettings;

namespace Integration.OPC.ViewModels
{
	public class ZonesOPCViewModel : MenuViewPartViewModel
	{
		public void Initialize()
		{
			ZonesOPC = new List<OPCZone> //TODO: Was implemented for testing purposes.
			{
				new OPCZone
				{
					Name = "Архив",
					No = 1,
					Description = "Archive",
					Type = OPCZoneType.Fire
				},
				new OPCZone
				{
					Name = "Архив",
					No = 1,
					Description = "Archive",
					Type = OPCZoneType.Guard
				}
			};
			Menu = new MenuViewModel(this);
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, () => SelectedZoneOPC != null);
			EditCommand = new RelayCommand(OnEdit, () => SelectedZoneOPC != null);
			SettingsCommand = new RelayCommand(OnSettings);
		}

		public List<OPCZone> ZonesOPC { get; set; }

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
			//TODO: Add integration Zone
		}

		public void OnDelete()
		{
			if (!MessageBoxService.ShowConfirmation(string.Format(Resources.ContentRemoveOPCZone, SelectedZoneOPC.Name))) return;


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
			//TODO: Show settings dialog
			var settingsView = new SettingsViewModel
			{
				Settings = new OPCSettings(SKDManager.SKDConfiguration.OPCSettings)
			};

			if (DialogService.ShowModalWindow(settingsView))
			{
				ServiceFactory.SaveService.SKDChanged = true;
			}
		}
	}
}
