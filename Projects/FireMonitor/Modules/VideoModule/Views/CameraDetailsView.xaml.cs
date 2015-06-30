using System.Windows;
using FiresecAPI.Models;
using VideoModule.ViewModels;
using Vlc.DotNet.Core.Medias;

namespace VideoModule.Views
{
	public partial class CameraDetailsView
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
			if (myVlcControl.IsPlaying)
				myVlcControl.Stop();
		}

		private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
			var cameraDetailsViewModel = DataContext as CameraDetailsViewModel;
			if (cameraDetailsViewModel != null)
			{
				myVlcControl.Media = new LocationMedia(cameraDetailsViewModel.RviRTSP);
				if (!myVlcControl.IsPlaying)
					myVlcControl.Play();
			}
		}
	}
}