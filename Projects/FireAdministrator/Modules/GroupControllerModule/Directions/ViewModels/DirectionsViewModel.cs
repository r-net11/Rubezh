using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using FiresecAPI.Models;
using FiresecClient;
using GKModule.Plans.Designer;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrastructure.ViewModels;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using XFiresecAPI;
using KeyboardKey = System.Windows.Input.Key;
using Infrastructure.Common.Ribbon;
using System.Diagnostics;

namespace GKModule.ViewModels
{
	public class DirectionsViewModel : MenuViewPartViewModel, IEditingViewModel, ISelectable<Guid>
	{
		private bool _lockSelection;
		public DirectionsViewModel()
		{
			Menu = new DirectionsMenuViewModel(this);
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanEditDelete);
			EditCommand = new RelayCommand(OnEdit, CanEditDelete);

			DeleteZoneCommand = new RelayCommand(OnDeleteZone, CanDeleteZone);
			ChangeZonesCommand = new RelayCommand(OnChangeZones, CanEditDelete);

			DeleteDeviceCommand = new RelayCommand(OnDeleteDevice, CanDeleteDevice);
			ChangeDevicesCommand = new RelayCommand(OnChangeDevices, CanEditDelete);

			DeleteOutputDeviceCommand = new RelayCommand(OnDeleteOutputDevice, CanDeleteOutputDevice);
			ChangeOutputDevicesCommand = new RelayCommand(OnChangeOutputDevices, CanEditDelete);
			
			RegisterShortcuts();
			IsRightPanelEnabled = true;
			SubscribeEvents();
			SetRibbonItems();
		}
		protected override bool IsRightPanelVisibleByDefault
		{
			get { return true; }
		}

		public void Initialize()
		{
			Directions = new ObservableCollection<DirectionViewModel>(
				from direction in XManager.Directions
				orderby direction.No
				select new DirectionViewModel(direction));
			SelectedDirection = Directions.FirstOrDefault();
		}

		ObservableCollection<DirectionViewModel> _directions;
		public ObservableCollection<DirectionViewModel> Directions
		{
			get { return _directions; }
			set
			{
				_directions = value;
				OnPropertyChanged("Directions");
			}
		}

		DirectionViewModel _selectedDirection;
		public DirectionViewModel SelectedDirection
		{
			get { return _selectedDirection; }
			set
			{
				_selectedDirection = value;
				if (value != null)
					value.Update();
				OnPropertyChanged("SelectedDirection");
				OnPropertyChanged("ZonesCount");
				OnPropertyChanged("OutputDevicesCount");
				OnPropertyChanged("DevicesCount");
				if (!_lockSelection && _selectedDirection != null && _selectedDirection.Direction.PlanElementUIDs.Count > 0)
					ServiceFactory.Events.GetEvent<FindElementEvent>().Publish(_selectedDirection.Direction.PlanElementUIDs);
			}
		}

