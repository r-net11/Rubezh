using MediaSourcePlayer.MediaSource;
using System.Windows;
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
			var layoutPartCameraViewModel = DataContext as LayoutPartCameraViewModel;
			if (layoutPartCameraViewModel != null)
			{
				if (layoutPartCameraViewModel.RviRTSP != null)
				{
					MediaSourcePlayer.Open(MediaSourceFactory.CreateFromRtspStream(layoutPartCameraViewModel.RviRTSP));
					MediaSourcePlayer.Play();
				}
			}
		}
	}
}