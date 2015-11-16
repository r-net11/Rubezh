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
		}

		private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
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
					MediaSourcePlayer.Open(MediaSourceFactory.CreateFromTcpSocket(ipEndPoint, vendorId));
					MediaSourcePlayer.Play();
				}
			}
			catch (Exception e)
			{
				Logger.Error(e);
			}
		}

		private void DispatcherOnShutdownStarted(object sender, EventArgs eventArgs)
		{
			MediaSourcePlayer.Stop();
			MediaSourcePlayer.Close();

			DataContextChanged -= OnDataContextChanged;
			Dispatcher.ShutdownStarted -= DispatcherOnShutdownStarted;
		}
	}
}