using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using FiresecAPI.Models;
using FiresecAPI.SKD;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using SKDModule.Events;
using SKDModule.Plans;
using KeyboardKey = System.Windows.Input.Key;

namespace SKDModule.ViewModels
{
	public class ZonesViewModel : MenuViewPartViewModel, IEditingViewModel, ISelectable<Guid>
	{
		public static ZonesViewModel Current { get; private set; }
		private bool _lockSelection;

		public ZonesViewModel()
		{
			_lockSelection = false;
			Menu = new ZonesMenuViewModel(this);
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanEditRemove);
			EditCommand = new RelayCommand(OnEdit, CanEditRemove);
			Current = this;
			RegisterShortcuts();
			IsRightPanelEnabled = true;
			SubscribeEvents();
			SetRibbonItems();
		}

		public void Initialize()
		{
			Zones = new ObservableCollection<ZoneViewModel>();
			foreach (var zone in SKDManager.Zones)
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

		private ZoneViewModel _selectedZone;
		public ZoneViewModel SelectedZone
		{
			get { return _selectedZone; }
			set
			{
				_selectedZone = value;
				OnPropertyChanged(() => SelectedZone);
				UpdateRibbonItems();
				if (!_lockSelection && _selectedZone != null && _selectedZone.Zone.PlanElementUIDs.Count > 0)
					ServiceFactory.Events.GetEvent<FindElementEvent>().Publish(_selectedZone.Zone.PlanElementUIDs);
			}
		}

		public void Select(Guid zoneUID)
		{
			if (zoneUID != Guid.Empty)
			{
				var zoneViewModel = Zones.FirstOrDefault(x => x.Zone.UID == zoneUID);
				SelectedZone = zoneViewModel;
			}
		}

		private bool CanEditRemove()
		{
			return SelectedZone != null;
		}

		public RelayCommand AddCommand { get; private set; }
		private void OnAdd()
		{
			OnAddZone();
		}

		public RelayCommand DeleteCommand { get; private set; }
		private void OnDelete()
		{
			var index = Zones.IndexOf(SelectedZone);
			SKDManager.Zones.Remove(SelectedZone.Zone);
			SelectedZone.Zone.OnChanged();
			Zones.Remove(SelectedZone);
			index = Math.Min(index, Zones.Count - 1);
			if (index > -1)
				SelectedZone = Zones[index];
			ServiceFactory.SaveService.GKChanged = true;
		}

		public RelayCommand EditCommand { get; private set; }
		private void OnEdit()
		{
			OnEdit(SelectedZone);
		}

		private void RegisterShortcuts()
		{
			RegisterShortcut(new KeyGesture(KeyboardKey.N, ModifierKeys.Control), () =>
			{
				if (SelectedZone != null)
				{
					if (AddCommand.CanExecute(null))
						AddCommand.Execute();
				}
			});
			RegisterShortcut(new KeyGesture(KeyboardKey.Delete, ModifierKeys.Control), () =>
			{
				if (SelectedZone != null)
				{
					if (DeleteCommand.CanExecute(null))
						DeleteCommand.Execute();
				}
			});
			RegisterShortcut(new KeyGesture(KeyboardKey.E, ModifierKeys.Control), () =>
			{
				if (SelectedZone != null)
				{
					if (EditCommand.CanExecute(null))
						EditCommand.Execute();
				}
			});
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
			ServiceFactory.Events.GetEvent<ElementRemovedEvent>().Unsubscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementChangedEvent>().Subscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementSelectedEvent>().Unsubscribe(OnElementSelected);

			ServiceFactory.Events.GetEvent<ElementAddedEvent>().Subscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementRemovedEvent>().Subscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementChangedEvent>().Subscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementSelectedEvent>().Subscribe(OnElementSelected);
		}
		private void OnZoneChanged(Guid zoneUID)
		{
			var zone = Zones.FirstOrDefault(x => x.Zone.UID == zoneUID);
			if (zone != null)
			{
				zone.Update();
				if (!_lockSelection)
				{
					SelectedZone = zone;
				}
			}
		}
		private void OnElementChanged(List<ElementBase> elements)
		{
			_lockSelection = true;
			elements.ForEach(element =>
			{
				var elementZone = GetElementSKDZone(element);
				if (elementZone != null)
					OnZoneChanged(elementZone.ZoneUID);
			});
			_lockSelection = false;
		}
		private void OnElementSelected(ElementBase element)
		{
			var elementZone = GetElementSKDZone(element);
			if (elementZone != null)
			{
				_lockSelection = true;
				Select(elementZone.ZoneUID);
				_lockSelection = false;
			}
		}
		private IElementZone GetElementSKDZone(ElementBase element)
		{
			IElementZone elementZone = element as ElementRectangleSKDZone;
			if (elementZone == null)
				elementZone = element as ElementPolygonSKDZone;
			return elementZone;
		}

		public override void OnShow()
		{
			SelectedZone = SelectedZone;
			base.OnShow();
		}
		public override void OnHide()
		{
			base.OnHide();
		}

		protected override void UpdateRibbonItems()
		{
			base.UpdateRibbonItems();
			RibbonItems[0][0].Command = AddCommand;
			RibbonItems[0][1].Command = SelectedZone == null ? null : EditCommand;
			RibbonItems[0][2].Command = SelectedZone == null ? null : DeleteCommand;
		}
		private void SetRibbonItems()
		{
			RibbonItems = new List<RibbonMenuItemViewModel>()
			{
				new RibbonMenuItemViewModel("Редактирование", new ObservableCollection<RibbonMenuItemViewModel>()
				{
					new RibbonMenuItemViewModel("Добавить", "/Controls;component/Images/BAdd.png"),
					new RibbonMenuItemViewModel("Редактировать", "/Controls;component/Images/BEdit.png"),
					new RibbonMenuItemViewModel("Удалить", "/Controls;component/Images/BDelete.png"),
				}, "/Controls;component/Images/BEdit.png") { Order = 1 }
			};
		}

		public void CreateZone(CreateSKDZoneEventArg createZoneEventArg)
		{
			var zoneViewModel = OnAddZone();
			createZoneEventArg.Zone = zoneViewModel == null ? null : zoneViewModel.Zone;
		}
		public void EditZone(Guid zoneUID)
		{
			var zoneViewModel = zoneUID == Guid.Empty ? null : Zones.FirstOrDefault(x => x.Zone.UID == zoneUID);
			if (zoneViewModel != null)
				OnEdit(zoneViewModel);
		}

		private void OnEdit(ZoneViewModel viewModel)
		{
			var zoneDetailsViewModel = new ZoneDetailsViewModel(SelectedZone.Zone);
			if (DialogService.ShowModalWindow(zoneDetailsViewModel))
			{
				SKDManager.EditZone(zoneDetailsViewModel.Zone);
				SelectedZone.Update(zoneDetailsViewModel.Zone);
				zoneDetailsViewModel.Zone.OnChanged();
				ServiceFactory.SaveService.SKDChanged = true;
			}
		}
		private ZoneViewModel OnAddZone()
		{
			var zoneDetailsViewModel = new ZoneDetailsViewModel();
			if (DialogService.ShowModalWindow(zoneDetailsViewModel))
			{
				SKDManager.Zones.Add(zoneDetailsViewModel.Zone);
				var zoneViewModel = new ZoneViewModel(zoneDetailsViewModel.Zone);
				Zones.Add(zoneViewModel);
				SelectedZone = zoneViewModel;
				ServiceFactory.SaveService.SKDChanged = true;
				SKDPlanExtension.Instance.Cache.BuildSafe<SKDZone>();
				return zoneViewModel;
			}
			return null;
		}
	}
}