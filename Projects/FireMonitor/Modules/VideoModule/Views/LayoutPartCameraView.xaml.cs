using System.Windows;
using VideoModule.ViewModels;
using Vlc.DotNet.Core.Medias;

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
			if (myVlcControl.IsPlaying)
				myVlcControl.Stop();
		}

		private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
			var layoutPartCameraViewModel = DataContext as LayoutPartCameraViewModel;
			if (layoutPartCameraViewModel != null)
			{
				myVlcControl.Media = new LocationMedia(layoutPartCameraViewModel.RviRTSP);
				if (!myVlcControl.IsPlaying)
					myVlcControl.Play();
			}
		}
	}
}