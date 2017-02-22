using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Common;
using Infrastructure.Common.Windows;
using MediaSourcePlayer.MediaSource;
using RVI.MediaSource.MediaSources;
using VideoModule.ViewModels;

namespace VideoModule.Views
{
	public partial class LayoutPartCameraView
	{
		public LayoutPartCameraView()
		{
			InitializeComponent();

			DataContextChanged += OnDataContextChanged;
			Dispatcher.ShutdownStarted += DispatcherOnShutdownStarted;
			videoCellControl.ReconnectEvent += VideoCellControlOnReconnectEvent;
		}

		private void VideoCellControlOnReconnectEvent(object sender, EventArgs eventArgs)
		{
			StartPlaying();
		}

		private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
		{
			StartPlaying();
		}

		private void StartPlaying()
		{
			var viewModel = DataContext as LayoutPartCameraViewModel;
			if (viewModel == null)
				return;

			Task.Factory.StartNew(() =>
			{
				try
				{
					IPEndPoint ipEndPoint;
					int vendorId;
					Logger.Info(string.Format("Камера '{0}'. Попытка начать трансляцию.", viewModel.Camera.Name));
					while (!viewModel.PrepareToTranslation(out ipEndPoint, out vendorId))
					{
						Thread.Sleep(30000);
						Logger.Info(string.Format("Камера '{0}'. Очередная попытка начать трансляцию.", viewModel.Camera.Name));
					}
					ApplicationService.Invoke(() =>
					{
						Logger.Info(string.Format("Камера '{0}'. Реквизиты для начала трансляции получены. Адрес='{1}', Издатель='{2}'", viewModel.Camera.Name, ipEndPoint, vendorId));
						videoCellControl.MediaPlayer.Open(MediaSourceFactory.CreateFromServerOnlineStream(ipEndPoint, vendorId));
						Logger.Info(string.Format("Камера '{0}'. Старт трансляции.", viewModel.Camera.Name));
						videoCellControl.MediaPlayer.Play();
					});
				}
				catch (Exception e)
				{
					Logger.Error(e, string.Format("Камера '{0}'. Исключительная ситуация при попытке трансляции.", viewModel.Camera.Name));
					ApplicationService.Invoke(() =>
					{
						videoCellControl.ShowReconnectButton = true;
					});
				}
			});
		}

		private void DispatcherOnShutdownStarted(object sender, EventArgs eventArgs)
		{
			videoCellControl.ReconnectEvent -= VideoCellControlOnReconnectEvent;
			videoCellControl.MediaPlayer.Stop();
			videoCellControl.MediaPlayer.Close();

			DataContextChanged -= OnDataContextChanged;
			Dispatcher.ShutdownStarted -= DispatcherOnShutdownStarted;
		}
	}
}