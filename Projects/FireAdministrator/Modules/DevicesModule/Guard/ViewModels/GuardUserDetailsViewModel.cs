using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class GuardUserDetailsViewModel : SaveCancelDialogContent
    {
        public GuardUserDetailsViewModel()
        {
            ChangeGuardLevelsCommand = new RelayCommand(OnChangeGuardLevels);
        }

        public GuardUser GuardUser { get; private set; }
        bool _isNew;

        public void Initialize()
        {
            Title = "Создать пользователя";
            _isNew = true;
            GuardUser = new GuardUser();
        }

        public void Initialize(GuardUser guardUser)
        {
            Title = "Редактировать пользователя";
            _isNew = false;
            GuardUser = guardUser;
            Name = guardUser.Name;
            Password = guardUser.Password;
            FIO = guardUser.FIO;
            Function = guardUser.Function;
            CanSetZone = guardUser.CanSetZone;
            CanUnSetZone = guardUser.CanUnSetZone;
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
                var presentationNames = new StringBuilder();
                if (GuardUser.GuardLevelNames.Count > 0)
                {
                    presentationNames.Append(GuardUser.GuardLevelNames[0]);
                }
                if (GuardUser.GuardLevelNames.Count > 1)
                {
                    for (int i = 1; i < GuardUser.GuardLevelNames.Count; ++i)
                    {
                        presentationNames.Append(", ");
                        presentationNames.Append(GuardUser.GuardLevelNames[i]);
                    }
                }
                return presentationNames.ToString().TrimStart(',');
            }
        }

        public RelayCommand ChangeGuardLevelsCommand { get; private set; }
        void OnChangeGuardLevels()
        {
            var guardLevelsSelectationViewModel = new GuardLevelsSelectationViewModel();
            guardLevelsSelectationViewModel.Initialize(GuardUser);
            var result = ServiceFactory.UserDialogs.ShowModalWindow(guardLevelsSelectationViewModel);
            if (result)
            {
                GuardUser.GuardLevelNames = guardLevelsSelectationViewModel.GuardLevels;
                OnPropertyChanged("PresentationLevels");
            }
        }

        protected override void Save()
        {
            GuardUser.Name = Name;
            GuardUser.Password = Password;
            GuardUser.FIO = FIO;
            GuardUser.Function = Function;
            GuardUser.CanSetZone = CanSetZone;
            GuardUser.CanUnSetZone = CanUnSetZone;
        }
    }
}