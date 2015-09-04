using System.Windows;
using System.Windows.Controls;
using MediaSourcePlayer.MediaSource;
using VideoModule.ViewModels;
using Infrastructure;
using Infrastructure.Events;

namespace VideoModule.Views
{
	public partial class CameraDetailsView : UserControl
	{
		public CameraDetailsView()
		{
			InitializeComponent();
		}

		private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
		{
			var viewModel = DataContext as CameraDetailsViewModel;
			if (viewModel == null)
				return;
			viewModel.CanPlay = false;
			viewModel.CloseEvent += OnClose;
			MediaSourcePlayer.Open(MediaSourceFactory.CreateFromRtspStream(viewModel.Camera.RviRTSP));
			MediaSourcePlayer.Play();
		}
		public void OnClose()
		{
			MediaSourcePlayer.Stop();
			MediaSourcePlayer.Close();
		}
	}
}