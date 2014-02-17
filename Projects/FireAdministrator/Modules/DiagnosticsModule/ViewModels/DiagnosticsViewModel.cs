using System;
using Common;
using DiagnosticsModule.Views;
using FiresecAPI.Models;
using Infrastructure.Common;
using Infrastructure.Common.Video;
using Infrastructure.Common.Windows.ViewModels;
using FiresecClient;

namespace DiagnosticsModule.ViewModels
{
	public class DiagnosticsViewModel : ViewPartViewModel
	{
		private bool IsNowPlaying { get; set; }
		CameraFramesWatcher CameraFramesWatcher { get; set; }
		public DiagnosticsViewModel()
		{
			var camera = new Camera();
			camera.Login = "admin";
			camera.Password = "admin";
			camera.Address = "172.16.7.88";
			CameraFramesWatcher = new CameraFramesWatcher(camera);
			StartCommand = new RelayCommand(OnStart, () => !IsNowPlaying);
			StopCommand = new RelayCommand(OnStop, () => IsNowPlaying);
			SaveCommand = new RelayCommand(OnSave);
		}

		public RelayCommand StartCommand { get; private set; }
		void OnStart()
		{
			IsNowPlaying = true;
			CameraFramesWatcher.StartVideo();
		}

		public RelayCommand StopCommand { get; private set; }
		void OnStop()
		{
			CameraFramesWatcher.StopVideo();
			IsNowPlaying = false;
		}

		public RelayCommand SaveCommand { get; private set; }
		void OnSave()
		{
			var guid = Guid.NewGuid();
			CameraFramesWatcher.Save(guid, 5);
		}


		public void StopThreads()
		{
			IsThreadStoping = true;
		}
		bool IsThreadStoping = false;

		string _text;
		public string Text
		{
			get { return _text; }
			set
			{
				_text = value;
				OnPropertyChanged("Text");
			}
		}
	}
}