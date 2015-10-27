﻿using RubezhLicense;
using FiresecService.Processor;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Microsoft.Win32;
using RubezhAPI.License;
using System;
using System.IO;

namespace FiresecService.ViewModels
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

		LicenseInfo _licenseInfo;
		public LicenseInfo LicenseInfo
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
			InitialKey = LicenseManager.FiresecLicenseManager.InitialKey.ToString();
			LicenseInfo = LicenseManager.FiresecLicenseManager.CurrentLicenseInfo;
			LicenseManager.FiresecLicenseManager.LicenseChanged += FiresecLicenseManager_LicenseChanged;
			LoadLicenseCommand = new RelayCommand(OnLoadLicenseCommand);
		}
		
        string GetLicensePath()
        {
            return AppDataFolderHelper.GetFile("FiresecService.license");
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
				if (!LicenseManager.FiresecLicenseManager.CheckLicense(openFileDialog.FileName))
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
				FiresecLicenseProcessor.SetLicense(LicenseManager.FiresecLicenseManager.TryLoad(GetLicensePath()));
            }
        }
       
		void FiresecLicenseManager_LicenseChanged()
		{
			LicenseInfo = LicenseManager.FiresecLicenseManager.CurrentLicenseInfo;
		}
    }
}
