using System.Windows;
using VideoModule.ViewModels;
using MediaSourcePlayer.MediaSource;

namespace VideoModule.Views
{
	public partial class CameraDetailsView
	{
		public CameraDetailsView()
		{
			InitializeComponent();
			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
		}


		private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
			var viewModel = DataContext as CameraDetailsViewModel;
			if (viewModel == null)
				return;
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