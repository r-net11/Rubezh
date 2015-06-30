using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DevicesModule.Plans.Designer;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrastructure.ViewModels;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using KeyboardKey = System.Windows.Input.Key;

namespace DevicesModule.ViewModels
{
	public class ZonesViewModel : MenuViewPartViewModel, IEditingViewModel, ISelectable<Guid>
	{
		public static ZonesViewModel Current { get; private set; }
		private bool _lockSelection;

		public ZonesViewModel()
		{
			_lockSelection = false;
			Current = this;
			Menu = new ZonesMenuViewModel(this);
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanDelete);
			EditCommand = new RelayCommand(OnEdit, CanEditDelete);
			DeleteAllEmptyCommand = new RelayCommand(OnDeleteAllEmpty, CanDeleteAll);
			RegisterShortcuts();
			IsRightPanelEnabled = true;
			IsRightPanelVisible = false;
			SubscribeEvents();
			SetRibbonItems();
		}

		public void Initialize()
		{
			ZoneDevices = new ZoneDevicesViewModel();
			Zones = new ObservableCollection<ZoneViewModel>(
				from zone in FiresecManager.Zones
				orderby zone.No
				select new ZoneViewModel(zone));
			SelectedZone = Zones.FirstOrDefault();
		}

		ZoneDevicesViewModel _zoneDevices;
		public ZoneDevicesViewModel ZoneDevices
		{
			get { return _zoneDevices; }
			set
			{
				_zoneDevices = value;
				OnPropertyChanged(() => ZoneDevices);
			}
		}

		ObservableCollection<ZoneViewModel> _zones;
		public ObservableCollection<ZoneViewModel> Zones
		{
			get { return _zones; }
			set
			{
				_zones = value;
				OnPropertyChanged(() => Zones);
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
				OnPropertyChanged(() => SelectedZone);
				if (!_lockSelection && _selectedZone != null && _selectedZone.Zone.PlanElementUIDs.Count > 0)
					ServiceFactory.Events.GetEvent<FindElementEvent>().Publish(_selectedZone.Zone.PlanElementUIDs);
			}
		}

		bool CanEditDelete()
		{
			return SelectedZone != null;
		}

		bool CanDelete()
		{
			return SelectedZone != null;
		}

		bool CanDeleteAll()
		{
			return Zones.Count > 0;
		}

		public void CreateZone(CreateZoneEventArg createZoneEventArg)
		{
			ZoneDetailsViewModel zoneDetailsViewModel;
			if (createZoneEventArg.Zone != null)
				zoneDetailsViewModel = new ZoneDetailsViewModel(createZoneEventArg.Zone.ZoneType);
			else
				zoneDetailsViewModel = new ZoneDetailsViewModel();
			if (DialogService.ShowModalWindow(zoneDetailsViewModel))
			{
				FiresecManager.Zones.Add(zoneDetailsViewModel.Zone);
				Zones.Add(new ZoneViewModel(zoneDetailsViewModel.Zone));
				ServiceFactory.SaveService.FSChanged = true;
				createZoneEventArg.Cancel = false;
				createZoneEventArg.Zone = zoneDetailsViewModel.Zone;
			}
			else
			{
				createZoneEventArg.Cancel = true;
				createZoneEventArg.Zone = null;
			}
		}
		public void EditZone(Guid zoneUID)
		{
			var zoneViewModel = zoneUID == Guid.Empty ? null : Zones.FirstOrDefault(x => x.Zone.UID == zoneUID);
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
				ServiceFactory.SaveService.FSChanged = true;
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var zoneDetailsViewModel = new ZoneDetailsViewModel();
			if (DialogService.ShowModalWindow(zoneDetailsViewModel))
			{
				FiresecManager.FiresecConfiguration.AddZone(zoneDetailsViewModel.Zone);
				var zoneViewModel = new ZoneViewModel(zoneDetailsViewModel.Zone);
				Zones.Add(zoneViewModel);
				SelectedZone = zoneViewModel;
				ServiceFactory.SaveService.FSChanged = true;
				Helper.BuildMap();
			}
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить зону " + SelectedZone.Zone.PresentationName + "?"))
			{
				var index = Zones.IndexOf(SelectedZone);
				FiresecManager.FiresecConfiguration.RemoveZone(SelectedZone.Zone);
				Zones.Remove(SelectedZone);
				index = Math.Min(index, Zones.Count - 1);
				if (index > -1)
					SelectedZone = Zones[index];
				ServiceFactory.SaveService.FSChanged = true;
				FiresecManager.FiresecConfiguration.InvalidateConfiguration();
				Helper.BuildMap();
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
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить все пустые зоны ?"))
			{
				var emptyZones = new List<ZoneViewModel>(
					Zones.Where(zone => FiresecManager.Devices.Any(x => x.Driver.IsZoneDevice && x.ZoneUID == zone.Zone.UID) == false)
				);
				foreach (var emptyZone in emptyZones)
				{
					FiresecManager.FiresecConfiguration.RemoveZone(emptyZone.Zone);
					Zones.Remove(emptyZone);
				}
				SelectedZone = Zones.FirstOrDefault();
				ServiceFactory.SaveService.FSChanged = true;
			}
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

		public void LockedSelect(Guid zoneUID)
		{
			_lockSelection = true;
			Select(zoneUID);
			_lockSelection = false;
		}
		private void RegisterShortcuts()
		{
			RegisterShortcut(new KeyGesture(KeyboardKey.N, ModifierKeys.Control), AddCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.Delete, ModifierKeys.Control), DeleteCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.E, ModifierKeys.Control), EditCommand);
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
			elements.OfType<ElementRectangleZone>().ToList().ForEach(element => Helper.ResetZone(element));
			elements.OfType<ElementPolygonZone>().ToList().ForEach(element => Helper.ResetZone(element));
			OnElementChanged(elements);
		}
		private void OnElementChanged(List<ElementBase> elements)
		{
			Guid guid = Guid.Empty;
			_lockSelection = true;
			elements.ForEach(element =>
			{
				var elementZone = GetElementZone(element);
				if (elementZone != null)
				{
					OnZoneChanged(elementZone.ZoneUID);
					//if (guid != Guid.Empty)
					//	OnZoneChanged(guid);
					//guid = elementZone.ZoneUID;
				}
			});
			_lockSelection = false;
			//if (guid != Guid.Empty)
			//	OnZoneChanged(guid);
		}
		private void OnElementSelected(ElementBase element)
		{
			var elementZone = GetElementZone(element);
			if (elementZone != null)
			{
				_lockSelection = true;
				Select(elementZone.ZoneUID);
				_lockSelection = false;
			}
		}
		private IElementZone GetElementZone(ElementBase element)
		{
			IElementZone elementZone = element as ElementRectangleZone;
			if (elementZone == null)
				elementZone = element as ElementPolygonZone;
			return elementZone;
		}

		private void SetRibbonItems()
		{
			RibbonItems = new List<RibbonMenuItemViewModel>()
			{
				new RibbonMenuItemViewModel("Редактирование", new ObservableCollection<RibbonMenuItemViewModel>()
				{
					new RibbonMenuItemViewModel("Добавить", AddCommand, "BAdd"),
					new RibbonMenuItemViewModel("Редактировать", EditCommand, "BEdit"),
					new RibbonMenuItemViewModel("Удалить", DeleteCommand, "BDelete"),
					new RibbonMenuItemViewModel("Удалить все пустые зоны", DeleteAllEmptyCommand, "BDeleteEmpty"),
				}, "BEdit") { Order = 2 }
			};
		}
	}
}