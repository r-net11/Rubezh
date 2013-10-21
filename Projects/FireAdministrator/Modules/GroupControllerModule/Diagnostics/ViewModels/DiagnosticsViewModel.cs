using System.IO;
using System.Linq;
using Common.GK;
using GKModule.Converter;
using GKModule.Diagnostics;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using FiresecClient;
using XFiresecAPI;
using System.Collections.Generic;
using Infrastructure.Events;

namespace GKModule.ViewModels
{
	public class DiagnosticsViewModel : ViewPartViewModel
	{
		public DiagnosticsViewModel()
		{
			ConvertToBinCommand = new RelayCommand(OnConvertToBin);
			ConvertFromFiresecCommand = new RelayCommand(OnConvertFromFiresec);
			ConvertToFiresecCommand = new RelayCommand(OnConvertToFiresec);
			GoToTechnologicalCommand = new RelayCommand(OnGoToTechnological);
			GoToWorkRegimeCommand = new RelayCommand(OnGoToWorkRegime);
			CreateTestZonesCommand = new RelayCommand(OnCreateTestZones);
			ConvertShleifCommand = new RelayCommand(OnConvertShleif);
		}

		public RelayCommand ConvertFromFiresecCommand { get; private set; }
		void OnConvertFromFiresec()
		{
			var configurationConverter = new FiresecToGKConverter();
			configurationConverter.Convert();

			DevicesViewModel.Current.Initialize();
			ZonesViewModel.Current.Initialize();
			ServiceFactory.SaveService.GKChanged = true;
		}

		public RelayCommand ConvertToBinCommand { get; private set; }
		void OnConvertToBin()
		{
			DatabaseManager.Convert();
			var databasesViewModel = new DatabasesViewModel();
			DialogService.ShowModalWindow(databasesViewModel);
		}

		public RelayCommand ConvertToFiresecCommand { get; private set; }
		void OnConvertToFiresec()
		{
			var gkToFiresecConverter = new GKToFiresecConverter();
			gkToFiresecConverter.Convert();
			ServiceFactory.SaveService.FSChanged = true;
		}

		public RelayCommand GoToTechnologicalCommand { get; private set; }
		void OnGoToTechnological()
		{
			var device = XManager.Devices.FirstOrDefault(x => x.Driver.DriverType == XFiresecAPI.XDriverType.GK);
			var sendResult = SendManager.Send(device, 0, 14, 0, null, device.Driver.DriverType == XDriverType.GK);
		}

		public RelayCommand GoToWorkRegimeCommand { get; private set; }
		void OnGoToWorkRegime()
		{
			var device = XManager.Devices.FirstOrDefault(x => x.Driver.DriverType == XFiresecAPI.XDriverType.GK);
			SendManager.Send(device, 0, 11, 0, null, device.Driver.DriverType == XDriverType.GK);
		}

		public RelayCommand CreateTestZonesCommand { get; private set; }
		void OnCreateTestZones()
		{
			var device = XManager.Devices.FirstOrDefault(x => x.Driver.DriverType == XDriverType.HandDetector);
			for (int i = 0; i < 20000; i++)
			{
				var zone = new XZone()
				{
					No = 10000 + i,
					Name = "TestZone_" + i
				};
				XManager.Zones.Add(zone);
				device.ZoneUIDs.Add(zone.UID);
			}
			ServiceFactory.SaveService.GKChanged = true;
		}

		public RelayCommand ConvertShleifCommand { get; private set; }
		void OnConvertShleif()
		{
			PatchShleif(XDriverType.KAU, XDriverType.KAU_Shleif);
			PatchShleif(XDriverType.RSR2_KAU, XDriverType.RSR2_KAU_Shleif);
			XManager.UpdateConfiguration();
			ServiceFactory.Events.GetEvent<ConfigurationChangedEvent>().Publish(null);
		}

		void PatchShleif(XDriverType kauDriverType, XDriverType shleifDriverType)
		{
			var kauDevices = XManager.Devices.Where(x => x.Driver.DriverType == kauDriverType);
			foreach (var kauDevice in kauDevices)
			{
				var driver = XManager.Drivers.FirstOrDefault(x => x.DriverType == shleifDriverType);
				var shleifDevices = new List<XDevice>();
				for (int i = 0; i < 8; i++)
				{
					var device = new XDevice()
					{
						IntAddress = (byte)(i + 1),
						Driver = driver,
						DriverUID = driver.UID
					};
					shleifDevices.Add(device);
				}
				foreach (var device in kauDevice.Children)
				{
					if (device.Driver.DriverType != XDriverType.KAUIndicator)
					{
						var shleifDevice = shleifDevices.FirstOrDefault(x => x.IntAddress == device.ShleifNo);
						shleifDevice.Children.Add(device);
					}
				}
				kauDevice.Children.RemoveAll(x => x.Driver.DriverType != XDriverType.KAUIndicator);
				kauDevice.Children.AddRange(shleifDevices);
			}
		}
	}
}