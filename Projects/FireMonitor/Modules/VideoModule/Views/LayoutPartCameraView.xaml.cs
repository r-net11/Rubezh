using System.Windows;
using VideoModule.ViewModels;
using System.Linq;

namespace VideoModule.Views
{
	public partial class LayoutPartCameraView
	{
		public LayoutPartCameraView()
		{
			InitializeComponent();
			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
		}

		private void OnUnloaded(object sender, RoutedEventArgs routedEventArgs)
		{
			VlcControl = new VlcControlView();
		}

		private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
			var layoutPartCameraViewModel = DataContext as LayoutPartCameraViewModel;
			if (layoutPartCameraViewModel != null)
			{
				var vlcControlViewModel = VlcControlHelper.VlcControlViewModels.FirstOrDefault(x => x.RviRTSP == layoutPartCameraViewModel.Camera.RviRTSP);
				if (vlcControlViewModel != null)
				{
					VlcControl.DataContext = vlcControlViewModel;
					vlcControlViewModel.Start();
				}
			}
		}

		private void UIElement_OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			var curentCameraViewModel = DataContext as LayoutPartCameraViewModel;
			if ((curentCameraViewModel == null) || (curentCameraViewModel.CameraViewModel == null))
				return;
			//if (((bool)e.NewValue) && (!curentCameraViewModel.CameraViewModel.IsNowPlaying))
			//	curentCameraViewModel.CameraViewModel.StartVideo();
			//if (((bool)e.OldValue) && (curentCameraViewModel.CameraViewModel.IsNowPlaying))
			//	curentCameraViewModel.CameraViewModel.StopVideo();
		}
	}
}