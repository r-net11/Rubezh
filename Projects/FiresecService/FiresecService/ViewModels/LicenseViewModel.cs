﻿using FiresecLicense;
using FiresecService.Processor;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Microsoft.Win32;
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

		FiresecLicenseInfo _licenseInfo;
		public FiresecLicenseInfo LicenseInfo
		{
			get { return _licenseInfo; }
			set 
			{ 
				_licenseInfo = value;
				OnPropertyChanged(() => LicenseInfo);
			}
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
				if (!FiresecLicenseManager.CheckLicense(openFileDialog.FileName))
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
				FiresecLicenseProcessor.SetLicense(FiresecLicenseManager.TryLoad(GetLicensePath()));
            }
        }
        
        public LicenseViewModel()
        {
			InitialKey = FiresecLicenseManager.InitialKey.ToString();
			LicenseInfo = FiresecLicenseManager.CurrentLicenseInfo;
			FiresecLicenseManager.LicenseChanged += FiresecLicenseManager_LicenseChanged;
            LoadLicenseCommand = new RelayCommand(OnLoadLicenseCommand);
        }

		void FiresecLicenseManager_LicenseChanged()
		{
			LicenseInfo = FiresecLicenseManager.CurrentLicenseInfo;
		}
    }
}
