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
using GKModule.Plans;
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
	public class DoorsViewModel : MenuViewPartViewModel,ISelectable<Guid>
	{
		private bool _lockSelection;
		public static DoorsViewModel Current { get; private set; }

		public DoorsViewModel()
		{
			Menu = new DoorsMenuViewModel(this);
			Current = this;
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanEditDelete);
			EditCommand = new RelayCommand(OnEdit, CanEditDelete);
			DeleteAllEmptyCommand = new RelayCommand(OnDeleteAllEmpty, CanDeleteAllEmpty);
			ShowDependencyItemsCommand = new RelayCommand(ShowDependencyItems);
			RegisterShortcuts();
			IsRightPanelEnabled = true;
			SubscribeEvents();
			SetRibbonItems();
		}

		public void Initialize()
		{
			Doors = new ObservableCollection<DoorViewModel>();
			foreach (var door in GKManager.DeviceConfiguration.Doors.OrderBy(x => x.No))
			{
				var doorViewModel = new DoorViewModel(door);
				Doors.Add(doorViewModel);
			}
			SelectedDoor = Doors.FirstOrDefault();
		}

		ObservableCollection<DoorViewModel> _doors;
		public ObservableCollection<DoorViewModel> Doors
		{
			get { return _doors; }
			set
			{
				_doors = value;
				OnPropertyChanged(() => Doors);
			}
		}

		DoorViewModel _selectedDoor;
		public DoorViewModel SelectedDoor
		{
			get { return _selectedDoor; }
			set
			{
				_selectedDoor = value;
				if (_selectedDoor != null)
					_selectedDoor.Update();
				OnPropertyChanged(() => SelectedDoor);
				OnPropertyChanged(() => HasSelectedDoor);
				if (!_lockSelection && _selectedDoor != null && _selectedDoor.Door.PlanElementUIDs.Count > 0)
					ServiceFactory.Events.GetEvent<FindElementEvent>().Publish(_selectedDoor.Door.PlanElementUIDs);
			}
		}

		public bool HasSelectedDoor
		{
			get { return SelectedDoor != null; }
		}

		bool CanEditDelete()
		{
			return SelectedDoor != null;
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			OnAddResult();
		}
		DoorDetailsViewModel OnAddResult()
		{
			var doorDetailsViewModel = new DoorDetailsViewModel();
			if (DialogService.ShowModalWindow(doorDetailsViewModel))
			{
				GKManager.DeviceConfiguration.Doors.Add(doorDetailsViewModel.Door);
				var doorViewModel = new DoorViewModel(doorDetailsViewModel.Door);
				Doors.Add(doorViewModel);
				SelectedDoor = doorViewModel;
				ServiceFactory.SaveService.GKChanged = true;
				GKPlanExtension.Instance.Cache.BuildSafe<GKDoor>();
				return doorDetailsViewModel;
			}
			return null;
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить точку доступа " + SelectedDoor.Door.PresentationName))
			{
				var index = Doors.IndexOf(SelectedDoor);
				GKManager.DeviceConfiguration.Doors.Remove(SelectedDoor.Door);
				SelectedDoor.Door.InputDependentElements.ForEach(x =>
				{
					x.OutDependentElements.Remove(SelectedDoor.Door);
				});

				SelectedDoor.Door.OutDependentElements.ForEach(x =>
				{
					x.InputDependentElements.Remove(SelectedDoor.Door);
					x.UpdateLogic();
					x.OnChanged();
				});
				SelectedDoor.Door.OnChanged();
				Doors.Remove(SelectedDoor);
				index = Math.Min(index, Doors.Count - 1);
				if (index > -1)
					SelectedDoor = Doors[index];
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand DeleteAllEmptyCommand { get; private set; }
		void OnDeleteAllEmpty()
		{
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить все пустые ТД ?"))
			{
				var emptyDoors = Doors.Where( x => x.Door.ExitButtonUID == Guid.Empty && x.Door.EnterDeviceUID == Guid.Empty && x.Door.ExitDeviceUID == Guid.Empty && x.Door.EnterButtonUID == Guid.Empty &&  x.Door.ExitZoneUID == Guid.Empty &&
					x.Door.LockDeviceUID == Guid.Empty && x.Door.LockDeviceExitUID == Guid.Empty && x.Door.LockControlDeviceUID == Guid.Empty &&  x.Door.LockControlDeviceExitUID == Guid.Empty && x.Door.EnterZoneUID == Guid.Empty );

				if (emptyDoors.Any())
				{
					for (var i = emptyDoors.Count() - 1; i >= 0; i--)
					{
						GKManager.Doors.Remove(emptyDoors.ElementAt(i).Door);
						Doors.Remove(emptyDoors.ElementAt(i));
					}
					SelectedDoor = Doors.FirstOrDefault();
					ServiceFactory.SaveService.GKChanged = true;
				}
			}
		}

		bool CanDeleteAllEmpty()
		{
			return Doors.Any(x => x.Door.ExitButtonUID == Guid.Empty && x.Door.EnterDeviceUID == Guid.Empty && x.Door.ExitDeviceUID == Guid.Empty && x.Door.EnterButtonUID == Guid.Empty && x.Door.ExitZoneUID == Guid.Empty &&
					x.Door.LockDeviceUID == Guid.Empty && x.Door.LockDeviceExitUID == Guid.Empty && x.Door.LockControlDeviceUID == Guid.Empty && x.Door.LockControlDeviceExitUID == Guid.Empty && x.Door.EnterZoneUID == Guid.Empty);
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			OnEdit(SelectedDoor.Door);
		}
		void OnEdit(GKDoor door)
		{
			var doorDetailsViewModel = new DoorDetailsViewModel(door);
			if (DialogService.ShowModalWindow(doorDetailsViewModel))
			{
				SelectedDoor.Update(doorDetailsViewModel.Door);
				doorDetailsViewModel.Door.OnChanged();
				doorDetailsViewModel.Door.OutDependentElements.ForEach(x => x.OnChanged());
				doorDetailsViewModel.Door.InputDependentElements.ForEach(x => x.OnChanged());
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand ShowDependencyItemsCommand { get; set; }

		void ShowDependencyItems()
		{
			if (SelectedDoor.Door != null)
			{
				var dependencyItemsViewModel = new DependencyItemsViewModel(SelectedDoor.Door.OutDependentElements);
				DialogService.ShowModalWindow(dependencyItemsViewModel);
			}
		}
		public void CreateDoor(CreateGKDoorEventArg createGKDoorEventArg)
		{
			DoorDetailsViewModel result = OnAddResult();
			createGKDoorEventArg.GKDoor = result == null ? null : result.Door;
		}
		public void EditDoor(Guid doorUID)
		{
			var doorViewModel = doorUID == Guid.Empty ? null : Doors.FirstOrDefault(x => x.Door.UID == doorUID);
			if (doorViewModel != null)
				OnEdit(doorViewModel.Door);
		}

		public override void OnShow()
		{
			base.OnShow();
			SelectedDoor = SelectedDoor;
		}

		public override void OnHide()
		{
			base.OnHide();
		}

		#region ISelectable<Guid> Members

		public void Select(Guid doorUID)
		{
			if (doorUID != Guid.Empty)
				SelectedDoor = Doors.FirstOrDefault(x => x.Door.UID == doorUID);
		}

		#endregion

		void RegisterShortcuts()
		{
			RegisterShortcut(new KeyGesture(KeyboardKey.N, ModifierKeys.Control), AddCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.Delete, ModifierKeys.Control), DeleteCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.E, ModifierKeys.Control), EditCommand);
		}
		public void LockedSelect(Guid doorUID)
		{
			_lockSelection = true;
			Select(doorUID);
			_lockSelection = false;
		}

		void SubscribeEvents()
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
		void OnDoorChanged(Guid doorUID)
		{
			var door = Doors.FirstOrDefault(x => x.Door.UID == doorUID);
			if (door != null)
			{
				door.Update();
				if (!_lockSelection)
					SelectedDoor = door;
			}
		}
		private void OnElementChanged(List<ElementBase> elements)
		{
			Guid guid = Guid.Empty;
			_lockSelection = true;
			elements.ForEach(element =>
			{
				var elementDoor = GetElementDoor(element);
				if (elementDoor != null)
					OnDoorChanged(elementDoor.DoorUID);
			});
			_lockSelection = false;
		}
		private void OnElementSelected(ElementBase element)
		{
			var elementDoor = GetElementDoor(element);
			if (elementDoor != null)
			{
				_lockSelection = true;
				Select(elementDoor.DoorUID);
				_lockSelection = false;
			}
		}
		private ElementGKDoor GetElementDoor(ElementBase element)
		{
			return element as ElementGKDoor;
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
					new RibbonMenuItemViewModel("Удалить все пустые ТД", DeleteAllEmptyCommand, "BDeleteEmpty"),
				}, "BEdit") { Order = 2 }
			};
		}
	}
}