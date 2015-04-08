using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using GKModule.Events;
using GKModule.Plans;
using KeyboardKey = System.Windows.Input.Key;
using FiresecClient;
using FiresecAPI.GK;

namespace GKModule.ViewModels
{
	public class SKDZonesViewModel : MenuViewPartViewModel, IEditingViewModel, ISelectable<Guid>
	{
		public static SKDZonesViewModel Current { get; private set; }
		bool _lockSelection;

		public SKDZonesViewModel()
		{
			_lockSelection = false;
			Menu = new SKDZonesMenuViewModel(this);
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
			Zones = new ObservableCollection<SKDZoneViewModel>();
			foreach (var zone in GKManager.SKDZones.OrderBy(x => x.No))
			{
				var zoneViewModel = new SKDZoneViewModel(zone);
				Zones.Add(zoneViewModel);
			}
			SelectedZone = Zones.FirstOrDefault();
		}

		ObservableCollection<SKDZoneViewModel> _zones;
		public ObservableCollection<SKDZoneViewModel> Zones
		{
			get { return _zones; }
			set
			{
				_zones = value;
				OnPropertyChanged(() => Zones);
			}
		}

		SKDZoneViewModel _selectedZone;
		public SKDZoneViewModel SelectedZone
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

		bool CanEditRemove()
		{
			return SelectedZone != null;
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			OnAddResult();
		}
		SKDZoneDetailsViewModel OnAddResult()
		{
			var zoneDetailsViewModel = new SKDZoneDetailsViewModel();
			if (DialogService.ShowModalWindow(zoneDetailsViewModel))
			{
				GKManager.SKDZones.Add(zoneDetailsViewModel.Zone);
				var zoneViewModel = new SKDZoneViewModel(zoneDetailsViewModel.Zone);
				Zones.Add(zoneViewModel);
				SelectedZone = zoneViewModel;
				ServiceFactory.SaveService.GKChanged = true;
				GKPlanExtension.Instance.Cache.BuildSafe<GKSKDZone>();
				return zoneDetailsViewModel;
			}
			return null;
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить зону " + SelectedZone.Zone.PresentationName))
			{
				var index = Zones.IndexOf(SelectedZone);
				GKManager.RemoveSKDZone(SelectedZone.Zone);
				Zones.Remove(SelectedZone);
				index = Math.Min(index, Zones.Count - 1);
				if (index > -1)
					SelectedZone = Zones[index];
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			OnEdit(SelectedZone.Zone);
		}
		void OnEdit(GKSKDZone zone)
		{
			var zoneDetailsViewModel = new SKDZoneDetailsViewModel(zone);
			if (DialogService.ShowModalWindow(zoneDetailsViewModel))
			{
				GKManager.EditSKDZone(SelectedZone.Zone);
				SelectedZone.Update(zoneDetailsViewModel.Zone);
				ServiceFactory.SaveService.GKChanged = true;
			}
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
			ServiceFactory.Events.GetEvent<ElementAddedEvent>().Unsubscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementRemovedEvent>().Unsubscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementChangedEvent>().Subscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementSelectedEvent>().Unsubscribe(OnElementSelected);

			ServiceFactory.Events.GetEvent<ElementAddedEvent>().Subscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementRemovedEvent>().Subscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementChangedEvent>().Subscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementSelectedEvent>().Subscribe(OnElementSelected);
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
		void SetRibbonItems()
		{
			RibbonItems = new List<RibbonMenuItemViewModel>()
			{
				new RibbonMenuItemViewModel("Редактирование", new ObservableCollection<RibbonMenuItemViewModel>()
				{
					new RibbonMenuItemViewModel("Добавить", "BAdd"),
					new RibbonMenuItemViewModel("Редактировать", "BEdit"),
					new RibbonMenuItemViewModel("Удалить", "BDelete"),
				}, "BEdit") { Order = 1 }
			};
		}

		public void CreateZone(CreateGKSKDZoneEventArg createZoneEventArg)
		{
			SKDZoneDetailsViewModel result = OnAddResult();
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
				OnEdit(zoneViewModel.Zone);
		}
	}
}