using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using System.Windows.Threading;
using Entities.DeviceOriented;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using KeyboardKey = System.Windows.Input.Key;
using Infrustructure.Plans.Events;
using System;
using Infrustructure.Plans.Elements;
using VideoModule.Plans.Designer;
using FiresecAPI.Models;
using System.Collections.Generic;

namespace VideoModule.ViewModels
{
	public class CamerasViewModel : MenuViewPartViewModel, IEditingViewModel, ISelectable<Guid>
	{
		bool _lockSelection = false;

		public CamerasViewModel()
		{
			Menu = new CamerasMenuViewModel(this);
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanEditDelete);
			EditCommand = new RelayCommand(OnEdit, CanEditDelete);
			SearchCommand = new RelayCommand(OnSearch);
			RegisterShortcuts();
			InitializeCameras();
			SubscribeEvents();
			IsRightPanelEnabled = true;
		}

		void InitializeCameras()
		{
			Cameras = new ObservableCollection<CameraViewModel>();
			foreach (var camera in FiresecManager.SystemConfiguration.Cameras)
			{
				var cameraViewModel = new CameraViewModel(this, camera);
				Cameras.Add(cameraViewModel);
			}
			SelectedCamera = Cameras.FirstOrDefault();
		}

		ObservableCollection<CameraViewModel> _cameras;
		public ObservableCollection<CameraViewModel> Cameras
		{
			get { return _cameras; }
			set
			{
				_cameras = value;
				OnPropertyChanged("Cameras");
			}
		}

		CameraViewModel _selectedCamera;
		public CameraViewModel SelectedCamera
		{
			get { return _selectedCamera; }
			set
			{
				_selectedCamera = value;
				OnPropertyChanged(() => SelectedCamera);
				if (!_lockSelection && SelectedCamera != null && SelectedCamera.Camera.PlanElementUIDs.Count > 0)
					ServiceFactory.Events.GetEvent<FindElementEvent>().Publish(SelectedCamera.Camera.PlanElementUIDs);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var cameraDetailsViewModel = new CameraDetailsViewModel();
			if (DialogService.ShowModalWindow(cameraDetailsViewModel))
			{
				FiresecManager.SystemConfiguration.Cameras.Add(cameraDetailsViewModel.Camera);
				var cameraViewModel = new CameraViewModel(this, cameraDetailsViewModel.Camera);
				Cameras.Add(cameraViewModel);
				ServiceFactory.SaveService.CamerasChanged = true;
				Plans.Designer.Helper.BuildCameraMap();
			}
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			SelectedCamera.Camera.OnChanged();
			FiresecManager.SystemConfiguration.Cameras.Remove(SelectedCamera.Camera);
			Cameras.Remove(SelectedCamera);
			ServiceFactory.SaveService.CamerasChanged = true;
			Plans.Designer.Helper.BuildCameraMap();
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var cameraDetailsViewModel = new CameraDetailsViewModel(SelectedCamera.Camera);
			if (DialogService.ShowModalWindow(cameraDetailsViewModel))
			{
				SelectedCamera.Camera = cameraDetailsViewModel.Camera;
				SelectedCamera.Update();
				SelectedCamera.Camera.OnChanged();
				ServiceFactory.SaveService.CamerasChanged = true;
			}
		}

		bool CanEditDelete()
		{
			return SelectedCamera != null;
		}

		public RelayCommand SearchCommand { get; private set; }
		void OnSearch()
		{
			var foundCamerasViewModel = new FoundCamerasViewModel(new List<CameraViewModel>(Cameras));
			if(DialogService.ShowModalWindow(foundCamerasViewModel))
				foreach (var foundCamera in foundCamerasViewModel.FoundCameras)
				{
					if (foundCamera.IsChecked)
					{
						var camera = new Camera();
						camera.Address = foundCamera.Address;
						camera.Port = foundCamera.Port;
						var cameraViewModel = new CameraViewModel(this, camera);
						Cameras.Add(cameraViewModel);
					}
				}
		}

		private void SubscribeEvents()
		{
			ServiceFactory.Events.GetEvent<ElementAddedEvent>().Unsubscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementRemovedEvent>().Unsubscribe(OnElementRemoved);
			ServiceFactory.Events.GetEvent<ElementChangedEvent>().Subscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementSelectedEvent>().Unsubscribe(OnElementSelected);

			ServiceFactory.Events.GetEvent<ElementAddedEvent>().Subscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementRemovedEvent>().Subscribe(OnElementRemoved);
			ServiceFactory.Events.GetEvent<ElementChangedEvent>().Subscribe(OnElementChanged);
			ServiceFactory.Events.GetEvent<ElementSelectedEvent>().Subscribe(OnElementSelected);
		}

		private void OnDeviceChanged(Guid cameraUID)
		{
			var camera = Cameras.FirstOrDefault(x => x.Camera.UID == cameraUID);
			if (camera != null)
			{
				camera.Update();
				// TODO: FIX IT
				if (!_lockSelection)
					SelectedCamera = camera;
			}
		}
		private void OnElementRemoved(List<ElementBase> elements)
		{
			elements.OfType<ElementCamera>().ToList().ForEach(element => Helper.ResetCamera(element));
			OnElementChanged(elements);
		}
		private void OnElementChanged(List<ElementBase> elements)
		{
			_lockSelection = true;
			elements.ForEach(element =>
			{
				ElementCamera elementDevice = element as ElementCamera;
				if (elementDevice != null)
					OnDeviceChanged(elementDevice.CameraUID);
			});
			_lockSelection = false;
		}
		private void OnElementSelected(ElementBase element)
		{
			ElementCamera elementCamera = element as ElementCamera;
			if (elementCamera != null)
			{
				_lockSelection = true;
				Select(elementCamera.CameraUID);
				_lockSelection = false;
			}
		}

		public void Select(Guid cameraUID)
		{
			if (cameraUID != Guid.Empty)
				SelectedCamera = Cameras.FirstOrDefault(item => item.Camera.UID == cameraUID);
		}

		private void RegisterShortcuts()
		{
			RegisterShortcut(new KeyGesture(KeyboardKey.N, ModifierKeys.Control), AddCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.Delete, ModifierKeys.Control), DeleteCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.E, ModifierKeys.Control), EditCommand);
		}
	}
}