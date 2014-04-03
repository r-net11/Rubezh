using System;
using System.Windows;
using System.Windows.Controls;
using FiresecAPI.Models;
using VideoModule.ViewModels;

namespace VideoModule.Views
{
	public partial class CameraDetailsView : UserControl
	{
		Camera Camera { get; set; }
		public CameraDetailsView()
		{
			InitializeComponent();
			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
		}

		private void OnUnloaded(object sender, RoutedEventArgs routedEventArgs)
		{
			CellPlayerWrap.Stop();
		}

		private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
			var cameraDetailsViewModel = DataContext as CameraDetailsViewModel;
			Camera = cameraDetailsViewModel.Camera;
			Connect();
			Start();
		}

		public bool Connect()
		{
			try
			{
				CellPlayerWrap.Connect(Camera.Address, Camera.Port, Camera.Login, Camera.Password);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public bool Start()
		{
			try
			{
				CellPlayerWrap.Start(Camera.ChannelNumber);
				return true;
			}
			catch (Exception)
			{

				return false;
			}
		}
	}
}