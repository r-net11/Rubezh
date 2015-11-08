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
		public static CancelEventHandler CameraClosing { get; private set; }
		public LayoutPartCameraView()
		{
			InitializeComponent();
			Loaded += OnLoaded;
		}
		private void OnClosed(object sender, EventArgs e)
		{
			MediaSourcePlayer.Stop();
			MediaSourcePlayer.Close();
		}	
		private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
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
		}	
	}
}