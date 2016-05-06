using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Common;
using StrazhAPI.Models;
using StrazhAPI.SKD;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using StrazhModule.Events;
using StrazhModule.Plans;
using KeyboardKey = System.Windows.Input.Key;

namespace StrazhModule.ViewModels
{
	public class ZonesViewModel : MenuViewPartViewModel, IEditingViewModel, ISelectable<Guid>
	{
		public static ZonesViewModel Current { get; private set; }
		bool _lockSelection;

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
			Zones = new SortableObservableCollection<ZoneViewModel>();
			foreach (var zone in SKDManager.Zones.OrderBy(x => x.No))
			{
				var zoneViewModel = new ZoneViewModel(zone);
				Zones.Add(zoneViewModel);
			}
			SelectedZone = Zones.FirstOrDefault();
		}

		SortableObservableCollection<ZoneViewModel> _zones;
		public SortableObservableCollection<ZoneViewModel> Zones
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
				OnPropertyChanged(() => SelectedZone);
				UpdateRibbonItems();
				if (!_lockSelection && _selectedZone != null && _selectedZone.Zone.PlanElementUIDs.Count > 0)
					ServiceFactoryBase.Events.GetEvent<FindElementEvent>().Publish(_selectedZone.Zone.PlanElementUIDs);
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

		bool CanEditRemove()
		{
			return SelectedZone != null;
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			OnAddZone();
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			if (!MessageBoxService.ShowConfirmation(String.Format("Вы действительно хотите удалить зону\n\"{0}\"?", SelectedZone.Zone.Name)))
				return;
			var index = Zones.IndexOf(SelectedZone);
			SKDManager.RemoveZone(SelectedZone.Zone);
			Zones.Remove(SelectedZone);
			index = Math.Min(index, Zones.Count - 1);
			if (index > -1)
				SelectedZone = Zones[index];
			ServiceFactory.SaveService.SKDChanged = true;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			OnEdit(SelectedZone);
		}

		void RegisterShortcuts()
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

		void SubscribeEvents()
		{
			ServiceFactoryBase.Events.GetEvent<ElementAddedEvent>().Unsubscribe(OnElementChanged);
			ServiceFactoryBase.Events.GetEvent<ElementRemovedEvent>().Unsubscribe(OnElementChanged);
			ServiceFactoryBase.Events.GetEvent<ElementChangedEvent>().Subscribe(OnElementChanged);
			ServiceFactoryBase.Events.GetEvent<ElementSelectedEvent>().Unsubscribe(OnElementSelected);

			ServiceFactoryBase.Events.GetEvent<ElementAddedEvent>().Subscribe(OnElementChanged);
			ServiceFactoryBase.Events.GetEvent<ElementRemovedEvent>().Subscribe(OnElementChanged);
			ServiceFactoryBase.Events.GetEvent<ElementChangedEvent>().Subscribe(OnElementChanged);
			ServiceFactoryBase.Events.GetEvent<ElementSelectedEvent>().Subscribe(OnElementSelected);
		}
		void OnZoneChanged(Guid zoneUID)
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
		void OnElementChanged(List<ElementBase> elements)
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
		void OnElementSelected(ElementBase element)
		{
			var elementZone = GetElementSKDZone(element);
			if (elementZone != null)
			{
				_lockSelection = true;
				Select(elementZone.ZoneUID);
				_lockSelection = false;
			}
		}
		IElementZone GetElementSKDZone(ElementBase element)
		{
			IElementZone elementZone = element as ElementRectangleSKDZone;
			if (elementZone == null)
				elementZone = element as ElementPolygonSKDZone;
			return elementZone;
		}

		public override void OnShow()
		{
			if(Zones != null)
				Zones.Sort(x => x.Name);

			SelectedZone = SelectedZone;
			base.OnShow();
		}

		protected override void UpdateRibbonItems()
		{
			base.UpdateRibbonItems();
			RibbonItems[0][0].Command = AddCommand;
			RibbonItems[0][1].Command = SelectedZone == null ? null : EditCommand;
			RibbonItems[0][2].Command = SelectedZone == null ? null : DeleteCommand;
		}
		void SetRibbonItems()
		{
			RibbonItems = new List<RibbonMenuItemViewModel>
			{
				new RibbonMenuItemViewModel("Редактирование", new ObservableCollection<RibbonMenuItemViewModel>
				{
					new RibbonMenuItemViewModel("Добавить", "BAdd"),
					new RibbonMenuItemViewModel("Редактировать", "BEdit"),
					new RibbonMenuItemViewModel("Удалить", "BDelete"),
				}, "BEdit") { Order = 1 }
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

		void OnEdit(ZoneViewModel viewModel)
		{
			var zoneDetailsViewModel = new ZoneDetailsViewModel(SelectedZone.Zone);
			if (DialogService.ShowModalWindow(zoneDetailsViewModel))
			{
				SKDManager.EditZone(zoneDetailsViewModel.Zone);
				SelectedZone.Update(zoneDetailsViewModel.Zone);
				ServiceFactory.SaveService.SKDChanged = true;
			}
		}
		ZoneViewModel OnAddZone()
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