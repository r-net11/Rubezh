using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Plans.Events;
using Infrastructure.ViewModels;
using RubezhAPI.Models;
using RubezhAPI.Plans.Elements;
using RubezhClient;
using RviClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using VideoModule.Plans;
using KeyboardKey = System.Windows.Input.Key;

namespace VideoModule.ViewModels
{
	public class CamerasViewModel : MenuViewPartViewModel, ISelectable<Guid>
	{
		public static CamerasViewModel Current { get; private set; }
		public List<CameraViewModel> AllCameras { get; private set; }
		bool _lockSelection;

		public CamerasViewModel()
		{
			Menu = new CamerasMenuViewModel(this);
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanEditDelete);
			EditCommand = new RelayCommand(OnEdit, CanEditDelete);
			SettingsCommand = new RelayCommand(OnSettings);
			RegisterShortcuts();
			IsRightPanelEnabled = true;
			Current = this;

			ServiceFactory.Events.GetEvent<ElementSelectedEvent>().Unsubscribe(OnElementSelected);
			ServiceFactory.Events.GetEvent<ElementSelectedEvent>().Subscribe(OnElementSelected);
		}

		public void Initialize()
		{
			Cameras = new ObservableCollection<CameraViewModel>();
			AllCameras = new List<CameraViewModel>();
			BuildTree();
			SelectedCamera = Cameras.FirstOrDefault();
		}
		void BuildTree()
		{
			var servers = ClientManager.SystemConfiguration.RviServers;
			foreach (var server in servers)
			{
				var serverViewModel = new CameraViewModel(server);
				foreach (var device in server.RviDevices)
				{
					var deviceViewModel = new CameraViewModel(device);
					foreach (var camera in device.Cameras)
					{
						var cameraViewModel = new CameraViewModel(this, camera, camera.Name);
						AllCameras.Add(cameraViewModel);
						deviceViewModel.AddChild(cameraViewModel);
					}
					serverViewModel.AddChild(deviceViewModel);
				}
				Cameras.Add(serverViewModel);
			}
		}
		ObservableCollection<CameraViewModel> _cameras;
		public ObservableCollection<CameraViewModel> Cameras
		{
			get { return _cameras; }
			set
			{
				_cameras = value;
				OnPropertyChanged(() => Cameras);
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
				if (!_lockSelection && _selectedCamera != null && _selectedCamera.IsOnPlan)
					ServiceFactory.Events.GetEvent<FindElementEvent>().Publish(_selectedCamera.Camera.PlanElementUIDs);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var rviServers = GetRviServers();
			if (rviServers.Count != 0)
			{
				var rviDeviceSelectionViewModel = new RviDeviceSelectionViewModel(rviServers);
				if (DialogService.ShowModalWindow(rviDeviceSelectionViewModel))
				{
					var oldCameras = ClientManager.SystemConfiguration.Cameras;
					ClientManager.SystemConfiguration.RviServers = rviDeviceSelectionViewModel.RviServers;
					ClientManager.SystemConfiguration.UpdateRviConfiguration();
					foreach (var removedCamera in oldCameras.Where(oldCamera => !ClientManager.SystemConfiguration.Cameras.Any(newCamera => newCamera.UID == oldCamera.UID)))
					{
						removedCamera.OnChanged();
					}
					Initialize();
					ServiceFactory.SaveService.CamerasChanged = true;
					PlanExtension.Instance.Cache.BuildSafe<Camera>();
				}
			}
		}
		List<RviServer> GetRviServers()
		{
			var rviSettings = ClientManager.SystemConfiguration.RviSettings;
			var rviServers = new List<RviServer>();
			WaitHelper.Execute(() =>
			{
				rviServers = RviClientHelper.GetServers(rviSettings.Url, rviSettings.Login, rviSettings.Password, ClientManager.SystemConfiguration.Cameras);
			});
			if (rviServers.Count == 0)
			{
				MessageBoxService.ShowWarning(string.Format("Не удалось подключиться к серверу {0}:{1}", rviSettings.Ip, rviSettings.Port));
			}
			else
			{
				var notConnectedRviServers = rviServers.Where(x => x.Status == RviStatus.ConnectionLost);
				if (notConnectedRviServers.Count() > 0)
				{
					var message = new StringBuilder("Не удалось подключиться к следующим серверам из конфигурации:\n");
					foreach (var notConnectedRviServer in notConnectedRviServers)
					{
						message.Append(string.Format("{0}:{1}\n", notConnectedRviServer.Ip, notConnectedRviServer.Port));
					}
					MessageBoxService.ShowWarning(message.ToString());
				}
			}
			return rviServers;
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			var camera = SelectedCamera.Camera;
			var server = ClientManager.SystemConfiguration.RviServers.First(x => x.Url == camera.RviServerUrl);
			var device = server.RviDevices.First(x => x.Uid == camera.RviDeviceUID);
			device.Cameras.Remove(camera);
			if (device.Cameras.Count == 0)
			{
				server.RviDevices.Remove(device);
				if (server.RviDevices.Count == 0)
				{
					ClientManager.SystemConfiguration.RviServers.Remove(server);
				}
			}
			ClientManager.SystemConfiguration.Cameras.Remove(camera);
			RemoveFromTree(SelectedCamera);
			camera.OnChanged();
			ServiceFactory.SaveService.CamerasChanged = true;
			SelectedCamera = Cameras.FirstOrDefault();
		}
		void RemoveFromTree(CameraViewModel cameraViewModel)
		{
			var parent = cameraViewModel.Parent;
			if (parent == null)
			{
				Cameras.Remove(cameraViewModel);
				return;
			}
			parent.RemoveChild(cameraViewModel);
			if (parent.Children.Count() == 0)
				RemoveFromTree(parent);
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var cameraDetailsViewModel = new CameraDetailsViewModel(SelectedCamera.Camera);
			if (DialogService.ShowModalWindow(cameraDetailsViewModel))
			{
				SelectedCamera.Camera = cameraDetailsViewModel.Camera;
				SelectedCamera.Camera.OnChanged();
				ServiceFactory.SaveService.CamerasChanged = true;
			}
		}

		bool CanEditDelete()
		{
			return SelectedCamera != null && SelectedCamera.Camera != null;
		}

		public RelayCommand SettingsCommand { get; private set; }
		void OnSettings()
		{
			var settingsSelectionViewModel = new RviSettingsViewModel(ClientManager.SystemConfiguration.RviSettings);
			if (DialogService.ShowModalWindow(settingsSelectionViewModel))
			{
				ClientManager.SystemConfiguration.RviSettings = settingsSelectionViewModel.RviSettings;
				ServiceFactory.SaveService.CamerasChanged = true;
			}
		}

		private void RegisterShortcuts()
		{
			RegisterShortcut(new KeyGesture(KeyboardKey.N, ModifierKeys.Control), AddCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.Delete, ModifierKeys.Control), DeleteCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.E, ModifierKeys.Control), EditCommand);
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
				SelectedCamera = AllCameras.FirstOrDefault(item => item.IsCamera && item.Camera.UID == cameraUID);
			}
		}
	}
}