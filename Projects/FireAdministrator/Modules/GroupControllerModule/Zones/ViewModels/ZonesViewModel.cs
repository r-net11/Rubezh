using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using FiresecAPI.Models;
using FiresecClient;
using GKModule.Plans.Designer;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrastructure.ViewModels;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using XFiresecAPI;
using KeyboardKey = System.Windows.Input.Key;
using Infrastructure.Common.Ribbon;

namespace GKModule.ViewModels
{
    public class ZonesViewModel : MenuViewPartViewModel, IEditingViewModel, ISelectable<Guid>
    {
		private bool _lockSelection;
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
			IsRightPanelEnabled = true;
			SubscribeEvents();
			SetRibbonItems();
		}

        public void Initialize()
        {
			Zones = new ObservableCollection<ZoneViewModel>();
			foreach (var zone in XManager.DeviceConfiguration.SortedZones)
			{
				var zoneViewModel = new ZoneViewModel(zone);
				Zones.Add(zoneViewModel);
			}
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
                    ZoneDevices.Initialize(value.Zone);
                else
                    ZoneDevices.Clear();
                OnPropertyChanged("SelectedZone");
				if (!_lockSelection && _selectedZone != null && _selectedZone.Zone.PlanElementUIDs.Count > 0)
					ServiceFactory.Events.GetEvent<FindElementEvent>().Publish(_selectedZone.Zone.PlanElementUIDs);
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
				ServiceFactory.SaveService.GKChanged = true;
				Helper.BuildMap();
				return zoneDetailsViewModel;
			}
			return null;
		}

        public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			var dialogResult = MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить зону " + SelectedZone.Zone.PresentationName);
			if (dialogResult == MessageBoxResult.Yes)
			{
				var index = Zones.IndexOf(SelectedZone);
				XManager.RemoveZone(SelectedZone.Zone);
				Zones.Remove(SelectedZone);
				index = Math.Min(index, Zones.Count - 1);
				if (index > -1)
					SelectedZone = Zones[index];
				ZoneDevices.UpdateAvailableDevices();
				ServiceFactory.SaveService.GKChanged = true;
				Helper.BuildMap();
			}
		}

        public RelayCommand EditCommand { get; private set; }
        void OnEdit()
        {
			OnEdit(SelectedZone.Zone);
		}
		void OnEdit(XZone xzone)
		{
			var zoneDetailsViewModel = new ZoneDetailsViewModel(xzone);
			if (DialogService.ShowModalWindow(zoneDetailsViewModel))
            {
				XManager.EditZone(SelectedZone.Zone);
                SelectedZone.Update(zoneDetailsViewModel.XZone);
                ServiceFactory.SaveService.GKChanged = true;
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
				createZoneEventArg.Zone = result.XZone;
            }
        }
        public void EditZone(Guid zoneUID)
        {
            var zoneViewModel = zoneUID == Guid.Empty ? null : Zones.FirstOrDefault(x => x.Zone.UID == zoneUID);
            if (zoneViewModel != null)
                OnEdit(zoneViewModel.Zone);
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
				SelectedZone = Zones.FirstOrDefault(x => x.Zone.UID == zoneUID);
		}

		#endregion

        private void RegisterShortcuts()
        {
            RegisterShortcut(new KeyGesture(KeyboardKey.N, ModifierKeys.Control), AddCommand);
            RegisterShortcut(new KeyGesture(KeyboardKey.Delete, ModifierKeys.Control), DeleteCommand);
            RegisterShortcut(new KeyGesture(KeyboardKey.E, ModifierKeys.Control), EditCommand);
        }
		public void LockedSelect(Guid zoneUID)
		{
			_lockSelection = true;
			Select(zoneUID);
			_lockSelection = false;
		}

		private void SubscribeEvents()
		{
			ServiceFactory.Events.GetEvent<ElementAddedEvent>().Unsubscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementRemovedEvent>().Unsubscribe(OnElementRemoved);
			ServiceFactory.Events.GetEvent<ElementChangedEvent>().Unsubscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementSelectedEvent>().Unsubscribe(OnElementSelected);

			ServiceFactory.Events.GetEvent<ElementAddedEvent>().Subscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementRemovedEvent>().Subscribe(OnElementRemoved);
			ServiceFactory.Events.GetEvent<ElementChangedEvent>().Subscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementSelectedEvent>().Subscribe(OnElementSelected);
		}
		private void OnZoneChanged(Guid zoneUID)
		{
			var zone = Zones.FirstOrDefault(x => x.Zone.UID == zoneUID);
			if (zone != null)
			{
				zone.Update();
				// TODO: FIX IT
				if (!_lockSelection)
					SelectedZone = zone;
			}
		}
		private void OnElementRemoved(List<ElementBase> elements)
		{
			elements.OfType<ElementRectangleXZone>().ToList().ForEach(element => Helper.ResetXZone(element));
			elements.OfType<ElementPolygonXZone>().ToList().ForEach(element => Helper.ResetXZone(element));
			OnElementChanged(elements);
		}
		private void OnElementChanged(List<ElementBase> elements)
		{
			Guid guid = Guid.Empty;
			_lockSelection = true;
			elements.ForEach(element =>
			{
				var elementZone = GetElementXZone(element);
				if (elementZone != null)
				{
					OnZoneChanged(elementZone.ZoneUID);
					//if (guid != Guid.Empty)
					//    OnZoneChanged(guid);
					//guid = elementZone.ZoneUID;
				}
			});
			_lockSelection = false;
			//if (guid != Guid.Empty)
			//    OnZoneChanged(guid);
		}
		private void OnElementSelected(ElementBase element)
		{
			var elementZone = GetElementXZone(element);
			if (elementZone != null)
			{
				_lockSelection = true;
				Select(elementZone.ZoneUID);
				_lockSelection = false;
			}
		}
		private IElementZone GetElementXZone(ElementBase element)
		{
			IElementZone elementZone = element as ElementRectangleXZone;
			if (elementZone == null)
				elementZone = element as ElementPolygonXZone;
			return elementZone;
		}

		private void SetRibbonItems()
		{
			RibbonItems = new List<RibbonMenuItemViewModel>()
			{
					new RibbonMenuItemViewModel("Редактирование", new ObservableCollection<RibbonMenuItemViewModel>()
				{
					new RibbonMenuItemViewModel("Добавить", AddCommand, "/Controls;component/Images/BAdd.png"),
					new RibbonMenuItemViewModel("Редактировать", EditCommand, "/Controls;component/Images/BEdit.png"),
					new RibbonMenuItemViewModel("Удалить", DeleteCommand, "/Controls;component/Images/BDelete.png"),
				}, "/Controls;component/Images/BZones.png") { Order = 2 }
			};
		}
	}
}