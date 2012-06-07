using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using DevicesModule.Views;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;

namespace DevicesModule.ViewModels
{
	public class ZonesViewModel : ViewPartViewModel, IEditingViewModel
	{
		public ZoneDevicesViewModel ZoneDevices { get; set; }

		public ZonesViewModel()
		{
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanEditDelete);
			EditCommand = new RelayCommand(OnEdit, CanEditDelete);
			DeleteAllCommand = new RelayCommand(OnDeleteAll, CanDeleteAll);
			DeleteAllEmptyCommand = new RelayCommand(OnDeleteAllEmpty, CanDeleteAll);
			ZoneDevices = new ZoneDevicesViewModel();
		}

		public void Initialize()
		{
			Zones = new ObservableCollection<ZoneViewModel>(
				from zone in FiresecManager.DeviceConfiguration.Zones
				orderby zone.No
				select new ZoneViewModel(zone));

			if (Zones.Count > 0)
				SelectedZone = Zones[0];
		}

		ObservableCollection<ZoneViewModel> _zones;
		public ObservableCollection<ZoneViewModel> Zones
		{
			get { return _zones; }
			set
			{
				_zones = value;
				OnPropertyChanged("Zones");
			}
		}

		ZoneViewModel _selectedZone;
		public ZoneViewModel SelectedZone
		{
			get { return _selectedZone; }
			set
			{
				_selectedZone = value;
				if (value != null)
					ZoneDevices.Initialize(value.Zone.No);
				else
					ZoneDevices.Clear();

				OnPropertyChanged("SelectedZone");
			}
		}

		bool CanEditDelete()
		{
			return SelectedZone != null;
		}

		bool CanDeleteAll()
		{
			return Zones.Count > 0;
		}

		public void CreateZone(CreateZoneEventArg createZoneEventArg)
		{
			var zoneDetailsViewModel = new ZoneDetailsViewModel();
			if (DialogService.ShowModalWindow(zoneDetailsViewModel))
			{
				FiresecManager.DeviceConfiguration.Zones.Add(zoneDetailsViewModel.Zone);
				createZoneEventArg.ZoneNo = zoneDetailsViewModel.Zone.No;

				ServiceFactory.SaveService.DevicesChanged = true;
			}
			else
			{
				createZoneEventArg.Cancel = true;
				createZoneEventArg.ZoneNo = null;
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var zoneDetailsViewModel = new ZoneDetailsViewModel();
			if (DialogService.ShowModalWindow(zoneDetailsViewModel))
			{
				FiresecManager.DeviceConfiguration.Zones.Add(zoneDetailsViewModel.Zone);
				Zones.Add(new ZoneViewModel(zoneDetailsViewModel.Zone));

				ServiceFactory.SaveService.DevicesChanged = true;
			}
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			var dialogResult = MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить зону " + SelectedZone.Zone.PresentationName);
			if (dialogResult == MessageBoxResult.Yes)
			{
				FiresecManager.DeviceConfiguration.Zones.Remove(SelectedZone.Zone);
				FiresecManager.DeviceConfiguration.Devices.ForEach(x => { if ((x.ZoneNo != null) && (x.ZoneNo.Value == SelectedZone.Zone.No)) x.ZoneNo = null; });
				Zones.Remove(SelectedZone);
				SelectFirstZone();
				ZoneDevices.UpdateAvailableDevices();
				ServiceFactory.SaveService.DevicesChanged = true;
				FiresecManager.InvalidateConfiguration();
			}
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var zoneDetailsViewModel = new ZoneDetailsViewModel(SelectedZone.Zone);
			if (DialogService.ShowModalWindow(zoneDetailsViewModel))
			{
				SelectedZone.Update(zoneDetailsViewModel.Zone);
				ServiceFactory.SaveService.DevicesChanged = true;
			}
		}

		public RelayCommand DeleteAllCommand { get; private set; }
		void OnDeleteAll()
		{
			var dialogResult = MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить все зоны ?");
			if (dialogResult == MessageBoxResult.Yes)
			{
				FiresecManager.DeviceConfiguration.Zones.Clear();
				FiresecManager.DeviceConfiguration.Devices.ForEach(x => x.ZoneNo = null);
				Zones.Clear();
				SelectedZone = null;

				ServiceFactory.SaveService.DevicesChanged = true;
				FiresecManager.InvalidateConfiguration();
			}
		}

		public RelayCommand DeleteAllEmptyCommand { get; private set; }
		void OnDeleteAllEmpty()
		{
			var dialogResult = MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить все пустые зоны ?");
			if (dialogResult == MessageBoxResult.Yes)
			{
				var emptyZones = new List<ZoneViewModel>(
					Zones.Where(zone => FiresecManager.DeviceConfiguration.Devices.Any(x => x.Driver.IsZoneDevice && x.ZoneNo == zone.Zone.No) == false)
				);
				foreach (var emptyZone in emptyZones)
				{
					FiresecManager.DeviceConfiguration.Zones.Remove(emptyZone.Zone);
					Zones.Remove(emptyZone);
				}

				SelectFirstZone();

				ServiceFactory.SaveService.DevicesChanged = true;
				FiresecManager.InvalidateConfiguration();
			}
		}

		void SelectFirstZone()
		{
			if (Zones.Count > 0)
				SelectedZone = Zones[0];
			else
				SelectedZone = null;
		}

		public override void OnShow()
		{
			SelectedZone = SelectedZone;
			ServiceFactory.Layout.ShowMenu(new ZonesMenuViewModel(this));

			if (ZonesMenuView.Current != null)
				ZonesMenuView.Current.AcceptKeyboard = true;

		}

		public override void OnHide()
		{
			ServiceFactory.Layout.ShowMenu(null);

			if (ZonesMenuView.Current != null)
				ZonesMenuView.Current.AcceptKeyboard = false;
		}
	}
}