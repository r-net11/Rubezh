using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Vlc.DotNet.Core;
using Vlc.DotNet.Core.Medias;

namespace DiagnosticsModule.Views
{
	public partial class DiagnosticsView
	{
		public DiagnosticsView()
		{
			InitializeComponent();
			return;

			//Important!!!
			//Set libvlc.dll and libvlccore.dll directory path
			VlcContext.LibVlcDllsPath = "C:\\Program Files\\VideoLAN\\VLC\\";
			//Set the vlc plugins directory path
			VlcContext.LibVlcPluginsPath = "C:\\Program Files\\VideoLAN\\VLC\\plugins\\";

			//Set the startup options
			VlcContext.StartupOptions.IgnoreConfig = true;
			VlcContext.StartupOptions.LogOptions.LogInFile = false;
			VlcContext.StartupOptions.LogOptions.ShowLoggerConsole = true;
			VlcContext.StartupOptions.LogOptions.Verbosity = VlcLogVerbosities.Debug;

			//Initialize the VlcContext
			VlcContext.Initialize();
			myVlcControl.VideoProperties.Scale = 2.0f;
		}

		private void MainWindowOnClosing(object sender, CancelEventArgs e)
		{
			// Close the context. 
			VlcContext.CloseAll();
		}

		private void buttonRtsp_Click(object sender, RoutedEventArgs e)
		{
			myVlcControl.Stop();
			myVlcControl.Media = new LocationMedia("rtsp://admin:admin@172.16.2.23:554/cam/realmonitor?channel=1&subtype=0");
			myVlcControl.Play();
		}
	}
}