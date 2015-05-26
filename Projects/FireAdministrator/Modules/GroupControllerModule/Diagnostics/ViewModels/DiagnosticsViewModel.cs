using System.Collections.Generic;
using System.Linq;
using FiresecAPI.GK;
using FiresecClient;
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
			var doorNo = 1;
			var shleifDevices = GKManager.Devices.Where(x => x.DriverType == GKDriverType.RSR2_KAU_Shleif);
			foreach (var shleifDevice in shleifDevices)
			{
				shleifDevice.Children = new List<GKDevice>();
				for (int i = 0; i < 80; i++)
				{
					var cardReaderDriver = GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_CardReader);
					var rmDriver = GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_RM_1);

					var enterDevice = new GKDevice();
					enterDevice.DriverUID = cardReaderDriver.UID;
					enterDevice.IntAddress = (byte)(3 * i + 1);
					enterDevice.Description = "ТД " + doorNo + " вход";
					shleifDevice.Children.Add(enterDevice);

					var exitDevice = new GKDevice();
					exitDevice.DriverUID = cardReaderDriver.UID;
					exitDevice.IntAddress = (byte)(3 * i + 2);
					exitDevice.Description = "ТД " + doorNo + " выход";
					shleifDevice.Children.Add(exitDevice);

					var lockDevice = new GKDevice();
					lockDevice.DriverUID = rmDriver.UID;
					lockDevice.IntAddress = (byte)(3 * i + 3);
					lockDevice.Description = "ТД " + doorNo + " замок";
					shleifDevice.Children.Add(lockDevice);

					var door = new GKDoor();
					door.No = doorNo;
					door.Name = "ТД " + doorNo;
					door.EnterDeviceUID = enterDevice.UID;
					door.ExitDeviceUID = exitDevice.UID;
					door.LockDeviceUID = lockDevice.UID;
					GKManager.Doors.Add(door);

					doorNo++;
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