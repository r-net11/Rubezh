using Common;
using FiresecClient.SKDHelpers;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using Infrustructure.Plans.Events;
using Localization.Strazh.Common;
using Localization.Strazh.ViewModels;
using StrazhAPI.Models;
using StrazhAPI.Plans.Elements;
using StrazhAPI.SKD;
using StrazhModule.Events;
using StrazhModule.Plans;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using KeyboardKey = System.Windows.Input.Key;

namespace StrazhModule.ViewModels
{
	public class DoorsViewModel : MenuViewPartViewModel, IEditingViewModel, ISelectable<Guid>
	{
		bool _lockSelection;
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
			Doors = new SortableObservableCollection<DoorViewModel>();
			foreach (var door in SKDManager.SKDConfiguration.Doors.OrderBy(x => x.No))
			{
				var doorViewModel = new DoorViewModel(door);
				Doors.Add(doorViewModel);
			}
			SelectedDoor = Doors.FirstOrDefault();
		}

		SortableObservableCollection<DoorViewModel> _doors;
		public SortableObservableCollection<DoorViewModel> Doors
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
				if (!_lockSelection && _selectedDoor != null && _selectedDoor.Door.PlanElementUIDs.Count > 0)
					ServiceFactoryBase.Events.GetEvent<FindElementEvent>().Publish(_selectedDoor.Door.PlanElementUIDs);
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
				SKDPlanExtension.Instance.Cache.BuildSafe<SKDDoor>();
				return doorDetailsViewModel;
			}
			return null;
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			if (!MessageBoxService.ShowConfirmation(String.Format(CommonViewModels.Door_DeleteConfirm, SelectedDoor.Door.Name)))
				return;
			var index = Doors.IndexOf(SelectedDoor);
			SKDManager.RemoveDoor(SelectedDoor.Door);
			SelectedDoor.Door.OnChanged();

			var organisations = OrganisationHelper.Get(new OrganisationFilter());
			foreach (var organisation in organisations)
			{
				if (organisation.DoorUIDs.Contains(SelectedDoor.Door.UID))
				{
					organisation.DoorUIDs.Remove(SelectedDoor.Door.UID);
					OrganisationHelper.SaveDoors(organisation);
				}
			}

			if (SelectedDoor.InDevice != null)
				SKDManager.RemoveDeviceDoor(SelectedDoor.InDevice.Parent, SelectedDoor.Door);

			Doors.Remove(SelectedDoor);

			index = Math.Min(index, Doors.Count - 1);
			if (index > -1)
				SelectedDoor = Doors[index];

			ServiceFactory.SaveService.SKDChanged = true;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			OnEdit(SelectedDoor.Door);
		}
		void OnEdit(SKDDoor door)
		{
			var previousDoorType = door.DoorType;
			var doorDetailsViewModel = new DoorDetailsViewModel(door);
			if (DialogService.ShowModalWindow(doorDetailsViewModel))
			{
				if (previousDoorType != doorDetailsViewModel.Door.DoorType)
				{
					SKDManager.ChangeDoorDevice(doorDetailsViewModel.Door, null);
				}

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
			ServiceFactoryBase.Events.GetEvent<ElementAddedEvent>().Unsubscribe(OnElementChanged);
			ServiceFactoryBase.Events.GetEvent<ElementRemovedEvent>().Unsubscribe(OnElementChanged);
			ServiceFactoryBase.Events.GetEvent<ElementChangedEvent>().Unsubscribe(OnElementChanged);
			ServiceFactoryBase.Events.GetEvent<ElementSelectedEvent>().Unsubscribe(OnElementSelected);

			ServiceFactoryBase.Events.GetEvent<ElementAddedEvent>().Subscribe(OnElementChanged);
			ServiceFactoryBase.Events.GetEvent<ElementRemovedEvent>().Subscribe(OnElementChanged);
			ServiceFactoryBase.Events.GetEvent<ElementChangedEvent>().Subscribe(OnElementChanged);
			ServiceFactoryBase.Events.GetEvent<ElementSelectedEvent>().Subscribe(OnElementSelected);
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
			RibbonItems = new List<RibbonMenuItemViewModel>
			{
					new RibbonMenuItemViewModel(CommonViewModels.Edition, new ObservableCollection<RibbonMenuItemViewModel>
				{
					new RibbonMenuItemViewModel(CommonResources.Add, AddCommand, "BAdd"),
					new RibbonMenuItemViewModel(CommonResources.Edit, EditCommand, "BEdit"),
					new RibbonMenuItemViewModel(CommonResources.Delete, DeleteCommand, "BDelete"),
				}, "BEdit") { Order = 2 }
			};
		}
	}
}