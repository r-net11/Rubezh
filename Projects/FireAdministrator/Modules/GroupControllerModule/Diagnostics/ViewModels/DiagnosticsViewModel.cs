using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
using Infrastructure;
using Common.GK;
using GKModule.Converter;
using Infrastructure.Common.Windows;
using System.IO;
using FiresecClient;
using XFiresecAPI;
using FiresecAPI.Models;
using Infrastructure.Events;
using GKModule.Diagnostics;

namespace GKModule.ViewModels
{
	public class DiagnosticsViewModel : ViewPartViewModel
	{
		public DiagnosticsViewModel()
		{
			ConvertToBinCommand = new RelayCommand(OnConvertToBin);
			ConvertToBinaryFileCommand = new RelayCommand(OnConvertToBinaryFile);
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

		public RelayCommand ConvertToBinaryFileCommand { get; private set; }
		void OnConvertToBinaryFile()
		{
			Directory.Delete(@"C:\GKConfig", true);
			Directory.CreateDirectory(@"C:\GKConfig");
			BinaryFileConverter.Convert();
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