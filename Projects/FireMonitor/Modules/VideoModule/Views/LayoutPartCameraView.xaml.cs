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
		bool isLoaded;
		public LayoutPartCameraView()
		{
			InitializeComponent();

			DataContextChanged += OnDataContextChanged;
			Dispatcher.ShutdownStarted += DispatcherOnShutdownStarted;
		}
		void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
		{
			if (isLoaded)
			{
				Close();
				Start(); 
			}
		}
		void DispatcherOnShutdownStarted(object sender, EventArgs eventArgs)
		{
			Close();

			DataContextChanged -= OnDataContextChanged;
			Dispatcher.ShutdownStarted -= DispatcherOnShutdownStarted;
		}
		void UserControl_Unloaded(object sender, RoutedEventArgs e)
		{
			Close();
			isLoaded = false;
		}
		void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			Start();
			isLoaded = true;
		}
		void Close()
		{
			MediaSourcePlayer.Stop();
			MediaSourcePlayer.Close();
		}
		void Start()
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
	}
}