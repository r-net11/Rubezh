using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using MuliclientAPI;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;

namespace MultiClientRunner
{
	public class AppItem : BaseViewModel
	{
		public AppItem()
		{
			SelectCommand = new RelayCommand(OnSelect);
		}

		Process Process;
		public string Name { get; private set; }
		public string ClientId { get; set; }

		public void Run(MulticlientData multiclientData)
		{
			Name = multiclientData.Name;
			var commandLineArguments = "regime='multiclient' " +
				"ClientId='" + ClientId +
				"' RemoteAddress='" + multiclientData.RemoteAddress +
				"' RemotePort='" + multiclientData.RemotePort.ToString() +
				"' login='" + multiclientData.Login +
				"' password='" + multiclientData.Password + "'";

			var processStartInfo = new ProcessStartInfo()
			{
				FileName = @"E:/Projects/Projects/FireMonitor/bin/Debug/FireMonitor.exe",
				Arguments = commandLineArguments
			};
			Process = System.Diagnostics.Process.Start(processStartInfo);
		}

		public void Kill()
		{
			Process.Kill();
		}

		public void Hide()
		{
			MulticlientServer.Muliclient.Hide(ClientId);
		}

		public void Show(WindowSize windowSize)
		{
			if (windowSize != null)
			{
				MulticlientServer.Muliclient.SetWindowSize(ClientId, windowSize);
			}
			MulticlientServer.Muliclient.Show(ClientId);
		}

		public WindowSize GetWindowSize()
		{
			return MulticlientServer.Muliclient.GetWindowSize(ClientId);
		}

		public RelayCommand SelectCommand { get; private set; }
		void OnSelect()
		{
            //if (AppItemsViewModels.Current.CurrentAppItem == this)
            //    return;

            //WindowSize windowSize = null;
            //if (AppItemsViewModels.Current.CurrentAppItem != null)
            //{
            //    windowSize = AppItemsViewModels.Current.CurrentAppItem.GetWindowSize();
            //}

            //Show(windowSize);
            //foreach (var appItem in AppItemsViewModels.Current.AppItems)
            //{
            //    if (appItem != this)
            //        appItem.Hide();
            //}

            //AppItemsViewModels.Current.CurrentAppItem = this;
		}
	}
}