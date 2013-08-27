using System.IO;
using Common.GK;
using GKModule.Converter;
using GKModule.Diagnostics;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class DiagnosticsViewModel : ViewPartViewModel
	{
		public DiagnosticsViewModel()
		{
			ConvertToBinCommand = new RelayCommand(OnConvertToBin);
			ConvertFromFiresecCommand = new RelayCommand(OnConvertFromFiresec);
			ConvertToFiresecCommand = new RelayCommand(OnConvertToFiresec);
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
	}
}