using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecClient;
using GKModule.Events;
using GKModule.Plans.Designer;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using KeyboardKey = System.Windows.Input.Key;

namespace GKModule.ViewModels
{
	public class GuardZonesViewModel : MenuViewPartViewModel, IEditingViewModel, ISelectable<Guid>
	{
		private bool _lockSelection;
		public static GuardZonesViewModel Current { get; private set; }
		public GuardZoneDevicesViewModel ZoneDevices { get; set; }

		public GuardZonesViewModel()
		{
			Menu = new GuardZonesMenuViewModel(this);
			Current = this;
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanEditDelete);
			DeleteAllEmptyCommand = new RelayCommand(OnDeleteAllEmpty, CanDeleteAllEmpty);
			EditCommand = new RelayCommand(OnEdit, CanEditDelete);
			ZoneDevices = new GuardZoneDevicesViewModel();
			RegisterShortcuts();
			IsRightPanelEnabled = true;
			SubscribeEvents();
			SetRibbonItems();
		}

		public void Initialize()
		{
			Zones = new ObservableCollection<GuardZoneViewModel>();
			foreach (var guardZone in XManager.DeviceConfiguration.GuardZones)
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
				OnPropertyChanged("Zones");
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
				else
				{
					ZoneDevices.Clear();
				}
				OnPropertyChanged("SelectedZone");
				if (!_lockSelection && _selectedZone != null && _selectedZone.Zone.PlanElementUIDs.Count > 0)
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
				XManager.DeviceConfiguration.GuardZones.Add(guardZoneDetailsViewModel.Zone);
				var zoneViewModel = new GuardZoneViewModel(guardZoneDetailsViewModel.Zone);
				Zones.Add(zoneViewModel);
				SelectedZone = zoneViewModel;
				ServiceFactory.SaveService.GKChanged = true;
				Helper.BuildMap();
				return guardZoneDetailsViewModel;
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
				XManager.DeviceConfiguration.GuardZones.Remove(SelectedZone.Zone);
				Zones.Remove(SelectedZone);
				index = Math.Min(index, Zones.Count - 1);
				if (index > -1)
					SelectedZone = Zones[index];
				ZoneDevices.UpdateAvailableDevices();
				ServiceFactory.SaveService.GKChanged = true;
				Helper.BuildMap();
			}
		}

		public RelayCommand DeleteAllEmptyCommand { get; private set; }
		void OnDeleteAllEmpty()
		{
			var dialogResult = MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить все пустые зоны ?");
			if (dialogResult == MessageBoxResult.Yes)
			{
				//var emptyZones = Zones.Where(x => x.Zone.Devices.Count == 0).ToList();
				//foreach (var emptyZone in emptyZones)
				//{
				//	XManager.RemoveZone(emptyZone.Zone);
				//	Zones.Remove(emptyZone);
				//}
				SelectedZone = Zones.FirstOrDefault();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		bool CanDeleteAllEmpty()
		{
			return false;// Zones.Any(x => x.Zone.Devices.Count == 0);
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			OnEdit(SelectedZone.Zone);
		}
		void OnEdit(XGuardZone zone)
		{
			var guardZoneDetailsViewModel = new GuardZoneDetailsViewModel(zone);
			if (DialogService.ShowModalWindow(guardZoneDetailsViewModel))
			{
				//XManager.EditZone(SelectedZone.Zone);
				SelectedZone.Update(guardZoneDetailsViewModel.Zone);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public void CreateZone(CreateXGuardZoneEventArg createGuardZoneEventArg)
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
				createGuardZoneEventArg.ZoneUID = result.Zone.BaseUID;
				createGuardZoneEventArg.Zone = result.Zone;
			}
		}
		public void EditZone(Guid zoneUID)
		{
			var zoneViewModel = zoneUID == Guid.Empty ? null : Zones.FirstOrDefault(x => x.Zone.BaseUID == zoneUID);
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
				SelectedZone = Zones.FirstOrDefault(x => x.Zone.BaseUID == zoneUID);
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
			var zone = Zones.FirstOrDefault(x => x.Zone.BaseUID == zoneUID);
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
			elements.OfType<ElementRectangleXGuardZone>().ToList().ForEach(element => Helper.ResetXGuardZone(element));
			elements.OfType<ElementPolygonXGuardZone>().ToList().ForEach(element => Helper.ResetXGuardZone(element));
			OnElementChanged(elements);
		}
		private void OnElementChanged(List<ElementBase> elements)
		{
			Guid guid = Guid.Empty;
			_lockSelection = true;
			elements.ForEach(element =>
			{
				var elementZone = GetElementXGuardZone(element);
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
			var elementZone = GetElementXGuardZone(element);
			if (elementZone != null)
			{
				_lockSelection = true;
				Select(elementZone.ZoneUID);
				_lockSelection = false;
			}
		}
		private IElementZone GetElementXGuardZone(ElementBase element)
		{
			IElementZone elementZone = element as ElementRectangleXGuardZone;
			if (elementZone == null)
				elementZone = element as ElementPolygonXGuardZone;
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
					new RibbonMenuItemViewModel("Удалить все пустые зоны", DeleteAllEmptyCommand, "/Controls;component/Images/BDeleteEmpty.png"),
				}, "/Controls;component/Images/BEdit.png") { Order = 2 }
			};
		}
	}
}