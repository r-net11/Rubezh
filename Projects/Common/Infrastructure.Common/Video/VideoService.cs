using System.Collections.Generic;
using System.Windows;
using FiresecAPI.Models;
using Vlc.DotNet.Core;
using Common;
using System;

namespace Infrastructure.Common
{
	public static class VideoService
	{
		static List<string> ActiveAddresses = new List<string>();
		static bool IsInitialized;
		internal static string _dllPath;

		public static void Initialize(string dllPath)
		{
			_dllPath = dllPath;
		}

		public static void Show(Camera camera)
		{
			try
			{
				var videoWindow = new VideoWindow()
				{
					Title = "Видео с камеры " + camera.Address,
					Address = camera.Address,
					FullAddress = camera.FullAddress,
					Left = camera.Left,
					Top = camera.Top,
					Width = camera.Width,
					Height = camera.Height
				};
				if (camera.IgnoreMoveResize)
				{
					videoWindow.WindowStyle = System.Windows.WindowStyle.None;
					videoWindow.MinHeight = videoWindow.MaxHeight = videoWindow.Height;
					videoWindow.MinWidth = videoWindow.MaxWidth = videoWindow.Width;
					videoWindow._title.Text = videoWindow.Title;
					videoWindow._headerGrid.Visibility = Visibility.Visible;
				}

				if (ActiveAddresses.Contains(camera.Address) == false)
				{
					ActiveAddresses.Add(camera.Address);
					videoWindow.Closed += new System.EventHandler(videoWindow_Closed);
					videoWindow.Show();
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "VideoService.Show");
			}
		}

		static void videoWindow_Closed(object sender, System.EventArgs e)
		{
			var address = (sender as VideoWindow).Address;
			ActiveAddresses.Remove(address);
		}

		public static void ShowModal(Camera camera)
		{
			try
			{
				var videoWindow = new VideoWindow()
				{
					Title = "Видео с камеры " + camera.Address,
					Address = camera.Address,
					FullAddress = camera.FullAddress,
					Left = camera.Left,
					Top = camera.Top,
					Width = camera.Width,
					Height = camera.Height
				};
				videoWindow.ShowDialog();

				camera.Left = (int)videoWindow.Left;
				camera.Top = (int)videoWindow.Top;
				camera.Width = (int)videoWindow.Width;
				camera.Height = (int)videoWindow.Height;
			}
			catch (Exception e)
			{
				Logger.Error(e, "VideoService.ShowModal");
			}
		}

		internal static void Open()
		{
			IsInitialized = true;
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
			if (IsInitialized)
			{
				if (VlcContext.IsInitialized)
					VlcContext.CloseAll();
			}
		}
	}
}