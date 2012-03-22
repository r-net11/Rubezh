using System.Windows;
using Vlc.DotNet.Core;
using Vlc.DotNet.Core.Medias;
using Vlc.DotNet.Forms;

namespace VideoModule.Views
{
    public partial class VideoWindow : Window
    {
        static VideoWindow()
        {
            VlcContext.LibVlcDllsPath = @"C:\Program Files\VideoLAN\VLC";
            VlcContext.LibVlcPluginsPath = @"C:\Program Files\VideoLAN\VLC\pugins";
            VlcContext.StartupOptions.IgnoreConfig = false;
            VlcContext.StartupOptions.LogOptions.LogInFile = false;
            VlcContext.StartupOptions.LogOptions.ShowLoggerConsole = false;
            VlcContext.Initialize();
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
            var rstpAddress = "rtsp://" + Address + "/snl/live/1/1/";
            var locationMedia = new LocationMedia(rstpAddress);
            vlcControl.Media = locationMedia;
            _windowsFormsHost.Child = vlcControl;
        }
    }
}