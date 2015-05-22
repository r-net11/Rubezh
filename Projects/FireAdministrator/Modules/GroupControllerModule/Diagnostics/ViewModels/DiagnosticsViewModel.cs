using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Common;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecClient;
using GKProcessor;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;

namespace GKModule.ViewModels
{
	public class DiagnosticsViewModel : ViewPartViewModel
	{
		public DiagnosticsViewModel()
		{
			TestCommand = new RelayCommand(OnTest);
			ShowPlotCommand = new RelayCommand(OnShowPlot);
		}

		public RelayCommand TestCommand { get; private set; }
		void OnTest()
		{
			var shleifDevices = GKManager.Devices.Where(x => x.DriverType == GKDriverType.RSR2_KAU_Shleif);
			foreach (var shleifDevice in shleifDevices)
			{
				shleifDevice.Children = new List<GKDevice>();
				for (int i = 1; i < 250; i++)
				{
					var driver = GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_SmokeDetector);
					var device = new GKDevice();
					device.DriverUID = driver.UID;
					device.IntAddress = (byte)i;
					shleifDevice.Children.Add(device);
				}
			}

			GKManager.UpdateConfiguration();
			ServiceFactory.Events.GetEvent<ConfigurationChangedEvent>().Publish(null);
			ServiceFactory.SaveService.GKChanged = true;
		}

		public RelayCommand ShowPlotCommand { get; private set; }
		void OnShowPlot()
		{
			var plotViewModel = new PlotViewModel();
			DialogService.ShowModalWindow(plotViewModel);
		}
	}
}