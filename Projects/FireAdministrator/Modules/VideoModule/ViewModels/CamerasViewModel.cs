using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using VideoPlayerTest;
using KeyboardKey = System.Windows.Input.Key;
using Vlc.DotNet.Core;
using System.Windows.Media.Imaging;
using System.IO;

namespace VideoModule.ViewModels
{
	public class CamerasViewModel : MenuViewPartViewModel, IEditingViewModel
	{
		private VideoClass _videoSequence;
		public CamerasViewModel()
		{
			Menu = new CamerasMenuViewModel(this);
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanEditDelete);
			EditCommand = new RelayCommand(OnEdit, CanEditDelete);
			ScreenShortCommand = new RelayCommand(OnScreenShort);
			PlayVideoCommand = new RelayCommand(OnPlayVideo, () => SelectedCamera != null);
			RegisterShortcuts();
			Initialize();
			//VlcInitialize();
		}
		private void VlcInitialize()
		{
			VlcContext.LibVlcDllsPath = CommonStrings.LIBVLC_DLLS_PATH_DEFAULT_VALUE_X86;
			VlcContext.LibVlcPluginsPath = CommonStrings.PLUGINS_PATH_DEFAULT_VALUE_X86;

			//Set the startup options
			//VlcContext.StartupOptions.IgnoreConfig = true;
			//VlcContext.StartupOptions.LogOptions.LogInFile = true;
			//VlcContext.StartupOptions.LogOptions.ShowLoggerConsole = true;
			//VlcContext.StartupOptions.LogOptions.Verbosity = VlcLogVerbosities.Debug;
			
			// Disable showing the movie file name as an overlay
			VlcContext.StartupOptions.AddOption("--no-video-title-show");

			// Initialize the VlcContext
			VlcContext.Initialize();
			_videoSequence = new VideoClass();
			_videoSequence.Play();
		}
		public void Initialize()
		{
			Cameras = new ObservableCollection<CameraViewModel>();
			foreach (var camera in FiresecManager.SystemConfiguration.Cameras)
			{
				var cameraViewModel = new CameraViewModel(camera);
				Cameras.Add(cameraViewModel);
			}
			SelectedCamera = Cameras.FirstOrDefault();
		}

		public VideoClass VideoSequence
		{
			get
			{
				return _videoSequence;
			}
		}
		public RelayCommand PlayVideoCommand { get; private set; }
		void OnPlayVideo()
		{
			if (!IsNowPlaying)
			{
				foreach (var camera in Cameras)
				{
					if (camera.IsNowPlaying)
						camera.StopVideo();
				}
				SelectedCamera.StartVideo();
			}
			else
			{
				StartedCamera.StopVideo();
			}
			OnPropertyChanged("StartedCamera");
			OnPropertyChanged("IsNowPlaying");
		}

		public RelayCommand ScreenShortCommand { get; private set; }
		void OnScreenShort()
		{
			var image = _videoSequence.Image;
			var encoder = new JpegBitmapEncoder();
			encoder.Frames.Add(BitmapFrame.Create(image as BitmapImage));
			using (var filestream = new FileStream("c:\\1.jpg", FileMode.Create))
				encoder.Save(filestream);
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
				OnPropertyChanged("SelectedCamera");
			}
		}

		public CameraViewModel StartedCamera
		{
			get { return Cameras.FirstOrDefault(x => x.IsNowPlaying); }
		}

		public bool IsNowPlaying
		{
			get { return StartedCamera != null; }
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var cameraDetailsViewModel = new CameraDetailsViewModel();
			if (DialogService.ShowModalWindow(cameraDetailsViewModel))
			{
				FiresecManager.SystemConfiguration.Cameras.Add(cameraDetailsViewModel.Camera);
				var cameraViewModel = new CameraViewModel(cameraDetailsViewModel.Camera);
				Cameras.Add(cameraViewModel);
				SelectedCamera = cameraViewModel;
				ServiceFactory.SaveService.CamerasChanged = true;
			}
		}

		bool CanEditDelete()
		{
			return SelectedCamera != null;
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			SelectedCamera.StopVideo();
			FiresecManager.SystemConfiguration.Cameras.Remove(SelectedCamera.Camera);
			Cameras.Remove(SelectedCamera);
			ServiceFactory.SaveService.CamerasChanged = true;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var cameraDetailsViewModel = new CameraDetailsViewModel(SelectedCamera.Camera);
			if (DialogService.ShowModalWindow(cameraDetailsViewModel))
			{
				SelectedCamera.Camera = cameraDetailsViewModel.Camera;
				SelectedCamera.Update();
				ServiceFactory.SaveService.CamerasChanged = true;
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