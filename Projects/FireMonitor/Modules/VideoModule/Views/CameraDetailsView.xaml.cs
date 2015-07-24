using System;
using System.Windows;
using FiresecAPI.Models;
using MediaSourcePlayer.MediaSource;
using VideoModule.ViewModels;

namespace VideoModule.Views
{
	public partial class CameraDetailsView
	{
		Camera Camera { get; set; }
		public CameraDetailsView()
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
			var viewModel = DataContext as CameraDetailsViewModel;
			if (viewModel != null)
			{
				MediaSourcePlayer.Open(MediaSourceFactory.CreateFromRtspStream(viewModel.RviRTSP));
				MediaSourcePlayer.Play();
			}
		}
	}
}