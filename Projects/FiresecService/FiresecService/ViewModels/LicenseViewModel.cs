using Defender;
using FiresecService.Processor;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace FiresecService.ViewModels
{
    public class LicenseViewModel : BaseViewModel
    {
        string _initialKeyString;
        public string InitialKeyString
        {
            get { return _initialKeyString; }
            set
            {
                _initialKeyString = value;
                OnPropertyChanged(() => InitialKeyString);
            }
        }

        ObservableCollection<LicenseParameter> _parameters;
        public ObservableCollection<LicenseParameter> Parameters
        {
            get { return _parameters; }
            set
            {
                _parameters = value;
                OnPropertyChanged(() => Parameters);
            }
        }

        string GetLicensePath()
        {
            return AppDataFolderHelper.GetFile("FiresecService.license");
        }

        bool TryLoadLicense()
        {
			bool success = FiresecLicenseProcessor.TryLoadLicense();
			Parameters = success ? 
				new ObservableCollection<LicenseParameter>(FiresecLicenseProcessor.License.Parameters.Where(x => x.Id != "version")) : 
				new ObservableCollection<LicenseParameter>();
            return success;
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
                if (!FiresecLicenseProcessor.CheckLicense(openFileDialog.FileName))
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
                TryLoadLicense();
            }
        }
        
        public LicenseViewModel()
        {
            InitialKeyString = FiresecLicenseProcessor.InitialKey.ToString();
            LoadLicenseCommand = new RelayCommand(OnLoadLicenseCommand);
            TryLoadLicense();
        }
    }
}
