using Vlc.DotNet.Core;
using System.Collections.Generic;

namespace Infrastructure.Common
{
	public static class VideoService
	{
		static List<string> ActiveAddresses = new List<string>();

		public static void Show(string address)
		{
			var videoWindow = new VideoWindow()
			{
				Title = "Видео с камеры " + address,
				Address = address
			};
			if (ActiveAddresses.Contains(address) == false)
			{
				ActiveAddresses.Add(address);
				videoWindow.Closed += new System.EventHandler(videoWindow_Closed);
				videoWindow.Show();
			}
		}

		static void videoWindow_Closed(object sender, System.EventArgs e)
		{
			var address = (sender as VideoWindow).Address;
			ActiveAddresses.Remove(address);
		}

		public static void ShowModal(string address)
		{
			var videoWindow = new VideoWindow()
			{
				Title = "Видео с камеры " + address,
				Address = address
			};
			videoWindow.ShowDialog();
		}

		internal static string _dllPath;

		public static void Initialize(string dllPath)
		{
			_dllPath = dllPath;
		}

		internal static void Open()
		{
			if (VlcContext.IsInitialized == false)
			{
				VlcContext.LibVlcDllsPath = _dllPath;
				//VlcContext.LibVlcDllsPath = @"C:\Program Files\VideoLAN\VLC";
				//VlcContext.LibVlcPluginsPath = @"C:\Program Files\VideoLAN\VLC\pugins";
				VlcContext.StartupOptions.IgnoreConfig = false;
				VlcContext.StartupOptions.LogOptions.LogInFile = false;
				VlcContext.StartupOptions.LogOptions.ShowLoggerConsole = false;
				VlcContext.Initialize();
			}
		}

		public static void Close()
		{
			if (VlcContext.IsInitialized)
				VlcContext.CloseAll();
		}
	}
}