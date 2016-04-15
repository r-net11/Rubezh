using System;
using System.ComponentModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using Common;
using MediaSourcePlayer.MediaSource;
using VideoModule.ViewModels;

namespace VideoModule.Views
{
	public partial class CameraDetailsView : UserControl
	{
		public CameraDetailsView()
		{
			InitializeComponent();

			DataContextChanged += OnDataContextChanged;
			Unloaded += OnUnloaded;
			videoCellControl.ReconnectEvent += VideoCellControlOnReconnectEvent;
		}

		void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			var viewModel = DataContext as CameraDetailsViewModel;
			if (viewModel == null)
				return;

			viewModel.Play -= ViewModelOnPlay;
			viewModel.Play += ViewModelOnPlay;
		}

		private void ViewModelOnPlay(object sender, EventArgs eventArgs)
		{
			StartPlaying();
		}

		private void StartPlaying()
		{
#if DEBUG
			Logger.Info("CameraDetilsView.StartPlaying()");
#endif
			var viewModel = DataContext as CameraDetailsViewModel;
			if (viewModel == null)
				return;

			try
			{
				IPEndPoint ipEndPoint;
				int vendorId;
				if (viewModel.PrepareToTranslation(out ipEndPoint, out vendorId))
				{
					videoCellControl.MediaPlayer.Open(MediaSourceFactory.CreateFromTcpSocket(ipEndPoint, vendorId));
					Logger.Info("Источник данных для видео ячейки открыт.");
					videoCellControl.MediaPlayer.Play();
					Logger.Info("Видео ячейка начала проигрывание");
				}
#if DEBUG
				else
				{
					Logger.Info("Ошибка при получении от видео источника ip-адреса для трансляции");
				}
#endif
			}
			catch (Exception e)
			{
				Logger.Info("Ошибка перазапуска видео ячейки");
				Logger.Error(e);
				videoCellControl.ShowReconnectButton = true;
			}
		}

		private void OnUnloaded(object sender, RoutedEventArgs routedEventArgs)
		{
#if DEBUG
			Logger.Info("CameraDetailsView.OnUnloaded()");
#endif
			videoCellControl.ReconnectEvent -= VideoCellControlOnReconnectEvent;

			videoCellControl.MediaPlayer.Stop();
			videoCellControl.MediaPlayer.Close();

			var viewModel = DataContext as CameraDetailsViewModel;
			if (viewModel == null)
				return;

			viewModel.Play -= ViewModelOnPlay;
		}

		private void VideoCellControlOnReconnectEvent(object sender, EventArgs eventArgs)
		{
			StartPlaying();
		}
	}
}