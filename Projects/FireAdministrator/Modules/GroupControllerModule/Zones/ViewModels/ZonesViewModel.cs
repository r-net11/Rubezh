using GKModule.Events;
using GKModule.Plans;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Ribbon;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using Infrastructure.ViewModels;
using Infrustructure.Plans.Events;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using KeyboardKey = System.Windows.Input.Key;

namespace GKModule.ViewModels
{
	public class ZonesViewModel : MenuViewPartViewModel, ISelectable<Guid>
	{
		bool _lockSelection = false;
		public ZoneDevicesViewModel ZoneDevices { get; set; }

		public ZonesViewModel()
		{
			AddCommand = new RelayCommand(OnAdd);
			EditCommand = new RelayCommand(OnEdit, CanEditDelete);
			DeleteCommand = new RelayCommand(OnDelete, CanEditDelete);
			DeleteAllEmptyCommand = new RelayCommand(OnDeleteAllEmpty, CanDeleteAllEmpty);
			ShowSettingsCommand = new RelayCommand(OnShowSettings);
			ShowDependencyItemsCommand = new RelayCommand(ShowDependencyItems);

			Menu = new ZonesMenuViewModel(this);
			ZoneDevices = new ZoneDevicesViewModel();
			IsRightPanelEnabled = true;
			RegisterShortcuts();
			SubscribeEvents();
			SetRibbonItems();
		}

		public void Initialize()
		{
			Zones = new ObservableCollection<ZoneViewModel>();
			foreach (var zone in GKManager.DeviceConfiguration.SortedZones)
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
				{
					ZoneDevices.Initialize(value.Zone);
				}
				OnPropertyChanged(() => SelectedZone);
				if (!_lockSelection && _selectedZone != null && _selectedZone.Zone.PlanElementUIDs != null && _selectedZone.Zone.PlanElementUIDs.Count > 0)
					ServiceFactory.Events.GetEvent<FindElementEvent>().Publish(_selectedZone.Zone.PlanElementUIDs);
			}
		}

		bool CanEditDelete()
		{
			return SelectedZone != null;
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			OnAddResult();
		}
		ZoneDetailsViewModel OnAddResult()
		{
			var zoneDetailsViewModel = new ZoneDetailsViewModel();
			if (ServiceFactory.DialogService.ShowModalWindow(zoneDetailsViewModel))
			{
				GKManager.AddZone(zoneDetailsViewModel.Zone);
				var zoneViewModel = new ZoneViewModel(zoneDetailsViewModel.Zone);
				Zones.Add(zoneViewModel);
				if (Zones.Count() == 1)
					ZoneDevices.InitializeAvailableDevice();
				SelectedZone = zoneViewModel;
				ServiceFactory.SaveService.GKChanged = true;
				GKPlanExtension.Instance.Cache.BuildSafe<GKZone>();
				return zoneDetailsViewModel;
			}
			return null;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			OnEdit(SelectedZone);
		}
		void OnEdit(ZoneViewModel zoneViewModel)
		{
			var zoneDetailsViewModel = new ZoneDetailsViewModel(zoneViewModel.Zone);
			if (DialogService.ShowModalWindow(zoneDetailsViewModel))
			{
				GKManager.EditZone(zoneViewModel.Zone);
				zoneViewModel.Update();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			if (ServiceFactory.MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить зону " + SelectedZone.Zone.PresentationName + " ?"))
			{
				var index = Zones.IndexOf(SelectedZone);
				GKManager.RemoveZone(SelectedZone.Zone);
				if (SelectedZone.Zone.Devices.Count() > 0 && Zones.Count() != 1)
					ZoneDevices.InitializeAvailableDevice();
				Zones.Remove(SelectedZone);
				index = Math.Min(index, Zones.Count - 1);
				if (index > -1)
					SelectedZone = Zones[index];
				if (Zones.Count() == 0)
					ZoneDevices.Clear();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand DeleteAllEmptyCommand { get; private set; }
		void OnDeleteAllEmpty()
		{
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить все пустые зоны ?"))
			{
				GetEmptyZones().ForEach(x =>
					{
						GKManager.RemoveZone(x.Zone);
						Zones.Remove(x);
					});
				SelectedZone = Zones.FirstOrDefault();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		bool CanDeleteAllEmpty()
		{
			return GetEmptyZones().Any();
		}
		List<ZoneViewModel> GetEmptyZones()
		{
			return Zones.Where(x => !x.Zone.Devices.Any()).ToList();
		}

		public RelayCommand ShowSettingsCommand { get; private set; }
		void OnShowSettings()
		{
			var zonesSettingsViewModel = new ZonesSettingsViewModel();
			DialogService.ShowModalWindow(zonesSettingsViewModel);
			if (SelectedZone != null)
				ZoneDevices.InitializeAvailableDevice();
		}

		public RelayCommand ShowDependencyItemsCommand { get; set; }

		void ShowDependencyItems()
		{
			if (SelectedZone.Zone != null)
			{
				var dependencyItemsViewModel = new DependencyItemsViewModel(SelectedZone.Zone.OutputDependentElements);
				DialogService.ShowModalWindow(dependencyItemsViewModel);
			}
		}

		public void CreateZone(CreateGKZoneEventArg createZoneEventArg)
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
				createZoneEventArg.ZoneUID = result.Zone.UID;
				createZoneEventArg.Zone = result.Zone;
			}
		}
		public void EditZone(Guid zoneUID)
		{
			var zoneViewModel = zoneUID == Guid.Empty ? null : Zones.FirstOrDefault(x => x.Zone.UID == zoneUID);
			if (zoneViewModel != null)
				OnEdit(zoneViewModel);
		}
		public override void OnShow()
		{
			base.OnShow();
			SelectedZone = SelectedZone;
			if (SelectedZone != null)
				ZoneDevices.InitializeAvailableDevice();
			else
				ZoneDevices.Clear();
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
			ServiceFactory.Events.GetEvent<ElementRemovedEvent>().Unsubscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementChangedEvent>().Unsubscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementSelectedEvent>().Unsubscribe(OnElementSelected);

			ServiceFactory.Events.GetEvent<ElementAddedEvent>().Subscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementRemovedEvent>().Subscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementChangedEvent>().Subscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementSelectedEvent>().Subscribe(OnElementSelected);
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
		private void OnElementChanged(List<ElementBase> elements)
		{
			Guid guid = Guid.Empty;
			_lockSelection = true;
			elements.ForEach(element =>
			{
				var elementZone = GetElementGKZone(element);
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
			var elementZone = GetElementGKZone(element);
			if (elementZone != null)
			{
				_lockSelection = true;
				Select(elementZone.ZoneUID);
				_lockSelection = false;
			}
		}
		private IElementZone GetElementGKZone(ElementBase element)
		{
			IElementZone elementZone = element as ElementRectangleGKZone;
			if (elementZone == null)
				elementZone = element as ElementPolygonGKZone;
			return elementZone;
		}
	}
}