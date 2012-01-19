using Controls;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class UserDetailsViewModel : SaveCancelDialogContent
    {
        public GuardUser GuardUser { get; private set; }

        public UserDetailsViewModel(GuardUser guardUser = null)
        {
            ChangeLevelsCommand = new RelayCommand(OnChangeLevels);

            if (guardUser == null)
            {
                Title = "Создать пользователя";
                GuardUser = new GuardUser();
            }
            else
            {
                Title = "Редактировать пользователя";
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
                foreach (var levelName in GuardUser.LevelNames)
                {
                    presentationNames += levelName + ", ";
                }
                return presentationNames.TrimEnd(',', ' ');
            }
        }

        public RelayCommand ChangeLevelsCommand { get; private set; }
        void OnChangeLevels()
        {
            var levelsSelectationViewModel = new LevelsSelectationViewModel(GuardUser);
            if (ServiceFactory.UserDialogs.ShowModalWindow(levelsSelectationViewModel))
            {
                GuardUser.LevelNames = levelsSelectationViewModel.LevelNames;
                OnPropertyChanged("PresentationLevels");
            }
        }

        protected override void Save(ref bool cancel)
        {
            if (!DigitalPasswordHelper.Check(Password))
            {
                DialogBox.DialogBox.Show("Пароль может содержать только цифры");
                cancel = true;
                return;
            }

            SaveProperies();
        }
    }
}