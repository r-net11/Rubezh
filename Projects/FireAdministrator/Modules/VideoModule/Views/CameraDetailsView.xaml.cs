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

			var viewModel = DataContext as CameraDetailsViewModel;
			if (viewModel == null)
				return;

			viewModel.Play -= ViewModelOnPlay;
		}
	}
}