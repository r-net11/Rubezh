using JournalModule.ViewModels;
using MediaSourcePlayer.MediaSource;
using System;
using System.Windows;
using System.Windows.Controls;

namespace JournalModule.Views
{
	public partial class VideoView : UserControl
	{
		public VideoView()
		{
			InitializeComponent();
			Loaded += OnLoaded;
		}

		void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
			var videoViewModel = DataContext as VideoViewModel;
			videoViewModel.Surface.WindowStartupLocation = WindowStartupLocation.Manual;
			videoViewModel.Surface.Left = videoViewModel.MarginLeft;
			videoViewModel.Surface.Top = videoViewModel.MarginTop;
			if (videoViewModel.VideoPath != null)
			{
				videoViewModel.Closing += Close;
				MediaSourcePlayer.Open(MediaSourceFactory.GetMediaSource(new Uri(videoViewModel.VideoPath)));
				MediaSourcePlayer.Play();
			}
		}

		void Close(object sender, EventArgs e)
		{
			MediaSourcePlayer.Stop();
			MediaSourcePlayer.Close();
		}
	}
}