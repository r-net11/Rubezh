using Common;
using MediaSourcePlayer.MediaSource;
using RubezhAPI.Models;
using System;
using System.Net;
using System.Windows;
using VideoModule.ViewModels;

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
			viewModel.Surface.WindowStartupLocation = WindowStartupLocation.Manual;
			viewModel.Surface.Left = viewModel.MarginLeft;
			viewModel.Surface.Top = viewModel.MarginTop;
			viewModel.Play -= ViewModelOnPlay;
			viewModel.Play += ViewModelOnPlay;
			viewModel.Stop -= ViewModelOnStop;
			viewModel.Stop += ViewModelOnStop;

			PlayerStart();
		}
		private void OnUnloaded(object sender, RoutedEventArgs routedEventArgs)
		{
			PlayerStop();
		}
		void ViewModelOnPlay(object sender, EventArgs eventArgs)
		{
			PlayerStart();
		}
		void ViewModelOnStop(object sender, EventArgs eventArgs)
		{
			PlayerStop();
		}
		void PlayerStart()
		{
			var viewModel = DataContext as CameraDetailsViewModel;
			if (viewModel == null || viewModel.Status == RviStatus.ConnectionLost || viewModel.Status == RviStatus.Error)
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
		void PlayerStop()
		{
			MediaSourcePlayer.Stop();
			MediaSourcePlayer.Close();
		}
	}
}