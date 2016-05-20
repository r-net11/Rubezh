using RubezhService.Processor;
using Infrastructure.Common;
using Infrastructure.Common.License;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Microsoft.Win32;
using RubezhAPI.License;
using System;
using System.IO;

namespace RubezhService.Models
{
	public class LicenseViewModel : BaseViewModel
	{
		string _initialKey;
		public string InitialKey
		{
			get { return _initialKey; }
			set
			{
				_initialKey = value;
				OnPropertyChanged(() => InitialKey);
			}
		}

		RubezhLicenseInfo _licenseInfo;
		public RubezhLicenseInfo LicenseInfo
		{
			get { return _licenseInfo; }
			set
			{
				_licenseInfo = value;
				OnPropertyChanged(() => LicenseInfo);
			}
		}

		public LicenseViewModel()
		{
			InitialKey = LicenseManager.InitialKey.ToString();
			LicenseInfo = LicenseManager.CurrentLicenseInfo;
			LicenseManager.LicenseChanged += RubezhLicenseManager_LicenseChanged;
			LoadLicenseCommand = new RelayCommand(OnLoadLicenseCommand);
		}

		string GetLicensePath()
		{
			return AppDataFolderHelper.GetFile("RubezhService.license");
		}

		public RelayCommand LoadLicenseCommand { get; private set; }
		void OnLoadLicenseCommand()
		{
			var openFileDialog = new OpenFileDialog()
			{
				Filter = "Файл лицензии (*.license)|*.license"
			};
			if (openFileDialog.ShowDialog().Value)
			{
				if (!LicenseManager.CheckLicense(openFileDialog.FileName))
				{
					MessageBoxService.ShowError("Некорректный файл лицензии");
					return;
				}
				try
				{
					File.Copy(openFileDialog.FileName, GetLicensePath(), true);
				}
				catch (Exception e)
				{
					MessageBoxService.ShowError("Ошибка копирования файла лицензии.\n" + e.Message);
				}
				RubezhLicenseProcessor.SetLicense(LicenseManager.TryLoad(GetLicensePath()));
			}
		}

		void RubezhLicenseManager_LicenseChanged()
		{
			LicenseInfo = LicenseManager.CurrentLicenseInfo;
		}
	}
}
