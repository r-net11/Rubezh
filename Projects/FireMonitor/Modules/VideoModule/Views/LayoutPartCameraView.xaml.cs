using System.Windows;
using VideoModule.ViewModels;

namespace VideoModule.Views
{
	public partial class LayoutPartCameraView
	{
		public LayoutPartCameraView()
		{
			InitializeComponent();
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