using System;
using System.Net;
using System.Windows;
using Common;
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
			if (viewModel == null)
				return;
			
			// При загрузке формы сразу стартуем просмотр видео ячейки
			try
			{
				IPEndPoint ipEndPoint;
				int vendorId;
				if (viewModel.PrepareToTranslation(out ipEndPoint, out vendorId))
				{
					var ms = MediaSourceFactory.CreateFromTcpSocket(ipEndPoint, vendorId);
					videoCellControl.MediaPlayer.Open(ms);
					videoCellControl.MediaPlayer.Play();
				}
			}
			catch (Exception e)
			{
				Logger.Error(e);
			}
		}

		private void OnUnloaded(object sender, RoutedEventArgs routedEventArgs)
		{
			// Останавливаем видео ячейку и освобождаем занятые ей ресурсы
			if (_needStopPlaying)
			{
				videoCellControl.MediaPlayer.Stop();
				videoCellControl.MediaPlayer.Close();
				_needStopPlaying = false;
			}
			
			Loaded -= OnLoaded;
			Unloaded -= OnUnloaded;
			Dispatcher.ShutdownStarted -= Dispatcher_ShutdownStarted;
		}

		private void Dispatcher_ShutdownStarted(object sender, EventArgs e)
		{
			// Останавливаем видео ячейку и освобождаем занятые ей ресурсы, если данное окно не закрывали, а вышли из приложения (т.к. окно не модальное)
			if (_needStopPlaying)
			{
				videoCellControl.MediaPlayer.Stop();
				videoCellControl.MediaPlayer.Close();
				_needStopPlaying = false;
			}
		}
	}
}