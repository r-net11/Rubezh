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
	public class DoorsViewModel : MenuViewPartViewModel, ISelectable<Guid>
	{
		bool _lockSelection = false;
		public static DoorsViewModel Current { get; private set; }

		public DoorsViewModel()
		{
			Menu = new DoorsMenuViewModel(this);
			Current = this;
			AddCommand = new RelayCommand(OnAdd);
			EditCommand = new RelayCommand(OnEdit, CanEditDelete);
			DeleteCommand = new RelayCommand(OnDelete, CanEditDelete);
			DeleteAllEmptyCommand = new RelayCommand(OnDeleteAllEmpty, CanDeleteAllEmpty);
			ShowDependencyItemsCommand = new RelayCommand(ShowDependencyItems);
			RegisterShortcuts();
			IsRightPanelEnabled = true;
			SetRibbonItems();

			ServiceFactory.Events.GetEvent<ElementSelectedEvent>().Subscribe(OnElementSelected);
			ServiceFactory.Events.GetEvent<CreateGKDoorEvent>().Subscribe(CreateDoor);
			ServiceFactory.Events.GetEvent<EditGKDoorEvent>().Subscribe(EditDoor);
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
				GKManager.AddDoor(doorDetailsViewModel.Door);
				var doorViewModel = new DoorViewModel(doorDetailsViewModel.Door);
				Doors.Add(doorViewModel);
				SelectedDoor = doorViewModel;
				ServiceFactory.SaveService.GKChanged = true;
				GKPlanExtension.Instance.Cache.BuildSafe<GKDoor>();
				return doorDetailsViewModel;
			}
			return null;
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
				SelectedDoor.Update();
				GKManager.EditDoor(doorDetailsViewModel.Door);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить точку доступа " + SelectedDoor.Door.PresentationName + " ?"))
			{
				var index = Doors.IndexOf(SelectedDoor);
				GKManager.RemoveDoor(SelectedDoor.Door);
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
				GetEmptyDoors().ForEach(x =>
					{
						GKManager.RemoveDoor(x.Door);
						Doors.Remove(x);
					});
				SelectedDoor = Doors.FirstOrDefault();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		bool CanDeleteAllEmpty()
		{
			return GetEmptyDoors().Any();
		}

		List<DoorViewModel> GetEmptyDoors()
		{
			return Doors.Where(x => !x.Door.InputDependentElements.Any()).ToList();
		}

		public RelayCommand ShowDependencyItemsCommand { get; set; }

		void ShowDependencyItems()
		{
			if (SelectedDoor.Door != null)
			{
				var dependencyItemsViewModel = new DependencyItemsViewModel(SelectedDoor.Door.OutputDependentElements);
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
	}
}