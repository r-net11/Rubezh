using Infrastructure.Common.Windows.ViewModels;
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
		}

		private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
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
					ShellViewModel.Current.Closing += OnClosing;
					MediaSourcePlayer.Open(MediaSourceFactory.CreateFromRtspStream(layoutPartCameraViewModel.RviRTSP));
					MediaSourcePlayer.Play();
				}
			}
		}	
	}
}