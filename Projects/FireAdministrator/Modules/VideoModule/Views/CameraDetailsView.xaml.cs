using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using MediaSourcePlayer.MediaSource;
using VideoModule.ViewModels;

namespace VideoModule.Views
{
	public partial class CameraDetailsView : UserControl
	{
		public CameraDetailsView()
		{
			InitializeComponent();

			Unloaded += OnUnloaded;
		}

		private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
		{
			var viewModel = DataContext as CameraDetailsViewModel;
			if (viewModel == null)
				return;
			viewModel.CanPlay = false;

			MediaSourcePlayer.Open(MediaSourceFactory.CreateFromRtspStream(viewModel.Camera.RviRTSP));
			MediaSourcePlayer.Play();
		}

		private void OnUnloaded(object sender, RoutedEventArgs routedEventArgs)
		{
			MediaSourcePlayer.Stop();
			MediaSourcePlayer.Close();
		}
	}
}