using System.Windows;
using VideoModule.ViewModels;
using MediaSourcePlayer.MediaSource;
using System.Net;
using System;
using Common;

namespace VideoModule.Views
{
	public partial class CameraDetailsView
	{
		public CameraDetailsView()
		{
			InitializeComponent();
			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
		}


		private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
			var viewModel = DataContext as CameraDetailsViewModel;
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
		private void OnUnloaded(object sender, RoutedEventArgs routedEventArgs)
		{
			MediaSourcePlayer.Stop();
			MediaSourcePlayer.Close();
		}
	}
}