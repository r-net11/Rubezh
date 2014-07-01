using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
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
	public class DoorsViewModel : MenuViewPartViewModel, IEditingViewModel, ISelectable<Guid>
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
			RegisterShortcuts();
			IsRightPanelEnabled = true;
			SubscribeEvents();
			SetRibbonItems();
		}

		public void Initialize()
		{
			Doors = new ObservableCollection<DoorViewModel>();
			foreach (var door in SKDManager.SKDConfiguration.Doors)
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
				OnPropertyChanged("Doors");
			}
		}

		DoorViewModel _selectedDoor;
		public DoorViewModel SelectedDoor
		{
			get { return _selectedDoor; }
			set
			{
				_selectedDoor = value;
				OnPropertyChanged(() => SelectedDoor);
				if (!_lockSelection && _selectedDoor != null && _selectedDoor.Door.PlanElementUIDs.Count > 0)
					ServiceFactory.Events.GetEvent<FindElementEvent>().Publish(_selectedDoor.Door.PlanElementUIDs);
			}
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
				SKDManager.SKDConfiguration.Doors.Add(doorDetailsViewModel.Door);
				var doorViewModel = new DoorViewModel(doorDetailsViewModel.Door);
				Doors.Add(doorViewModel);
				SelectedDoor = doorViewModel;
				ServiceFactory.SaveService.SKDChanged = true;
				SKDPlanExtension.Instance.Cache.BuildSafe<Door>();
				return doorDetailsViewModel;
			}
			return null;
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			var dialogResult = MessageBoxService.ShowQuestion("Вы уверены, что хотите точку доступа " + SelectedDoor.Door.Name);
			if (dialogResult == MessageBoxResult.Yes)
			{
				var index = Doors.IndexOf(SelectedDoor);
				SKDManager.SKDConfiguration.Doors.Remove(SelectedDoor.Door);
				SelectedDoor.Door.OnChanged();
				Doors.Remove(SelectedDoor);
				index = Math.Min(index, Doors.Count - 1);
				if (index > -1)
					SelectedDoor = Doors[index];
				ServiceFactory.SaveService.SKDChanged = true;
			}
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			OnEdit(SelectedDoor.Door);
		}
		void OnEdit(Door door)
		{
			var doorDetailsViewModel = new DoorDetailsViewModel(door);
			if (DialogService.ShowModalWindow(doorDetailsViewModel))
			{
				SelectedDoor.Update(doorDetailsViewModel.Door);
				doorDetailsViewModel.Door.OnChanged();
				ServiceFactory.SaveService.SKDChanged = true;
			}
		}

		public void CreateDoor(CreateDoorEventArg createDoorEventArg)
		{
			DoorDetailsViewModel result = OnAddResult();
			createDoorEventArg.Door = result == null ? null : result.Door;
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

		private void RegisterShortcuts()
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
		private void OnDoorChanged(Guid doorUID)
		{
			var door = Doors.FirstOrDefault(x => x.Door.UID == doorUID);
			if (door != null)
			{
				door.Update();
				// TODO: FIX IT
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
				{
					OnDoorChanged(elementDoor.DoorUID);
					//if (guid != Guid.Empty)
					//	OnDoorChanged(guid);
					//guid = elementDoor.DoorUID;
				}
			});
			_lockSelection = false;
			//if (guid != Guid.Empty)
			//	OnDoorChanged(guid);
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
		private ElementDoor GetElementDoor(ElementBase element)
		{
			ElementDoor elementDoor = element as ElementDoor;
			return elementDoor;
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
				}, "/Controls;component/Images/BEdit.png") { Order = 2 }
			};
		}
	}
}