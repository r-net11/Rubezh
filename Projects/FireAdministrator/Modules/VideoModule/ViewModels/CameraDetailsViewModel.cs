using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Vlc.DotNet.Core;
using Vlc.DotNet.Core.Medias;
using Vlc.DotNet.Wpf;

namespace VideoModule.ViewModels
{
	class CameraDetailsViewModel : SaveCancelDialogViewModel
	{
		private bool _isPlaying;

		public CameraDetailsViewModel(Camera camera = null)
		{
			Camera = camera ?? new Camera();

			ShowCommand = new RelayCommand(OnShow, CanShow);
		}

		#region Properties


		public Camera Camera { get; private set; }

		VlcControl _vlcControl;
		public ImageSource Image
		{
			get
			{
				return _vlcControl == null ? new BitmapImage() : _vlcControl.VideoSource;
			}
		}
		#endregion

		#region Commands

		public RelayCommand ShowCommand { get; private set; }

		void OnShow()
		{
			try
			{
				if (_isPlaying)
					return;

				if (!VlcContext.IsInitialized)
				{
					VlcContext.LibVlcDllsPath = FiresecManager.SystemConfiguration.RviSettings.DllsPath;
					VlcContext.LibVlcPluginsPath = FiresecManager.SystemConfiguration.RviSettings.PluginsPath;
					VlcContext.StartupOptions.IgnoreConfig = true;
					VlcContext.StartupOptions.LogOptions.LogInFile = false;
					VlcContext.StartupOptions.LogOptions.ShowLoggerConsole = false;
					VlcContext.StartupOptions.LogOptions.Verbosity = VlcLogVerbosities.Debug;
					VlcContext.Initialize();
				}
				_vlcControl = new VlcControl { Media = new LocationMedia(Camera.RviRTSP) };
				_vlcControl.PositionChanged -= VlcControlOnPositionChanged;
				_vlcControl.PositionChanged += VlcControlOnPositionChanged;
				if (_vlcControl.IsPlaying)
					_vlcControl.Stop();
				_vlcControl.Play();

				_isPlaying = true;
			}
			catch (Exception e)
			{
				MessageBoxService.ShowWarning(e.Message);
			}
		}

		bool CanShow()
		{
			return !_isPlaying;
		}

		#endregion

		#region Methods

		private void VlcControlOnPositionChanged(VlcControl sender, VlcEventArgs<float> vlcEventArgs)
		{
			OnPropertyChanged(() => Image);
		}
		#endregion
	}
}