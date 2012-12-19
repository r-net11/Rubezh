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
		public MulticlientData MulticlientData { get; private set; }

		public void Run(MulticlientData multiclientData)
		{
			MulticlientData = multiclientData;
			var commandLineArguments = "regime='multiclient' " +
				"ClientId='" + multiclientData.Id +
				"' RemoteAddress='" + multiclientData.Address +
				"' RemotePort='" + multiclientData.Port.ToString() +
				"' login='" + multiclientData.Login +
				"' password='" + multiclientData.Password + "'";

			var processStartInfo = new ProcessStartInfo()
			{
				FileName = @"D:/Projects/Projects/FireMonitor/bin/Debug/FireMonitor.exe",
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
			MulticlientServer.Muliclient.Hide(MulticlientData.Id);
		}

		public void Show(WindowSize windowSize)
		{
			if (windowSize != null)
			{
				MulticlientServer.Muliclient.SetWindowSize(MulticlientData.Id, windowSize);
			}
			MulticlientServer.Muliclient.Show(MulticlientData.Id);
		}

		public WindowSize GetWindowSize()
		{
			return MulticlientServer.Muliclient.GetWindowSize(MulticlientData.Id);
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