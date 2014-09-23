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
			foreach (var door in XManager.DeviceConfiguration.Doors)
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
				OnPropertyChanged(() => SelectedDoor);
				OnPropertyChanged(() => HasSelectedDoor);
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
				XManager.DeviceConfiguration.Doors.Add(doorDetailsViewModel.Door);
				var doorViewModel = new DoorViewModel(doorDetailsViewModel.Door);
				Doors.Add(doorViewModel);
				SelectedDoor = doorViewModel;
				ServiceFactory.SaveService.GKChanged = true;
				GKPlanExtension.Instance.Cache.BuildSafe<XDoor>();
				return doorDetailsViewModel;
			}
			return null;
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			var dialogResult = MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить точку доступа " + SelectedDoor.Door.PresentationName);
			if (dialogResult == MessageBoxResult.Yes)
			{
				var index = Doors.IndexOf(SelectedDoor);
				XManager.DeviceConfiguration.Doors.Remove(SelectedDoor.Door);
				SelectedDoor.Door.OnChanged();
				Doors.Remove(SelectedDoor);
				index = Math.Min(index, Doors.Count - 1);
				if (index > -1)
					SelectedDoor = Doors[index];
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			OnEdit(SelectedDoor.Door);
		}
		void OnEdit(XDoor door)
		{
			var doorDetailsViewModel = new DoorDetailsViewModel(door);
			if (DialogService.ShowModalWindow(doorDetailsViewModel))
			{
				SelectedDoor.Update(doorDetailsViewModel.Door);
				doorDetailsViewModel.Door.OnChanged();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public void CreateDoor(CreateXDoorEventArg createDoorEventArg)
		{
			DoorDetailsViewModel result = OnAddResult();
			if (result == null)
			{
				createDoorEventArg.Cancel = true;
				createDoorEventArg.DoorUID = Guid.Empty;
			}
			else
			{
				createDoorEventArg.Cancel = false;
				createDoorEventArg.DoorUID = result.Door.UID;
				createDoorEventArg.Door = result.Door;
			}
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