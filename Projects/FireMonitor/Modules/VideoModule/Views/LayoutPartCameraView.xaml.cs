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

			DataContextChanged += OnDataContextChanged;
			Dispatcher.ShutdownStarted += DispatcherOnShutdownStarted;
		}

		private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
		{
			var viewModel = DataContext as LayoutPartCameraViewModel;
			if (viewModel == null || String.IsNullOrEmpty(viewModel.RviRTSP))
				return;
			MediaSourcePlayer.Open(MediaSourceFactory.CreateFromRtspStream(viewModel.RviRTSP));
			MediaSourcePlayer.Play();
		}

		private void DispatcherOnShutdownStarted(object sender, EventArgs eventArgs)
		{
			MediaSourcePlayer.Stop();
			MediaSourcePlayer.Close();
		}
	}
}