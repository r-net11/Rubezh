using System;
using System.Net;
using System.Windows;
using Common;
using MediaSourcePlayer.MediaSource;
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
			try
			{
				IPEndPoint ipEndPoint;
				int vendorId;
				if (viewModel.PrepareToTranslation(out ipEndPoint, out vendorId))
				{
					videoCellControl.MediaPlayer.Open(MediaSourceFactory.CreateFromTcpSocket(ipEndPoint, vendorId));
					videoCellControl.MediaPlayer.Play();
				}
			}
			catch (Exception e)
			{
				Logger.Error(e);
				videoCellControl.ShowReconnectButton = true;
			}
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