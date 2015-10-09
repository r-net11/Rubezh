using System;
using System.Windows;
using FiresecAPI.Models;
using MediaSourcePlayer.MediaSource;
using VideoModule.ViewModels;

namespace VideoModule.Views
{
	public partial class CameraDetailsView
	{
		private bool _needStopPlaying;

		private Camera Camera { get; set; }

		public CameraDetailsView()
		{
			InitializeComponent();

			_needStopPlaying = false;

			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
			Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
		}

		private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
			var viewModel = DataContext as CameraDetailsViewModel;
			
			// При загрузке формы сразу стартуем просмотр видео ячейки
			if (viewModel != null)
			{
				MediaSourcePlayer.Open(MediaSourceFactory.CreateFromRtspStream(viewModel.RviRTSP));
				MediaSourcePlayer.Play();
				_needStopPlaying = true;
			}
		}

		private void OnUnloaded(object sender, RoutedEventArgs routedEventArgs)
		{
			// Останавливаем видео ячейку и освобождаем занятые ей ресурсы
			if (_needStopPlaying)
			{
				MediaSourcePlayer.Stop();
				MediaSourcePlayer.Close();
				_needStopPlaying = false;
			}
			Dispatcher.ShutdownStarted -= Dispatcher_ShutdownStarted;
		}

		private void Dispatcher_ShutdownStarted(object sender, EventArgs e)
		{
			// Останавливаем видео ячейку и освобождаем занятые ей ресурсы, если данное окно не закрывали, а вышли из приложения (т.к. окно не модальное)
			if (_needStopPlaying)
			{
				MediaSourcePlayer.Stop();
				MediaSourcePlayer.Close();
				_needStopPlaying = false;
			}
		}
	}
}