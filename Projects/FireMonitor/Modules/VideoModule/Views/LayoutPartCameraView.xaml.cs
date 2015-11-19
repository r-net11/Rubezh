using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using MediaSourcePlayer.MediaSource;
using System;
using System.ComponentModel;
using System.Windows;
using VideoModule.ViewModels;

namespace VideoModule.Views
{
	public partial class LayoutPartCameraView
	{
		bool isStarted;
		public static CancelEventHandler CameraClosing { get; private set; }
		public LayoutPartCameraView()
		{
			InitializeComponent();
			Loaded += OnLoaded;
		}
		private void OnClosed(object sender, EventArgs e)
		{
			Stop();
		}	
		private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
			Start();
		}	
		public void Start()
		{
			if (isStarted)
			{
				Stop();
			}
			var layoutPartCameraViewModel = DataContext as LayoutPartCameraViewModel;
			if (layoutPartCameraViewModel != null)
			{
				if (layoutPartCameraViewModel.RviRTSP != null)
				{
					ApplicationService.Closed += OnClosed;
					MediaSourcePlayer.Open(MediaSourceFactory.CreateFromRtspStream(layoutPartCameraViewModel.RviRTSP));
					MediaSourcePlayer.Play();
				}
			}
			isStarted = true;
		}
		void Stop()
		{
			MediaSourcePlayer.Stop();
			MediaSourcePlayer.Close();
		}
	}
}