		bool CanEditDelete()
		{
			return SelectedDirection != null;
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			OnAddResult();
		}
		private DirectionDetailsViewModel OnAddResult()
		{
			var directionDetailsViewModel = new DirectionDetailsViewModel();
			if (DialogService.ShowModalWindow(directionDetailsViewModel))
			{
				XManager.AddDirection(directionDetailsViewModel.XDirection);
				var directionViewModel = new DirectionViewModel(directionDetailsViewModel.XDirection);
				Directions.Add(directionViewModel);
				SelectedDirection = directionViewModel;
				ServiceFactory.SaveService.GKChanged = true;
				Helper.BuildMap();
				return directionDetailsViewModel;
			}
			return null;
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			var dialogResult = MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить направление " + SelectedDirection.Direction.PresentationName);
			if (dialogResult == MessageBoxResult.Yes)
			{
				XManager.RemoveDirection(SelectedDirection.Direction);
				Directions.Remove(SelectedDirection);
				SelectedDirection = Directions.FirstOrDefault();
				ServiceFactory.SaveService.GKChanged = true;
				Helper.BuildMap();
			}
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			OnEdit(SelectedDirection.Direction);
		}
		void OnEdit(XDirection xdirection)
		{
			var directionDetailsViewModel = new DirectionDetailsViewModel(xdirection);
			if (DialogService.ShowModalWindow(directionDetailsViewModel))
			{
				SelectedDirection.Direction = directionDetailsViewModel.XDirection;
				SelectedDirection.Update();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand DeleteZoneCommand { get; private set; }
		void OnDeleteZone()
		{
			if (SelectedDirection.SelectedZone == null)
				return;
			SelectedDirection.DeleteZone(SelectedDirection.SelectedZone.DirectionZone);
			OnPropertyChanged("ZonesCount");
			SelectedDirection.SelectedZone = null;
		}

		public RelayCommand ChangeZonesCommand { get; private set; }
		void OnChangeZones()
		{
			SelectedDirection.ChangeZones();
			OnPropertyChanged("ZonesCount");
		}

		bool CanDeleteZone()
		{
			return SelectedDirection!=null && SelectedDirection.SelectedZone != null;
		}

		public RelayCommand ChangeDevicesCommand { get; private set; }
		void OnChangeDevices()
		{
			SelectedDirection.ChangeDevices();
			OnPropertyChanged("DevicesCount");
		}

		public RelayCommand DeleteDeviceCommand { get; private set; }
		void OnDeleteDevice()
		{
			if (SelectedDirection.SelectedDevice == null)
				return;
			SelectedDirection.DeleteDevice(SelectedDirection.SelectedDevice.DirectionDevice.Device);
			OnPropertyChanged("DevicesCount");
			SelectedDirection.SelectedDevice = null;
		}

		bool CanDeleteDevice()
		{
			return SelectedDirection != null && SelectedDirection.SelectedDevice != null;
		}

		public RelayCommand ChangeOutputDevicesCommand { get; private set; }
		void OnChangeOutputDevices()
		{
			SelectedDirection.ChangeOutputDevices();
			OnPropertyChanged("OutputDevicesCount");
		}

		public RelayCommand DeleteOutputDeviceCommand { get; private set; }
		void OnDeleteOutputDevice()
		{
			if (SelectedDirection.SelectedOutputDevice == null)
				return;
			SelectedDirection.DeleteOutputDevice(SelectedDirection.SelectedOutputDevice.Device);
			OnPropertyChanged("OutputDevicesCount");
			SelectedDirection.SelectedOutputDevice = null;
		}

		bool CanDeleteOutputDevice()
		{
			return SelectedDirection != null && SelectedDirection.SelectedOutputDevice != null;
		}



		public int ZonesCount
		{
			get
			{
				if (SelectedDirection == null)
					return 0;
				return SelectedDirection.Zones.Count;
			}
		}
		public int DevicesCount
		{
			get
			{
				if (SelectedDirection == null)
					return 0;
				return SelectedDirection.Devices.Count;
			}
		}
		public int OutputDevicesCount
		{
			get
			{
				if (SelectedDirection == null)
					return 0;
				return SelectedDirection.OutputDevices.Count;
			}
		}

		public void CreateDirection(CreateXDirectionEventArg createDirectionEventArg)
		{
			DirectionDetailsViewModel result = OnAddResult();
			if (result == null)
			{
				createDirectionEventArg.Cancel = true;
				createDirectionEventArg.DirectionUID = Guid.Empty;
			}
			else
			{
				createDirectionEventArg.Cancel = false;
				createDirectionEventArg.DirectionUID = result.XDirection.UID;
				createDirectionEventArg.XDirection = result.XDirection;
			}
		}
		public void EditDirection(Guid directionUID)
		{
			var directionViewModel = directionUID == Guid.Empty ? null : Directions.FirstOrDefault(x => x.Direction.UID == directionUID);
			if (directionViewModel != null)
				OnEdit(directionViewModel.Direction);
		}

		public override void OnShow()
		{
			base.OnShow();
			SelectedDirection = SelectedDirection;
		}
		public override void OnHide()
		{
			base.OnHide();
		}

		#region ISelectable<Guid> Members
		public void Select(Guid directionUID)
		{
			if (directionUID != Guid.Empty)
				SelectedDirection = Directions.FirstOrDefault(x => x.Direction.UID == directionUID);
		}
		#endregion

		void RegisterShortcuts()
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

		void SubscribeEvents()
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
		private void OnDirectionChanged(Guid directionUID)
		{
			var direction = Directions.FirstOrDefault(x => x.Direction.UID == directionUID);
			if (direction != null)
			{
				direction.Update();
				// TODO: FIX IT
				if (!_lockSelection)
					SelectedDirection = direction;
			}
		}
		private void OnElementRemoved(List<ElementBase> elements)
		{
			elements.OfType<ElementRectangleXDirection>().ToList().ForEach(element => Helper.ResetXDirection(element));
			elements.OfType<ElementPolygonXDirection>().ToList().ForEach(element => Helper.ResetXDirection(element));
			OnElementChanged(elements);
		}
		private void OnElementChanged(List<ElementBase> elements)
		{
			Guid guid = Guid.Empty;
			_lockSelection = true;
			elements.ForEach(element =>
			{
				var elementDirection = GetElementXDirection(element);
				if (elementDirection != null)
				{
					OnDirectionChanged(elementDirection.DirectionUID);
					//if (guid != Guid.Empty)
					//    OnZoneChanged(guid);
					//guid = elementZone.ZoneUID;
				}
			});
			_lockSelection = false;
			//if (guid != Guid.Empty)
			//    OnZoneChanged(guid);
		}
		private void OnElementSelected(ElementBase element)
		{
			var elementDirection = GetElementXDirection(element);
			if (elementDirection != null)
			{
				_lockSelection = true;
				Select(elementDirection.DirectionUID);
				_lockSelection = false;
			}
		}
		private IElementDirection GetElementXDirection(ElementBase element)
		{
			IElementDirection elementDirection = element as ElementRectangleXDirection;
			if (elementDirection == null)
				elementDirection = element as ElementPolygonXDirection;
			return elementDirection;
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