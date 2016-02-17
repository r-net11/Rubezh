using RubezhLicense;
using FiresecService.Processor;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Microsoft.Win32;
using RubezhAPI.License;
using System;
using System.IO;
using System.Windows.Forms;

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

		public LicenseViewModel()
		{
			InitialKey = LicenseManager.InitialKey.ToString();
			LicenseInfo = LicenseManager.CurrentLicenseInfo;
			LicenseManager.LicenseChanged += FiresecLicenseManager_LicenseChanged;
		}
		
        string GetLicensePath()
        {
            return AppDataFolderHelper.GetFile("FiresecService.license");
        }

        public void OnLoadLicenseCommand()
        {
            var openFileDialog = new OpenFileDialog()
            {
                Filter = "Файл лицензии (*.license)|*.license"
            };
            if (DialogResult.OK == openFileDialog.ShowDialog())
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
				FiresecLicenseProcessor.SetLicense(LicenseManager.TryLoad(GetLicensePath()));
            }
        }
       
		void FiresecLicenseManager_LicenseChanged()
		{
			LicenseInfo = LicenseManager.CurrentLicenseInfo;
		}
    }
}
