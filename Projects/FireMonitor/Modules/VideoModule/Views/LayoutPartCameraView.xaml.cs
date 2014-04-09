using System.Windows;
using VideoModule.ViewModels;

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
			_grid.Child = new UIElement();
		}

		private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
			var layoutPartCameraViewModel = DataContext as LayoutPartCameraViewModel;
			_grid.Child = layoutPartCameraViewModel.CellPlayerWrap;
		}

		private void UIElement_OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			var curentCameraViewModel = DataContext as LayoutPartCameraViewModel;
			if ((curentCameraViewModel == null) || (curentCameraViewModel.CameraViewModel == null))
				return;
			//if (((bool)e.NewValue) && (!curentCameraViewModel.CameraViewModel.IsNowPlaying))
			//    curentCameraViewModel.CameraViewModel.StartVideo();
			//if (((bool)e.OldValue) && (curentCameraViewModel.CameraViewModel.IsNowPlaying))
			//    curentCameraViewModel.CameraViewModel.StopVideo();
		}
	}
}