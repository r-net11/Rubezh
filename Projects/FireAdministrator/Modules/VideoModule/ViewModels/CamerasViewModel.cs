using Common;
using Localization.Video.ViewModels;
using StrazhAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrastructure.ViewModels;
using StrazhAPI.Plans.Elements;
using Infrustructure.Plans.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using VideoModule.Plans;
using KeyboardKey = System.Windows.Input.Key;

namespace VideoModule.ViewModels
{
	public class CamerasViewModel : MenuViewPartViewModel, ISelectable<Guid>, IEditingViewModel
	{
		bool _lockSelection;

		public CamerasViewModel()
		{
			Menu = new CamerasMenuViewModel(this);
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanEditDelete);
			EditCommand = new RelayCommand(OnEdit, CanEditDelete);
			SettingsCommand = new RelayCommand(OnSettings);
			RegisterShortcuts();
			SubscribeEvents();
			IsRightPanelEnabled = true;
			Current = this;
		}

		#region Properties

		public static CamerasViewModel Current { get; private set; }

		private SortableObservableCollection<CameraViewModel> _cameras;
		public SortableObservableCollection<CameraViewModel> Cameras
		{
			get { return _cameras; }
			set
			{
				_cameras = value;
				OnPropertyChanged(() => Cameras);
			}
		}

		private CameraViewModel _selectedCamera;
		public CameraViewModel SelectedCamera
		{
			get { return _selectedCamera; }
			set
			{
				_selectedCamera = value;
				OnPropertyChanged(() => SelectedCamera);
			}
		}

		#endregion

		#region Commands
		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var devicesViewModel = new DeviceSelectionViewModel();
			if (DialogService.ShowModalWindow(devicesViewModel))
			{
				foreach (var camera in devicesViewModel.GetCameras())
				{
					camera.OnChanged();
					FiresecManager.SystemConfiguration.Cameras.Add(camera);
					Cameras.Add(new CameraViewModel(this, camera));
				}
				ServiceFactory.SaveService.CamerasChanged = true;
				PlanExtension.Instance.Cache.BuildSafe<Camera>();
			}
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			var camera = SelectedCamera.Camera;
			if (!MessageBoxService.ShowConfirmation(String.Format(CommonViewModels.DeleteCamConfirm, camera.Name)))
				return;
			Cameras.Remove(SelectedCamera);
			FiresecManager.SystemConfiguration.Cameras.Remove(camera);
			camera.OnChanged();
			ServiceFactory.SaveService.CamerasChanged = true;

			// Уведомляем подписчиков на событие CameraDeletedEvent о том, что камера была удалена из конфигурации
			ServiceFactory.Events.GetEvent<CameraDeletedEvent>().Publish(camera.UID);
			
			SelectedCamera = Cameras.FirstOrDefault();
		}

		public RelayCommand EditCommand { get; private set; }
		private void OnEdit()
		{
			DialogService.ShowModalWindow(new CameraDetailsViewModel(SelectedCamera.Camera));
		}

		bool CanEditDelete()
		{
			return SelectedCamera != null;
		}

		public RelayCommand SettingsCommand { get; private set; }
		void OnSettings()
		{
			var settingsSelectionViewModel = new SettingsSelectionViewModel(FiresecManager.SystemConfiguration.RviSettings);
			if (DialogService.ShowModalWindow(settingsSelectionViewModel))
			{
				if (FiresecManager.SystemConfiguration.RviSettings.VideoIntegrationProvider != settingsSelectionViewModel.RviSettings.VideoIntegrationProvider)
					RemoveAllCameras();
				FiresecManager.SystemConfiguration.RviSettings = settingsSelectionViewModel.RviSettings;
				ServiceFactory.SaveService.CamerasChanged = true;
			}
		}

		#endregion

		#region Methods

		public void Initialize()
		{
			Cameras = new SortableObservableCollection<CameraViewModel>();
			foreach (var camera in FiresecManager.SystemConfiguration.Cameras)
			{
				var cameraViewModel = new CameraViewModel(this, camera);
				Cameras.Add(cameraViewModel);
			}
			SelectedCamera = Cameras.FirstOrDefault();
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
			var camera = Cameras.FirstOrDefault(x => x.Camera.UID == cameraUID);
			if (camera != null)
			{
				camera.Update();
				// TODO: FIX IT
				if (!_lockSelection)
					SelectedCamera = camera;
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
			if (elementCamera == null) return;

			_lockSelection = true;
			Select(elementCamera.CameraUID);
			_lockSelection = false;
		}

		public void Select(Guid cameraUID)
		{
			if (cameraUID != Guid.Empty)
			{
				SelectedCamera = Cameras.FirstOrDefault(item => item.Camera.UID == cameraUID);
			}
		}

		private void RegisterShortcuts()
		{
			RegisterShortcut(new KeyGesture(KeyboardKey.N, ModifierKeys.Control), AddCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.Delete, ModifierKeys.Control), DeleteCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.E, ModifierKeys.Control), EditCommand);
		}

		private void RemoveAllCameras()
		{
			Cameras.Clear();
			FiresecManager.SystemConfiguration.Cameras.Clear();
			ServiceFactory.SaveService.CamerasChanged = true;
			SelectedCamera = Cameras.FirstOrDefault();
		}

		#endregion

		#region Overrides

		public override void OnShow()
		{
			base.OnShow();

			if (Cameras != null)
				Cameras.Sort(x => x.Camera != null ? x.Camera.Name : string.Empty);
		}

		#endregion
	}
}