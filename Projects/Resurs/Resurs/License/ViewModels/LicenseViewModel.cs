using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using Microsoft.Win32;
using ResursAPI.License;
using System;
using System.IO;
using ResursAPI;

namespace Resurs.ViewModels
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

		ResursLicenseInfo _licenseInfo;
		public ResursLicenseInfo LicenseInfo
		{
			get { return _licenseInfo; }
			set 
			{ 
				_licenseInfo = value;
				OnPropertyChanged(() => LicenseInfo);
			}
		}

		public string LicenseStatus
		{
			get { return _licenseInfo == null ? "Лицензия получена" : "Лицензия отсутствует"; }
		}

		public LicenseViewModel()
		{
			InitialKey = LicenseManager.InitialKey.ToString();
			LicenseManager.LicenseChanged += FiresecLicenseManager_LicenseChanged;
			LicenseInfo = LicenseManager.CurrentLicenseInfo;
			LoadLicenseCommand = new RelayCommand(OnLoadLicenseCommand);
		}
		
        string GetLicensePath()
        {
            return FolderHelper.GetFile("Resurs.license");
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
				LicenseManager.CurrentLicenseInfo = LicenseManager.TryLoad(GetLicensePath());
            }
        }
       
		void FiresecLicenseManager_LicenseChanged()
		{
			LicenseInfo = LicenseManager.CurrentLicenseInfo;
			OnPropertyChanged(() => LicenseStatus);
		}
    }
}
