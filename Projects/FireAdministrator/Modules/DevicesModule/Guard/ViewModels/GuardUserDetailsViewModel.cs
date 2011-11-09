using System.Text;
using System.Linq;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;
using FiresecClient;

namespace DevicesModule.ViewModels
{
    public class GuardUserDetailsViewModel : SaveCancelDialogContent
    {
        public GuardUser GuardUser { get; private set; }
        bool _isNew;

        public GuardUserDetailsViewModel(GuardUser guardUser = null)
        {
            ChangeGuardLevelsCommand = new RelayCommand(OnChangeGuardLevels);
            ChangeDevicesCommand = new RelayCommand(OnChangeDevices);

            if (guardUser == null)
            {
                Title = "Создать пользователя";
                _isNew = true;
                GuardUser = new GuardUser();
            }
            else
            {
                Title = "Редактировать пользователя";
                _isNew = false;
                GuardUser = guardUser;
            }

            CopyProperies();
        }

        void CopyProperies()
        {
            Name = GuardUser.Name;
            Password = GuardUser.Password;
            FIO = GuardUser.FIO;
            Function = GuardUser.Function;
            CanSetZone = GuardUser.CanSetZone;
            CanUnSetZone = GuardUser.CanUnSetZone;
        }

        void SaveProperies()
        {
            GuardUser.Name = Name;
            GuardUser.Password = Password;
            GuardUser.FIO = FIO;
            GuardUser.Function = Function;
            GuardUser.CanSetZone = CanSetZone;
            GuardUser.CanUnSetZone = CanUnSetZone;
        }

        string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged("Password");
            }
        }

        string _FIO;
        public string FIO
        {
            get { return _FIO; }
            set
            {
                _FIO = value;
                OnPropertyChanged("FIO");
            }
        }

        string _function;
        public string Function
        {
            get { return _function; }
            set
            {
                _function = value;
                OnPropertyChanged("Function");
            }
        }

        bool _canSetZone;
        public bool CanSetZone
        {
            get { return _canSetZone; }
            set
            {
                _canSetZone = value;
                OnPropertyChanged("CanSetZone");
            }
        }

        bool _canUnSetZone;
        public bool CanUnSetZone
        {
            get { return _canUnSetZone; }
            set
            {
                _canUnSetZone = value;
                OnPropertyChanged("CanUnSetZone");
            }
        }

        public string PresentationLevels
        {
            get
            {
                string presentationNames = "";
                foreach (var guardLevelName in GuardUser.GuardLevelNames)
                {
                    presentationNames += guardLevelName + ", ";
                }
                return presentationNames.TrimEnd(',', ' ');
            }
        }

        public RelayCommand ChangeGuardLevelsCommand { get; private set; }
        void OnChangeGuardLevels()
        {
            var guardLevelsSelectationViewModel = new GuardLevelsSelectationViewModel();
            guardLevelsSelectationViewModel.Initialize(GuardUser);
            if (ServiceFactory.UserDialogs.ShowModalWindow(guardLevelsSelectationViewModel))
            {
                GuardUser.GuardLevelNames = guardLevelsSelectationViewModel.GuardLevels;
                OnPropertyChanged("PresentationLevels");
            }
        }

        public string PresentationDevices
        {
            get
            {
                string presentationNames = "";
                foreach (var deviceUID in GuardUser.Devices)
                {
                    var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == deviceUID);
                    presentationNames += device.Driver.ShortName + " " + device.DottedAddress + ", ";
                }
                return presentationNames.TrimEnd(',', ' ');
            }
        }

        public RelayCommand ChangeDevicesCommand { get; private set; }
        void OnChangeDevices()
        {
            var guardDevicesSelectationViewModel = new GuardDevicesSelectationViewModel(GuardUser);
            if (ServiceFactory.UserDialogs.ShowModalWindow(guardDevicesSelectationViewModel))
            {
                //GuardUser.GuardLevelNames = guardDevicesSelectationViewModel.GuardLevels;
                OnPropertyChanged("PresentationDevices");
            }
        }

        protected override void Save(ref bool cancel)
        {
            SaveProperies();
        }
    }
}