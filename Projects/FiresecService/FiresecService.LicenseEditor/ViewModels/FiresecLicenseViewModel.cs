using FiresecService.LicenseEditor.Models;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiresecService.LicenseEditor.ViewModels
{
    public class FiresecLicenseViewModel : ApplicationViewModel
    {
        FiresecLicense firesecLicense;

        public FiresecLicenseViewModel()
        {
            firesecLicense = new FiresecLicense();
            LoadLicenseCommand = new RelayCommand(OnLoadLicense);
            SaveLicenseCommand = new RelayCommand(OnSaveLicense);
        }

        public string InitialKeyString
        {
            get { return firesecLicense.InitialKeyString; }
            set 
            {
                firesecLicense.InitialKeyString = value; 
                OnPropertyChanged(() => InitialKeyString);
            }
        }

        public int NumberOfUsers
        {
            get { return firesecLicense.NumberOfUsers; }
            set 
            {
                firesecLicense.NumberOfUsers = value;
                OnPropertyChanged(() => NumberOfUsers);
            }
        }

        public bool FireAlarm
        {
            get { return firesecLicense.FireAlarm; }
            set 
            { 
                firesecLicense.FireAlarm = value;
                OnPropertyChanged(() => FireAlarm);
            }
        }

        public bool SecurityAlarm
        {
            get { return firesecLicense.SecurityAlarm; }
            set
            {
                firesecLicense.SecurityAlarm = value;
                OnPropertyChanged(() => SecurityAlarm);
            }
        }

        public bool Skd
        {
            get { return firesecLicense.Skd; }
            set
            {
                firesecLicense.Skd = value;
                OnPropertyChanged(() => Skd);
            }
        }

        public bool ControlScripts
        {
            get { return firesecLicense.ControlScripts; }
            set
            {
                firesecLicense.ControlScripts = value;
                OnPropertyChanged(() => ControlScripts);
            }
        }

        public bool OrsServer
        {
            get { return firesecLicense.OrsServer; }
            set
            {
                firesecLicense.OrsServer = value;
                OnPropertyChanged(() => OrsServer);
            }
        }

        public RelayCommand LoadLicenseCommand { get; set; }
        void OnLoadLicense()
        {
            var openFileDialog = new OpenFileDialog()
            {
                Filter = "Файл лицензии (*.license)|*.license"
            };
            if (openFileDialog.ShowDialog().Value)
            {
                firesecLicense.Load(openFileDialog.FileName);
            }
        }

        public RelayCommand SaveLicenseCommand { get; set; }
        void OnSaveLicense()
        {
            var saveFileDialog = new SaveFileDialog()
            {
                Filter = "Файл лицензии (*.license)|*.license"
            };
            if (saveFileDialog.ShowDialog().Value)
            {
                firesecLicense.Save(saveFileDialog.FileName);
            }
        }


    }
}
