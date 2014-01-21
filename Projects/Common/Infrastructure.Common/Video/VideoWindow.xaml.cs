using System.Windows;
using Vlc.DotNet.Core.Medias;
using Vlc.DotNet.Forms;

namespace Infrastructure.Common
{
	public partial class VideoWindow : Window
	{
		static VideoWindow()
		{
			VideoService.Open();
		}

		public VideoWindow()
		{
			InitializeComponent();
			Loaded += new RoutedEventHandler(OnLoaded);
		}

		public string Address { get; set; }

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			var vlcControl = new VlcControl();
			 var rstpAddress = "rtsp://" + Address + "/video.h264";
			var locationMedia = new LocationMedia(rstpAddress);
			vlcControl.Media = locationMedia;
			_windowsFormsHost.Child = vlcControl;
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}