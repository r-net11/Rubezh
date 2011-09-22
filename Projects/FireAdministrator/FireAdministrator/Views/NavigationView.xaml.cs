using System;
using System.ComponentModel;
using System.Windows.Controls;
using Infrastructure;
using Infrastructure.Events;

namespace FireAdministrator
{
    public partial class NavigationView : UserControl, INotifyPropertyChanged
    {
        public NavigationView()
        {
            InitializeComponent();
            DataContext = this;

            ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Subscribe(x => { _isDevicesSelected = true; OnPropertyChanged("IsDevicesSelected"); });
            ServiceFactory.Events.GetEvent<ShowZoneEvent>().Subscribe(x => { _isZonesSelected = true; OnPropertyChanged("IsZonesSelected"); });
            ServiceFactory.Events.GetEvent<ShowDirectionsEvent>().Subscribe(x => { _isDerectonsSelected = true; OnPropertyChanged("IsDerectonsSelected"); });

            ServiceFactory.Events.GetEvent<ShowGuardUsersEvent>().Subscribe(x => { _isGuardUsersSelected = true; OnPropertyChanged("IsGuardUsersSelected"); });
            ServiceFactory.Events.GetEvent<ShowGuardLevelsEvent>().Subscribe(x => { _isGuardLevelsSelected = true; OnPropertyChanged("IsGuardLevelsSelected"); });
            ServiceFactory.Events.GetEvent<ShowGuardDevicesEvent>().Subscribe(x => { _isGuardDevicesSelected = true; OnPropertyChanged("IsGuardDevicesSelected"); });

            ServiceFactory.Events.GetEvent<ShowLibraryEvent>().Subscribe(x => { _isLibrarySelected = true; OnPropertyChanged("IsLibrarySelected"); });
            ServiceFactory.Events.GetEvent<ShowPlansEvent>().Subscribe(x => { _isPlanSelected = true; OnPropertyChanged("IsPlanSelected"); });

            ServiceFactory.Events.GetEvent<ShowUsersEvent>().Subscribe(x => { _isUsersSelected = true; OnPropertyChanged("IsUsersSelected"); });
            ServiceFactory.Events.GetEvent<ShowUserGroupsEvent>().Subscribe(x => { _isUserGroupsSelected = true; OnPropertyChanged("IsUserGroupsSelected"); });

            ServiceFactory.Events.GetEvent<ShowJournalEvent>().Subscribe(x => { _isJournalSelected = true; OnPropertyChanged("IsJournalSelected"); });
            ServiceFactory.Events.GetEvent<ShowSoundsEvent>().Subscribe(x => { _isSoundsSelected = true; OnPropertyChanged("IsSoundsSelected"); });
            ServiceFactory.Events.GetEvent<ShowInstructionsEvent>().Subscribe(x => { _isInstructionsSelected = true; OnPropertyChanged("IsInstructionsSelected"); });
            ServiceFactory.Events.GetEvent<ShowSettingsEvent>().Subscribe(x => { _isSettingsSelected = true; OnPropertyChanged("IsSettingsSelected"); });
        }

        bool _isDevicesSelected;
        public bool IsDevicesSelected
        {
            get { return _isDevicesSelected; }
            set
            {
                _isDevicesSelected = value;
                if (value)
                {
                    ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Publish(Guid.Empty);
                }
                OnPropertyChanged("IsDevicesSelected");
            }
        }

        bool _isZonesSelected;
        public bool IsZonesSelected
        {
            get { return _isZonesSelected; }
            set
            {
                _isZonesSelected = value;
                if (value)
                {
                    ServiceFactory.Events.GetEvent<ShowZoneEvent>().Publish(null);
                }
                OnPropertyChanged("IsZonesSelected");
            }
        }

        bool _isDerectonsSelected;
        public bool IsDerectonsSelected
        {
            get { return _isDerectonsSelected; }
            set
            {
                _isDerectonsSelected = value;
                if (value)
                {
                    ServiceFactory.Events.GetEvent<ShowDirectionsEvent>().Publish(null);
                }
                OnPropertyChanged("IsDerectonsSelected");
            }
        }

        bool _isGuardSelected;
        public bool IsGuardSelected
        {
            get { return _isGuardSelected; }
            set
            {
                if (!value && (IsGuardUsersSelected || IsGuardLevelsSelected || IsGuardDevicesSelected))
                    return;

                _isGuardSelected = value;
                OnPropertyChanged("IsGuardSelected");
            }
        }

