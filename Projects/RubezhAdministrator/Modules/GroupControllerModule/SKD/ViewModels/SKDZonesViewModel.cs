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
using RubezhAPI.Plans.Elements;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using KeyboardKey = System.Windows.Input.Key;

namespace GKModule.ViewModels
{
	public class SKDZonesViewModel : MenuViewPartViewModel, ISelectable<Guid>
	{
		bool _lockSelection = false;

		public SKDZonesViewModel()
		{
			AddCommand = new RelayCommand(OnAdd);
			EditCommand = new RelayCommand(OnEdit, CanEditRemove);
			DeleteCommand = new RelayCommand(OnDelete, CanEditRemove);
			DeleteAllEmptyCommand = new RelayCommand(OnDeleteAllEmpty, CanDeleteAllEmpty);

			Menu = new SKDZonesMenuViewModel(this);
			IsRightPanelEnabled = true;
			RegisterShortcuts();
			SetRibbonItems();

			ServiceFactory.Events.GetEvent<ElementSelectedEvent>().Subscribe(OnElementSelected);
			ServiceFactory.Events.GetEvent<CreateGKSKDZoneEvent>().Subscribe(CreateZone);
			ServiceFactory.Events.GetEvent<EditGKSKDZoneEvent>().Subscribe(EditZone);
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
				if (_selectedZone != null)
					_selectedZone.Update();
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
			if (GKManager.SKDZones.Count >= 255)
			{
				MessageBoxService.ShowWarning("Невозможно добавить больше 255 зон");
				return;
			}

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

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			OnEdit(SelectedZone);
		}
		void OnEdit(SKDZoneViewModel zoneViewModel)
		{
			var zoneDetailsViewModel = new SKDZoneDetailsViewModel(zoneViewModel.Zone);
			if (DialogService.ShowModalWindow(zoneDetailsViewModel))
			{
				GKManager.EditSKDZone(zoneViewModel.Zone);
				zoneViewModel.Update();
				if (DoorsViewModel.Current.SelectedDoor != null)
					DoorsViewModel.Current.SelectedDoor.Update();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить зону " + SelectedZone.Zone.PresentationName + " ?"))
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

		public RelayCommand DeleteAllEmptyCommand { get; private set; }
		void OnDeleteAllEmpty()
		{
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить все пустые зоны ?"))
			{
				var emptyzone = Zones.Where(x => !GKManager.Doors.Any(y => y.EnterZoneUID == x.Zone.UID) && !GKManager.Doors.Any(y => y.ExitZoneUID == x.Zone.UID));
				if (emptyzone.Any())
				{
					for (var i = emptyzone.Count() - 1; i >= 0; i--)
					{
						GKManager.SKDZones.Remove(emptyzone.ElementAt(i).Zone);
						Zones.Remove(emptyzone.ElementAt(i));
					}
					SelectedZone = Zones.FirstOrDefault();
					ServiceFactory.SaveService.GKChanged = true;
				}
			}
		}

		bool CanDeleteAllEmpty()
		{
			return Zones.Any(x => !GKManager.Doors.Any(y => y.EnterZoneUID == x.Zone.UID) && !GKManager.Doors.Any(y => y.ExitZoneUID == x.Zone.UID));
		}

		void RegisterShortcuts()
		{
			RegisterShortcut(new KeyGesture(KeyboardKey.N, ModifierKeys.Control), AddCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.Delete, ModifierKeys.Control), DeleteCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.E, ModifierKeys.Control), EditCommand);
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
					new RibbonMenuItemViewModel("Удалить все пустые зоны", DeleteAllEmptyCommand, "BDeleteEmpty"),
				}, "BEdit") { Order = 1 }
			};
		}
		void OnElementSelected(ElementBase element)
		{
			var elementZone = element as IElementZone;
			if (elementZone != null)
			{
				_lockSelection = true;
				Select(elementZone.ZoneUID);
				_lockSelection = false;
			}
		}

		public override void OnShow()
		{
			SelectedZone = SelectedZone;
			base.OnShow();
		}

		public void CreateZone(CreateGKSKDZoneEventArg createZoneEventArg)
		{
			SKDZoneDetailsViewModel result = OnAddResult();
			if (result == null)
			{
				createZoneEventArg.Cancel = true;
			}
			else
			{
				createZoneEventArg.Cancel = false;
				createZoneEventArg.Zone = result.Zone;
			}
		}
		public void EditZone(Guid zoneUID)
		{
			var zoneViewModel = zoneUID == Guid.Empty ? null : Zones.FirstOrDefault(x => x.Zone.UID == zoneUID);
			if (zoneViewModel != null)
				OnEdit(zoneViewModel);
		}
	}
}