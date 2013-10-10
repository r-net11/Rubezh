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
	}
}