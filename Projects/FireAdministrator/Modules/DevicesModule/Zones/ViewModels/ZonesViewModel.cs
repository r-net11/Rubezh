using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using DevicesModule.Views;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrastructure.ViewModels;

namespace DevicesModule.ViewModels
{
	public class ZonesViewModel : MenuViewPartViewModel, IEditingViewModel, ISelectable<int>
	{
		public ZoneDevicesViewModel ZoneDevices { get; set; }

		public ZonesViewModel()
		{
			Menu = new ZonesMenuViewModel(this);
			AddCommand = new RelayCommand(OnAdd);
            DeleteCommand = new RelayCommand(OnDelete, CanDelete);
			EditCommand = new RelayCommand(OnEdit, CanEditDelete);
			DeleteAllEmptyCommand = new RelayCommand(OnDeleteAllEmpty, CanDeleteAll);
			ZoneDevices = new ZoneDevicesViewModel();
		}

		public void Initialize()
		{
			Zones = new ObservableCollection<ZoneViewModel>(
				from zone in FiresecManager.Zones
				orderby zone.No
				select new ZoneViewModel(zone));
			SelectedZone = Zones.FirstOrDefault();
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
            int count = Zones.Count(zone => zone.IsSelected == true);
            EditingEnabled = (count > 1) ? false : true;
            return EditingEnabled;
		}

        bool CanDelete()
        {
            return SelectedZone!=null;
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
				FiresecManager.Zones.Add(zoneDetailsViewModel.Zone);
				Zones.Add(new ZoneViewModel(zoneDetailsViewModel.Zone));
				ServiceFactory.SaveService.DevicesChanged = true;
				createZoneEventArg.Cancel = false;
				createZoneEventArg.ZoneNo = zoneDetailsViewModel.Zone.No;
			}
			else
			{
				createZoneEventArg.Cancel = true;
				createZoneEventArg.ZoneNo = null;
			}
		}
		public void EditZone(int zoneNo)
		{
			var zoneViewModel = zoneNo == 0 ? null : Zones.FirstOrDefault(x => x.Zone.No == zoneNo);
			if (zoneViewModel != null)
				OnEdit(zoneViewModel.Zone);
		}
		private void OnEdit(Zone zone)
		{
			var zoneDetailsViewModel = new ZoneDetailsViewModel(zone);
			if (DialogService.ShowModalWindow(zoneDetailsViewModel))
			{
				if (SelectedZone != null)
					SelectedZone.Update(zoneDetailsViewModel.Zone);
				ServiceFactory.SaveService.DevicesChanged = true;
			}
		}

        private bool editingEnabled;
        public bool EditingEnabled
	    {
	        get
	        {
                return editingEnabled;
	        }
            set
            {
                editingEnabled = value;
                OnPropertyChanged("EditingEnabled");
            }
	    }
       
		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var zoneDetailsViewModel = new ZoneDetailsViewModel();
			if (DialogService.ShowModalWindow(zoneDetailsViewModel))
			{
				FiresecManager.FiresecConfiguration.AddZone(zoneDetailsViewModel.Zone);
				Zones.Add(new ZoneViewModel(zoneDetailsViewModel.Zone));
				ServiceFactory.SaveService.DevicesChanged = true;
			}
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
		    string title = "Вы уверены, что хотите удалить зону " + SelectedZone.Zone.PresentationName + "?";
            if(Zones.Count(zone => zone.IsSelected == true) > 1)
                title = "Вы уверены, что хотите удалить выбранные зоны?";
			var dialogResult = MessageBoxService.ShowQuestion(title);
			if (dialogResult == MessageBoxResult.Yes)
			{
			    var tempZones = new ObservableCollection<ZoneViewModel>(Zones);
                foreach (var zoneViewModel in Zones)
                {
                    if (zoneViewModel.IsSelected)
                    {
                        FiresecManager.Zones.Remove(zoneViewModel.Zone);
                        FiresecManager.Devices.ForEach(
                            x =>
                            { if ((x.ZoneNo != null) && (x.ZoneNo.Value == zoneViewModel.Zone.No)) x.ZoneNo = null; });
                        tempZones.Remove(zoneViewModel);
                    }
                }
                Zones = new ObservableCollection<ZoneViewModel>(tempZones);
                SelectedZone = Zones.FirstOrDefault();
                ZoneDevices.UpdateAvailableDevices();
                ServiceFactory.SaveService.DevicesChanged = true;
                FiresecManager.FiresecConfiguration.InvalidateConfiguration();
			}
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			OnEdit(SelectedZone.Zone);
		}

		public RelayCommand DeleteAllEmptyCommand { get; private set; }
		void OnDeleteAllEmpty()
		{
			var dialogResult = MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить все пустые зоны ?");
			if (dialogResult == MessageBoxResult.Yes)
			{
				var emptyZones = new List<ZoneViewModel>(
					Zones.Where(zone => FiresecManager.Devices.Any(x => x.Driver.IsZoneDevice && x.ZoneNo == zone.Zone.No) == false)
				);
				foreach (var emptyZone in emptyZones)
				{
					FiresecManager.Zones.Remove(emptyZone.Zone);
					Zones.Remove(emptyZone);
				}

				SelectedZone = Zones.FirstOrDefault();

				ServiceFactory.SaveService.DevicesChanged = true;
				FiresecManager.FiresecConfiguration.InvalidateConfiguration();
			}
		}

		public override void OnShow()
		{
			base.OnShow();
			SelectedZone = SelectedZone;
			if (ZonesMenuView.Current != null)
				ZonesMenuView.Current.AcceptKeyboard = true;
		}

		public override void OnHide()
		{
			base.OnHide();
			if (ZonesMenuView.Current != null)
				ZonesMenuView.Current.AcceptKeyboard = false;
		}

		#region ISelectable<int> Members

		public void Select(int zoneNo)
		{
			if (zoneNo != 0)
				SelectedZone = Zones.FirstOrDefault(x => x.Zone.No == zoneNo);
		}

		#endregion
	}
}