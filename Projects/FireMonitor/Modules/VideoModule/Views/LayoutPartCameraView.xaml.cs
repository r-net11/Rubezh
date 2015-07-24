using System;
using System.Windows;
using MediaSourcePlayer.MediaSource;
using VideoModule.ViewModels;

namespace VideoModule.Views
{
	public partial class LayoutPartCameraView
	{
		public LayoutPartCameraView()
		{
			InitializeComponent();

			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
		}

		private void OnUnloaded(object sender, RoutedEventArgs routedEventArgs)
		{
			MediaSourcePlayer.Stop();
			MediaSourcePlayer.Close();
		}

		private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
			var viewModel = DataContext as LayoutPartCameraViewModel;
			if (viewModel == null || String.IsNullOrEmpty(viewModel.RviRTSP))
				return;
			MediaSourcePlayer.Open(MediaSourceFactory.CreateFromRtspStream(viewModel.RviRTSP));
			MediaSourcePlayer.Play();
		}
	}
}