using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using FiresecClient;
using System.IO;
using Vlc.DotNet.Core;
using Vlc.DotNet.Core.Medias;
using Vlc.DotNet.Wpf;

namespace JournalModule.ViewModels
{
	public class VideoViewModel : DialogViewModel
	{
		readonly string DirectoryPath = AppDataFolderHelper.GetTempFolder();
		string VideoPath { get; set; }

		public VideoViewModel(Guid eventUID, Guid cameraUID)
		{
			//if (!Directory.Exists(DirectoryPath))
			//	Directory.CreateDirectory(DirectoryPath);
			VideoPath = AppDataFolderHelper.GetTempFileName() + ".avi";
			Title = "Видеофрагмент, связанный с событием";
			RviClient.RviClientHelper.GetVideoFile(FiresecManager.SystemConfiguration, eventUID, cameraUID, VideoPath);
			Play();
		}

		void Play()
		{
			try
			{
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
				_vlcControl = new VlcControl { Media = new PathMedia(VideoPath) };
				_vlcControl.PositionChanged -= VlcControlOnPositionChanged;
				_vlcControl.PositionChanged += VlcControlOnPositionChanged;
				if (_vlcControl.IsPlaying)
					_vlcControl.Stop();
				_vlcControl.Play();
			}
			catch (Exception e)
			{
				MessageBoxService.ShowWarning(e.Message);
			}
		}

		private VlcControl _vlcControl;
		public ImageSource Image
		{
			get
			{
				if (_vlcControl == null)
					return new BitmapImage();
				return _vlcControl.VideoSource;
			}
		}


		private void VlcControlOnPositionChanged(VlcControl sender, VlcEventArgs<float> vlcEventArgs)
		{
			OnPropertyChanged(() => Image);
		}

		public override bool OnClosing(bool isCanceled)
		{
			if (_vlcControl.IsPlaying)
				_vlcControl.Stop();
			if (File.Exists(VideoPath))
				File.Delete(VideoPath);
			return base.OnClosing(isCanceled);
		}
	}
}