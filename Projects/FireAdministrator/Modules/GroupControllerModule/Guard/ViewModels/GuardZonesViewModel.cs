using GKModule.Events;
using GKModule.Plans;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Plans.Events;
using Infrastructure.ViewModels;
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
	public class GuardZonesViewModel : MenuViewPartViewModel, ISelectable<Guid>
	{
		bool _lockSelection = false;
		public GuardZoneDevicesViewModel ZoneDevices { get; set; }

		public GuardZonesViewModel()
		{
			AddCommand = new RelayCommand(OnAdd);
			EditCommand = new RelayCommand(OnEdit, CanEditDelete);
			DeleteCommand = new RelayCommand(OnDelete, CanEditDelete);
			DeleteAllEmptyCommand = new RelayCommand(OnDeleteAllEmpty, CanDeleteAllEmpty);
			ZoneDevices = new GuardZoneDevicesViewModel();
			ShowSettingsCommand = new RelayCommand(OnShowSettings);
			ShowDependencyItemsCommand = new RelayCommand(ShowDependencyItems);

			Menu = new GuardZonesMenuViewModel(this);
			IsRightPanelEnabled = true;
			RegisterShortcuts();
			SubscribeEvents();
			SetRibbonItems();
		}

		public void Initialize()
		{
			Zones = new ObservableCollection<GuardZoneViewModel>();
			foreach (var guardZone in GKManager.DeviceConfiguration.GuardZones.OrderBy(x => x.No))
			{
				var zoneViewModel = new GuardZoneViewModel(guardZone);
				Zones.Add(zoneViewModel);
			}
			SelectedZone = Zones.FirstOrDefault();
		}

		ObservableCollection<GuardZoneViewModel> _zones;
		public ObservableCollection<GuardZoneViewModel> Zones
		{
			get { return _zones; }
			set
			{
				_zones = value;
				OnPropertyChanged(() => Zones);
			}
		}

		GuardZoneViewModel _selectedZone;
		public GuardZoneViewModel SelectedZone
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
		GuardZoneDetailsViewModel OnAddResult()
		{
			var guardZoneDetailsViewModel = new GuardZoneDetailsViewModel();
			if (DialogService.ShowModalWindow(guardZoneDetailsViewModel))
			{
				GKManager.AddGuardZone(guardZoneDetailsViewModel.Zone);
				var zoneViewModel = new GuardZoneViewModel(guardZoneDetailsViewModel.Zone);
				Zones.Add(zoneViewModel);
				SelectedZone = zoneViewModel;
				if (Zones.Count() == 1)
					ZoneDevices.InitializeAvailableDevices(guardZoneDetailsViewModel.Zone);
				ServiceFactory.SaveService.GKChanged = true;
				GKPlanExtension.Instance.Cache.BuildSafe<GKGuardZone>();
				return guardZoneDetailsViewModel;
			}
			return null;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			OnEdit(SelectedZone);
		}
		void OnEdit(GuardZoneViewModel zoneViewModel)
		{
			var guardZoneDetailsViewModel = new GuardZoneDetailsViewModel(zoneViewModel.Zone);
			if (DialogService.ShowModalWindow(guardZoneDetailsViewModel))
			{
				GKManager.EditGuardZone(zoneViewModel.Zone);
				zoneViewModel.Update();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить охранную зону " + SelectedZone.Zone.PresentationName + " ?"))
			{
				var index = Zones.IndexOf(SelectedZone);
				GKManager.RemoveGuardZone(SelectedZone.Zone);
				if (SelectedZone.Zone.GuardZoneDevices.Count() > 0 && Zones.Count() != 1)
				{
					SelectedZone.Zone.GuardZoneDevices.Clear();
					ZoneDevices.InitializeAvailableDevices(SelectedZone.Zone);
				}
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
					GKManager.RemoveGuardZone(x.Zone);
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

		List<GuardZoneViewModel> GetEmptyZones()
		{
			return Zones.Where(x => !x.Zone.GuardZoneDevices.Any()).ToList();
		}
		public RelayCommand ShowSettingsCommand { get; private set; }
		void OnShowSettings()
		{
			var zonesSettingsViewModel = new ZonesSettingsViewModel(true);
			DialogService.ShowModalWindow(zonesSettingsViewModel);
			if (SelectedZone != null)
				ZoneDevices.InitializeAvailableDevices(SelectedZone.Zone);
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

		public void CreateZone(CreateGKGuardZoneEventArg createGuardZoneEventArg)
		{
			GuardZoneDetailsViewModel result = OnAddResult();
			if (result == null)
			{
				createGuardZoneEventArg.Cancel = true;
				createGuardZoneEventArg.ZoneUID = Guid.Empty;
			}
			else
			{
				createGuardZoneEventArg.Cancel = false;
				createGuardZoneEventArg.ZoneUID = result.Zone.UID;
				createGuardZoneEventArg.Zone = result.Zone;
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
				ZoneDevices.InitializeAvailableDevices(SelectedZone.Zone);
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

		public void LockedSelect(Guid zoneUID)
		{
			_lockSelection = true;
			Select(zoneUID);
			_lockSelection = false;
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
				var elementZone = GetElementGuardZone(element);
				if (elementZone != null)
				{
					OnZoneChanged(elementZone.ZoneUID);
				}
			});
			_lockSelection = false;
		}
		private void OnElementSelected(ElementBase element)
		{
			var elementZone = GetElementGuardZone(element);
			if (elementZone != null)
			{
				_lockSelection = true;
				Select(elementZone.ZoneUID);
				_lockSelection = false;
			}
		}
		private IElementZone GetElementGuardZone(ElementBase element)
		{
			IElementZone elementZone = element as ElementRectangleGKGuardZone;
			if (elementZone == null)
				elementZone = element as ElementPolygonGKGuardZone;
			return elementZone;
		}
	}
}