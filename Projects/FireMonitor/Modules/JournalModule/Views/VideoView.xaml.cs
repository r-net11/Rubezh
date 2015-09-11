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
		private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
			var videoViewModel = DataContext as VideoViewModel;
			{
				if (videoViewModel.VideoPath != null)
				{
					videoViewModel.Closing += Close;
					MediaSourcePlayer.Open(MediaSourceFactory.GetMediaSource(new Uri(videoViewModel.VideoPath)));
					MediaSourcePlayer.Play();
				}
			}
		}
		private void Close(object sender, EventArgs e)
		{
			MediaSourcePlayer.Stop();
			MediaSourcePlayer.Close();
		}
	}
}