using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
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

            ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Subscribe(x => { isDevicesSelected = true; OnPropertyChanged("IsDevicesSelected"); });
            ServiceFactory.Events.GetEvent<ShowZoneEvent>().Subscribe(x => { isZonesSelected = true; OnPropertyChanged("IsZonesSelected"); });
            ServiceFactory.Events.GetEvent<ShowDirectionsEvent>().Subscribe(x => { isDerectonsSelected = true; OnPropertyChanged("IsDerectonsSelected"); });
            ServiceFactory.Events.GetEvent<ShowLibraryEvent>().Subscribe(x => { isLibrarySelected = true; OnPropertyChanged("IsLibrarySelected"); });
            ServiceFactory.Events.GetEvent<ShowPlansEvent>().Subscribe(x => { isPlanSelected = true; OnPropertyChanged("IsPlanSelected"); });
            ServiceFactory.Events.GetEvent<ShowSecurityEvent>().Subscribe(x => { isSecuritySelected = true; OnPropertyChanged("IsSecuritySelected"); });
            ServiceFactory.Events.GetEvent<ShowJournalEvent>().Subscribe(x => { isJournalSelected = true; OnPropertyChanged("IsJournalSelected"); });
            ServiceFactory.Events.GetEvent<ShowSoundsEvent>().Subscribe(x => { isSoundsSelected = true; OnPropertyChanged("IsSoundsSelected"); });
        }

        bool isDevicesSelected;
        public bool IsDevicesSelected
        {
            get { return isDevicesSelected; }
            set
            {
                isDevicesSelected = value;
                if (value)
                {
                    ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Publish(null);
                }
                OnPropertyChanged("IsDevicesSelected");
            }
        }

        bool isZonesSelected;
        public bool IsZonesSelected
        {
            get { return isZonesSelected; }
            set
            {
                isZonesSelected = value;
                if (value)
                {
                    ServiceFactory.Events.GetEvent<ShowZoneEvent>().Publish(null);
                }
                OnPropertyChanged("IsZonesSelected");
            }
        }

        bool isDerectonsSelected;
        public bool IsDerectonsSelected
        {
            get { return isDerectonsSelected; }
            set
            {
                isDerectonsSelected = value;
                if (value)
                {
                    ServiceFactory.Events.GetEvent<ShowDirectionsEvent>().Publish(null);
                }
                OnPropertyChanged("IsDerectonsSelected");
            }
        }

        bool isLibrarySelected;
        public bool IsLibrarySelected
        {
            get { return isLibrarySelected; }
            set
            {
                isLibrarySelected = value;
                if (value)
                {
                    ServiceFactory.Events.GetEvent<ShowLibraryEvent>().Publish(null);
                }
                OnPropertyChanged("IsLibrarySelected");
            }
        }

        bool isPlanSelected;
        public bool IsPlanSelected
        {
            get { return isPlanSelected; }
            set
            {
                isPlanSelected = value;
                if (value)
                {
                    ServiceFactory.Events.GetEvent<ShowPlansEvent>().Publish(null);
                }
                OnPropertyChanged("IsPlanSelected");
            }
        }

        bool isSecuritySelected;
        public bool IsSecuritySelected
        {
            get { return isSecuritySelected; }
            set
            {
                isSecuritySelected = value;
                if (value)
                {
                    ServiceFactory.Events.GetEvent<ShowSecurityEvent>().Publish(null);
                }
                OnPropertyChanged("IsSecuritySelected");
            }
        }

        bool isJournalSelected;
        public bool IsJournalSelected
        {
            get { return isJournalSelected; }
            set
            {
                isJournalSelected = value;
                if (value)
                {
                    ServiceFactory.Events.GetEvent<ShowJournalEvent>().Publish(null);
                }
                OnPropertyChanged("IsJournalSelected");
            }
        }

        bool isSoundsSelected;
        public bool IsSoundsSelected
        {
            get { return isSoundsSelected; }
            set
            {
                isSoundsSelected = value;
                if (value)
                {
                    ServiceFactory.Events.GetEvent<ShowSoundsEvent>().Publish(null);
                }
                OnPropertyChanged("IsSoundsSelected");
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
