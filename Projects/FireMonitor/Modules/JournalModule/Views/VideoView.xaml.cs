using System;
using System.Windows;
using System.Windows.Controls;
using Common;
using JournalModule.ViewModels;
using MediaSourcePlayer.MediaSource;

namespace JournalModule.Views
{
	public partial class VideoView : UserControl
	{
		public VideoView()
		{
			InitializeComponent();

			DataContextChanged += OnDataContextChanged;
			Unloaded += OnUnloaded;
		}

		private void OnUnloaded(object sender, RoutedEventArgs routedEventArgs)
		{
			var model = DataContext as VideoViewModel;
			if (model == null)
				return;

			model.Play -= ModelOnPlay;
			model.Stop -= ModelOnStop;

			DataContextChanged -= OnDataContextChanged;
			Unloaded -= OnUnloaded;
		}

		private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
		{
			var model = DataContext as VideoViewModel;
			if (model == null) return;
			model.Play -= ModelOnPlay;
			model.Play += ModelOnPlay;
			model.Stop -= ModelOnStop;
			model.Stop += ModelOnStop;
		}

		private void ModelOnStop(object sender, EventArgs eventArgs)
		{
			videoCellControl.MediaPlayer.Stop();
			videoCellControl.MediaPlayer.Close();
		}

		private void ModelOnPlay(object sender, EventArgs eventArgs)
		{
			var model = DataContext as VideoViewModel;
			if (model == null)
				return;
			try
			{
				videoCellControl.MediaPlayer.Open(MediaSourceFactory.GetMediaSource(new Uri(model.VideoPath)));
				videoCellControl.MediaPlayer.Play();
			}
			catch (Exception e)
			{
				Logger.Error(e);
			}
		}
	}
}