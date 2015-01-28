using System;
using System.Windows.Media;
using FiresecClient;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Vlc.DotNet.Core;
using Vlc.DotNet.Core.Medias;
using Vlc.DotNet.Wpf;

namespace VideoModule.ViewModels
{
	public class VlcControlViewModel : BaseViewModel
	{
		public VlcControlViewModel(string rviRTSP)
		{
			_vlcControl = new VlcControl { Media = new LocationMedia(rviRTSP) };
			_vlcControl.PositionChanged -= VlcControlOnPositionChanged;
			_vlcControl.PositionChanged += VlcControlOnPositionChanged;
		}

		static VlcControlViewModel()
		{
			if (!VlcContext.IsInitialized)
			{
				//Set libvlc.dll and libvlccore.dll directory path
				VlcContext.LibVlcDllsPath = FiresecManager.SystemConfiguration.RviSettings.DllsPath;
				//Set the vlc plugins directory path
				VlcContext.LibVlcPluginsPath = FiresecManager.SystemConfiguration.RviSettings.PluginsPath;

				//Set the startup options
				VlcContext.StartupOptions.IgnoreConfig = true;
				VlcContext.StartupOptions.LogOptions.LogInFile = false;
				VlcContext.StartupOptions.LogOptions.ShowLoggerConsole = true;
				VlcContext.StartupOptions.LogOptions.Verbosity = VlcLogVerbosities.Debug;

				//Initialize the VlcContext
				VlcContext.Initialize();
			}
		}

		private VlcControl _vlcControl;
		public ImageSource Image
		{
			get
			{
				return _vlcControl.VideoSource;
			}
		}
		
		public void Start()
		{
			try
			{

				if (_vlcControl.IsPlaying)
					_vlcControl.Stop();
				_vlcControl.Play();
			}
			catch (Exception e)
			{
				MessageBoxService.ShowWarning(e.Message);
			}
		}

		public void Stop()
		{
			if (_vlcControl.IsPlaying)
				_vlcControl.Stop();
		}

		private void VlcControlOnPositionChanged(VlcControl sender, VlcEventArgs<float> vlcEventArgs)
		{
			OnPropertyChanged(() => Image);
		}
	}
}
