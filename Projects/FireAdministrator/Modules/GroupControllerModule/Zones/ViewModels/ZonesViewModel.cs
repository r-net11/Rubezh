using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrastructure.ViewModels;
using XFiresecAPI;
using System.Windows.Input;
using KeyboardKey = System.Windows.Input.Key;

namespace GKModule.ViewModels
{
    public class ZonesViewModel : MenuViewPartViewModel, IEditingViewModel, ISelectable<Guid>
    {
        public static ZonesViewModel Current { get; private set; }
        public ZoneDevicesViewModel ZoneDevices { get; set; }

        public ZonesViewModel()
        {
			Menu = new ZonesMenuViewModel(this);
            Current = this;
            AddCommand = new RelayCommand(OnAdd);
            DeleteCommand = new RelayCommand(OnDelete, CanEditDelete);
            EditCommand = new RelayCommand(OnEdit, CanEditDelete);
            ZoneDevices = new ZoneDevicesViewModel();
            RegisterShortcuts();
        }

        public void Initialize()
        {
            Zones = new ObservableCollection<ZoneViewModel>(
                from zone in XManager.DeviceConfiguration.Zones
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
                    ZoneDevices.Initialize(value.XZone);
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

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
			OnAddResult();
        }
		ZoneDetailsViewModel OnAddResult()
		{
			var zoneDetailsViewModel = new ZoneDetailsViewModel();
			if (DialogService.ShowModalWindow(zoneDetailsViewModel))
			{
				XManager.AddZone(zoneDetailsViewModel.XZone);
                var zoneViewModel = new ZoneViewModel(zoneDetailsViewModel.XZone);
				Zones.Add(zoneViewModel);
                SelectedZone = zoneViewModel;
				ServiceFactory.SaveService.XDevicesChanged = true;
				return zoneDetailsViewModel;
			}
			return null;
		}

        public RelayCommand DeleteCommand { get; private set; }
        void OnDelete()
        {
            var dialogResult = MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить зону " + SelectedZone.XZone.PresentationName);
            if (dialogResult == MessageBoxResult.Yes)
            {
				XManager.RemoveZone(SelectedZone.XZone);
                Zones.Remove(SelectedZone);
				SelectedZone = Zones.FirstOrDefault();
                ZoneDevices.UpdateAvailableDevices();
                ServiceFactory.SaveService.XDevicesChanged = true;
            }
        }

        public RelayCommand EditCommand { get; private set; }
        void OnEdit()
        {
			OnEdit(SelectedZone.XZone);
		}
		void OnEdit(XZone xzone)
		{
			var zoneDetailsViewModel = new ZoneDetailsViewModel(xzone);
			if (DialogService.ShowModalWindow(zoneDetailsViewModel))
            {
                SelectedZone.XZone = zoneDetailsViewModel.XZone;
                SelectedZone.Update();

                ServiceFactory.SaveService.XDevicesChanged = true;
            }
        }

        public void CreateZone(CreateXZoneEventArg createZoneEventArg)
        {
            ZoneDetailsViewModel result = OnAddResult();
            if (result == null)
            {
                createZoneEventArg.Cancel = true;
                createZoneEventArg.ZoneUID = Guid.Empty;
            }
            else
            {
                createZoneEventArg.Cancel = false;
                createZoneEventArg.ZoneUID = result.XZone.UID;
            }
        }
        public void EditZone(Guid zoneUID)
        {
            var zoneViewModel = zoneUID == Guid.Empty ? null : Zones.FirstOrDefault(x => x.XZone.UID == zoneUID);
            if (zoneViewModel != null)
                OnEdit(zoneViewModel.XZone);
        }

        public override void OnShow()
        {
			base.OnShow();
            SelectedZone = SelectedZone;
        }

        public override void OnHide()
        {
			base.OnHide();
        }

		#region ISelectable<Guid> Members

		public void Select(Guid zoneUID)
		{
			if (zoneUID != Guid.Empty)
				SelectedZone = Zones.FirstOrDefault(x => x.XZone.UID == zoneUID);
		}

		#endregion

        private void RegisterShortcuts()
        {
            RegisterShortcut(new KeyGesture(KeyboardKey.N, ModifierKeys.Control), AddCommand);
            RegisterShortcut(new KeyGesture(KeyboardKey.Delete, ModifierKeys.Control), DeleteCommand);
            RegisterShortcut(new KeyGesture(KeyboardKey.E, ModifierKeys.Control), EditCommand);
        }
	}
}