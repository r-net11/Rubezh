using System;
using System.Collections.Generic;
using System.Windows;
using Common;
using FiresecAPI.Models;
using Vlc.DotNet.Core;

namespace Infrastructure.Common
{
	public static class VideoService
	{
		static List<string> ActiveAddresses = new List<string>();
		static bool IsInitialized;

		public static void Show(Camera camera)
		{
			try
			{
				var videoWindow = CreateWindow(camera);
				if (camera.IgnoreMoveResize)
				{
					videoWindow.WindowStyle = System.Windows.WindowStyle.None;
					videoWindow.MinHeight = videoWindow.MaxHeight = videoWindow.Height;
					videoWindow.MinWidth = videoWindow.MaxWidth = videoWindow.Width;
					videoWindow._title.Text = videoWindow.Title;
					videoWindow._headerGrid.Visibility = Visibility.Visible;
				}

				if (ActiveAddresses.Contains(camera.Ip) == false)
				{
					ActiveAddresses.Add(camera.Ip);
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
				var videoWindow = CreateWindow(camera);
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

		static VideoWindow CreateWindow(Camera camera)
		{
			var title = camera.Name;
			if(string.IsNullOrEmpty(title))
				title = "Видео с камеры " + camera.Ip;

			var videoWindow = new VideoWindow()
			{
				Title = title,
				Address = camera.Ip,
				Left = camera.Left,
				Top = camera.Top,
				Width = camera.Width,
				Height = camera.Height
			};
			return videoWindow;
		}

		internal static void Open()
		{
			IsInitialized = true;
			if (VlcContext.IsInitialized == false)
			{
				VlcContext.LibVlcDllsPath = GlobalSettingsHelper.GlobalSettings.LibVlcDllsPath;
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