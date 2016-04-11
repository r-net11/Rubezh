using Common;
using MediaSourcePlayer.MediaSource;
using System;
using System.Net;
using System.Windows;
using VideoModule.ViewModels;

namespace VideoModule.Views
{
	public partial class LayoutPartCameraView
	{
		bool isLoaded;
		LayoutPartCameraViewModel viewModel;
		public LayoutPartCameraView()
		{
			InitializeComponent();

			Loaded += UserControl_Loaded;
			Unloaded += UserControl_Unloaded;
			DataContextChanged += OnDataContextChanged;
			Dispatcher.ShutdownStarted += DispatcherOnShutdownStarted;
		}
		void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
		{
			Load();
		}
		void Load()
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
			if (viewModel != null && viewModel.Camera != null)
				viewModel.Camera.StatusChanged -= Load;
			Close();
			isLoaded = false;
		}
		void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			viewModel = DataContext as LayoutPartCameraViewModel;
			if (viewModel != null && viewModel.Camera != null)
				viewModel.Camera.StatusChanged += Load;
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