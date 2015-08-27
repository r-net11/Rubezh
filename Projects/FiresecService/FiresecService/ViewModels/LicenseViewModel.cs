using Defender;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace FiresecService.ViewModels
{
    public class LicenseViewModel : BaseViewModel
    {
        License _license;

        InitialKey _initialKey;
        
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
            LicenseHelper.License = _license = LicenseProcessor.ProcessLoad(GetLicensePath(), _initialKey);
            Parameters = _license == null ? new ObservableCollection<LicenseParameter>() : new ObservableCollection<LicenseParameter>(_license.Parameters);
            return _license != null;
        }

        bool CheckLicense(string path)
        {
            var license = LicenseProcessor.ProcessLoad(path, _initialKey);
            return license != null && license.InitialKey == _initialKey;
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
                if (!CheckLicense(openFileDialog.FileName))
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
            _initialKey = InitialKey.Generate();
            InitialKeyString = _initialKey.ToString();
            LoadLicenseCommand = new RelayCommand(OnLoadLicenseCommand);
            TryLoadLicense();
        }
    }
}
