using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using VideoModule.Plans.Designer;
using KeyboardKey = System.Windows.Input.Key;
using VideoModule.Plans;

namespace VideoModule.ViewModels
{
	public class CamerasViewModel : MenuViewPartViewModel, IEditingViewModel, ISelectable<Guid>
	{
		bool _lockSelection = false;
		public static CamerasViewModel Current { get; private set; }
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
			Current = this;
		}

		public void Initialize()
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
				if (cameraDetailsViewModel.Camera.CameraType != CameraType.Channel)
					FiresecManager.SystemConfiguration.Cameras.Add(cameraDetailsViewModel.Camera);
				var cameraViewModel = new CameraViewModel(this, cameraDetailsViewModel.Camera);
				if (cameraViewModel.IsChannel)
				{
					if (SelectedCamera.IsDvr)
					{
						cameraViewModel.Camera.Ip = SelectedCamera.Camera.Ip;
						cameraViewModel.Camera.Port = SelectedCamera.Camera.Port;
						cameraViewModel.Camera.Login = SelectedCamera.Camera.Login;
						cameraViewModel.Camera.Password = SelectedCamera.Camera.Password;
						SelectedCamera.AddChild(cameraViewModel);
						var rootDevice = FiresecManager.SystemConfiguration.Cameras.FirstOrDefault(x => x.UID == SelectedCamera.Camera.UID);
						if (rootDevice != null)
							rootDevice.Children.Add(cameraViewModel.Camera);
					}
					else
					{
						cameraViewModel.Camera.Ip = SelectedCamera.Parent.Camera.Ip;
						cameraViewModel.Camera.Port = SelectedCamera.Parent.Camera.Port;
						cameraViewModel.Camera.Login = SelectedCamera.Parent.Camera.Login;
						cameraViewModel.Camera.Password = SelectedCamera.Parent.Camera.Password;
						SelectedCamera.Parent.AddChild(cameraViewModel);
						var rootDevice = FiresecManager.SystemConfiguration.Cameras.FirstOrDefault(x => x.UID == SelectedCamera.Parent.Camera.UID);
						if (rootDevice != null)
							rootDevice.Children.Add(cameraViewModel.Camera);
					}
				}
				else
					Cameras.Add(cameraViewModel);
				if (SelectedCamera == null)
					SelectedCamera = cameraViewModel;
				ServiceFactory.SaveService.CamerasChanged = true;
				PlanExtension.Instance.Cache.BuildSafe<Camera>();
			}
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			var camera = SelectedCamera.Camera;
			if (SelectedCamera.IsChannel)
			{
				var parent = SelectedCamera.Parent;
				parent.RemoveChild(SelectedCamera);
				parent.Camera.Children.Remove(camera);
			}
			else
			{
				Cameras.Remove(SelectedCamera);
				FiresecManager.SystemConfiguration.Cameras.Remove(camera);
				camera.Children.ForEach(item => item.OnChanged());
			}
			camera.OnChanged();
			ServiceFactory.SaveService.CamerasChanged = true;
			SelectedCamera = Cameras.FirstOrDefault();
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var dvrDetailsViewModel = new CameraDetailsViewModel(SelectedCamera.Camera);
			if (DialogService.ShowModalWindow(dvrDetailsViewModel))
			{
				SelectedCamera.Camera = dvrDetailsViewModel.Camera;
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
			var autoSearchCameraViewModel = new AutoSearchCamerasViewModel(new List<CameraViewModel>(Cameras));
			if (DialogService.ShowModalWindow(autoSearchCameraViewModel))
				foreach (var autoSearchCamera in autoSearchCameraViewModel.AutoSearchCameras)
				{
					if (autoSearchCamera.IsChecked)
					{
						var camera = new Camera();
						camera.Ip = autoSearchCamera.DeviceSearchInfo.IpAddress;
						camera.Port = autoSearchCamera.DeviceSearchInfo.Port;
						var cameraViewModel = new CameraViewModel(this, camera);
						if (autoSearchCamera.DeviceSearchInfo.DeviceType.Contains("DVR"))
						{
							cameraViewModel.Camera.CameraType = CameraType.Dvr;
							cameraViewModel.Camera.Children.Add(new Camera
							{
								ChannelNumber = 1,
								Parent = cameraViewModel.Camera,
								CameraType = CameraType.Channel,
								Name = "Канал",
								Ip = cameraViewModel.Camera.Ip,
								Port = cameraViewModel.Camera.Port,
								Login = cameraViewModel.Camera.Login,
								Password = cameraViewModel.Camera.Password
							});
						}
						Cameras.Add(cameraViewModel);
					}
				}
		}

		private void SubscribeEvents()
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

		private void OnCameraChanged(Guid cameraUID)
		{
			var camera = AllCameras.FirstOrDefault(x => x.Camera.UID == cameraUID);
			if (camera != null)
			{
				camera.Update();
				// TODO: FIX IT
				if (!_lockSelection)
					SelectedCamera = camera;
			}
		}

		public List<CameraViewModel> AllCameras
		{
			get
			{
				var cameras = new List<CameraViewModel>();
				foreach (var camera in Cameras)
				{
					cameras.Add(camera);
					cameras.AddRange(camera.Children);
				}
				return cameras;
			}
		}

		private void OnElementChanged(List<ElementBase> elements)
		{
			_lockSelection = true;
			elements.ForEach(element =>
			{
				var elementCamera = element as ElementCamera;
				if (elementCamera != null)
					OnCameraChanged(elementCamera.CameraUID);
			});
			_lockSelection = false;
		}
		private void OnElementSelected(ElementBase element)
		{
			var elementCamera = element as ElementCamera;
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
			{
				SelectedCamera = AllCameras.FirstOrDefault(item => item.Camera.UID == cameraUID);
				if (SelectedCamera != null)
					SelectedCamera.ExpandToThis();
			}
		}

		private void RegisterShortcuts()
		{
			RegisterShortcut(new KeyGesture(KeyboardKey.N, ModifierKeys.Control), AddCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.Delete, ModifierKeys.Control), DeleteCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.E, ModifierKeys.Control), EditCommand);
		}
	}
}