        bool _isGuardUsersSelected;
        public bool IsGuardUsersSelected
        {
            get { return _isGuardUsersSelected; }
            set
            {
                _isGuardUsersSelected = value;
                if (value)
                {
                    ServiceFactory.Events.GetEvent<ShowGuardUsersEvent>().Publish(null);
                }
                else
                {
                    IsGuardSelected = false;
                }
                OnPropertyChanged("IsGuardUsersSelected");
            }
        }

        bool _isGuardLevelsSelected;
        public bool IsGuardLevelsSelected
        {
            get { return _isGuardLevelsSelected; }
            set
            {
                _isGuardLevelsSelected = value;
                if (value)
                {
                    ServiceFactory.Events.GetEvent<ShowGuardLevelsEvent>().Publish(null);
                }
                else
                {
                    IsGuardSelected = false;
                }
                OnPropertyChanged("IsGuardLevelsSelected");
            }
        }

        bool _isGuardDevicesSelected;
        public bool IsGuardDevicesSelected
        {
            get { return _isGuardDevicesSelected; }
            set
            {
                _isGuardDevicesSelected = value;
                if (value)
                {
                    ServiceFactory.Events.GetEvent<ShowGuardDevicesEvent>().Publish(null);
                }
                else
                {
                    IsGuardSelected = false;
                }
                OnPropertyChanged("IsGuardLevelsSelected");
            }
        }

        bool _isLibrarySelected;
        public bool IsLibrarySelected
        {
            get { return _isLibrarySelected; }
            set
            {
                _isLibrarySelected = value;
                if (value)
                {
                    ServiceFactory.Events.GetEvent<ShowLibraryEvent>().Publish(null);
                }
                OnPropertyChanged("IsLibrarySelected");
            }
        }

        bool _isPlanSelected;
        public bool IsPlanSelected
        {
            get { return _isPlanSelected; }
            set
            {
                _isPlanSelected = value;
                if (value)
                {
                    ServiceFactory.Events.GetEvent<ShowPlansEvent>().Publish(null);
                }
                OnPropertyChanged("IsPlanSelected");
            }
        }

        bool _isSecuritySelected;
        public bool IsSecuritySelected
        {
            get { return _isSecuritySelected; }
            set
            {
                if (!value && (IsUsersSelected || IsUserGroupsSelected))
                    return;

                _isSecuritySelected = value;
                OnPropertyChanged("IsSecuritySelected");
            }
        }

        bool _isUsersSelected;
        public bool IsUsersSelected
        {
            get { return _isUsersSelected; }
            set
            {
                _isUsersSelected = value;
                if (value)
                {
                    ServiceFactory.Events.GetEvent<ShowUsersEvent>().Publish(null);
                }
                else
                {
                    IsSecuritySelected = false;
                }
                OnPropertyChanged("IsUsersSelected");
            }
        }

        bool _isUserGroupsSelected;
        public bool IsUserGroupsSelected
        {
            get { return _isUserGroupsSelected; }
            set
            {
                _isUserGroupsSelected = value;
                if (value)
                {
                    ServiceFactory.Events.GetEvent<ShowUserGroupsEvent>().Publish(null);
                }
                else
                {
                    IsSecuritySelected = false;
                }
                OnPropertyChanged("IsUserGroupsSelected");
            }
        }

        bool _isJournalSelected;
        public bool IsJournalSelected
        {
            get { return _isJournalSelected; }
            set
            {
                _isJournalSelected = value;
                if (value)
                {
                    ServiceFactory.Events.GetEvent<ShowJournalEvent>().Publish(null);
                }
                OnPropertyChanged("IsJournalSelected");
            }
        }

        bool _isSoundsSelected;
        public bool IsSoundsSelected
        {
            get { return _isSoundsSelected; }
            set
            {
                _isSoundsSelected = value;
                if (value)
                {
                    ServiceFactory.Events.GetEvent<ShowSoundsEvent>().Publish(null);
                }
                OnPropertyChanged("IsSoundsSelected");
            }
        }

        bool _isInstructionsSelected;
        public bool IsInstructionsSelected
        {
            get { return _isInstructionsSelected; }
            set
            {
                _isInstructionsSelected = value;
                if (value)
                {
                    ServiceFactory.Events.GetEvent<ShowInstructionsEvent>().Publish(null);
                }
                OnPropertyChanged("IsInstructionsSelected");
            }
        }

        bool _isSettingsSelected;
        public bool IsSettingsSelected
        {
            get { return _isSettingsSelected; }
            set
            {
                _isSettingsSelected = value;
                if (value)
                {
                    ServiceFactory.Events.GetEvent<ShowSettingsEvent>().Publish(null);
                }
                OnPropertyChanged("IsSettingsSelected");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}