using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
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
			SubscribeEvents();
			IsRightPanelEnabled = true;
		}

		public void Initialize()
		{
			Cameras = new ObservableCollection<CameraViewModel>();
			foreach (var camera in FiresecManager.SystemConfiguration.Cameras)
			{
				var cameraViewModel = new CameraViewModel(this, camera);
				var cccameraViewModel = new CameraViewModel(this, camera);
				cameraViewModel.AddChild(cccameraViewModel);
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
			if ((SelectedCamera == null) || (!SelectedCamera.IsChannel))
			{
				var dvrDetailsViewModel = new DvrDetailsViewModel();
				if (DialogService.ShowModalWindow(dvrDetailsViewModel))
				{
					FiresecManager.SystemConfiguration.Cameras.Add(dvrDetailsViewModel.Camera);
					var cameraViewModel = new CameraViewModel(this, dvrDetailsViewModel.Camera);
					Cameras.Add(cameraViewModel);
					ServiceFactory.SaveService.CamerasChanged = true;
					Helper.BuildCameraMap();
				}
			}
			else
			{
				var camera = new Camera();
				camera.Parent = SelectedCamera.Camera;
				var cameraDetailsViewModel = new CameraDetailsViewModel(camera);
				if (DialogService.ShowModalWindow(cameraDetailsViewModel))
				{
					FiresecManager.SystemConfiguration.Cameras.Add(cameraDetailsViewModel.Camera);
					var cameraViewModel = new CameraViewModel(this, cameraDetailsViewModel.Camera);
					SelectedCamera.AddChild(cameraViewModel);
					ServiceFactory.SaveService.CamerasChanged = true;
					Helper.BuildCameraMap();
				}
			}
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			SelectedCamera.Camera.OnChanged();
			FiresecManager.SystemConfiguration.Cameras.Remove(SelectedCamera.Camera);
			Cameras.Remove(SelectedCamera);
			ServiceFactory.SaveService.CamerasChanged = true;
			Helper.BuildCameraMap();
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			if (!SelectedCamera.IsChannel)
			{
				var dvrDetailsViewModel = new DvrDetailsViewModel(SelectedCamera.Camera);
				if (DialogService.ShowModalWindow(dvrDetailsViewModel))
				{
					SelectedCamera.Camera = dvrDetailsViewModel.Camera;
					SelectedCamera.Update();
					SelectedCamera.Camera.OnChanged();
					ServiceFactory.SaveService.CamerasChanged = true;
				}
			}
			else
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
		}

		bool CanEditDelete()
		{
			return SelectedCamera != null;
		}

		public RelayCommand SearchCommand { get; private set; }
		void OnSearch()
		{
			var autoSearchCameraViewModel = new AutoSearchCamerasViewModel(new List<CameraViewModel>(Cameras));
			if (DialogService.ShowModalWindow(autoSearchCameraViewModel))
				foreach (var autoSearchCamera in autoSearchCameraViewModel.AutoSearchCameras)
				{
					if (autoSearchCamera.IsChecked)
					{
						var camera = new Camera();
						camera.Address = autoSearchCamera.DeviceSearchInfo.IpAddress;
						camera.Port = autoSearchCamera.DeviceSearchInfo.Port;
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