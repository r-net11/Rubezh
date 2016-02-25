using Common;
using MediaSourcePlayer.MediaSource;
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
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
		}
		void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			var viewModel = DataContext as CameraDetailsViewModel;
			if (viewModel == null)
				return;

			viewModel.Play -= ViewModelOnPlay;
			viewModel.Play += ViewModelOnPlay;
			viewModel.Stop -= ViewModelOnStop;
			viewModel.Stop += ViewModelOnStop;
		}
		void ViewModelOnPlay(object sender, EventArgs eventArgs)
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
		void ViewModelOnStop(object sender, EventArgs eventArgs)
		{
			PlayerStop();
		}
		void OnUnloaded(object sender, RoutedEventArgs routedEventArgs)
		{
			PlayerStop();
			var viewModel = DataContext as CameraDetailsViewModel;
			if (viewModel == null)
				return;

			viewModel.Play -= ViewModelOnPlay;
		}
		void PlayerStop()
		{
			MediaSourcePlayer.Stop();
			MediaSourcePlayer.Close();
		}
	}
}