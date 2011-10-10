using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;

namespace FireMonitor
{
    public partial class AutoActivationControl : UserControl, INotifyPropertyChanged
    {
        public AutoActivationControl()
        {
            InitializeComponent();
            DataContext = this;
            ChangeAutoactivationCommand = new RelayCommand(OnChangeAutoactivation);
            IsAutoactivation = true;
        }

        bool _isAutoactivation;
        public bool IsAutoactivation
        {
            get { return _isAutoactivation; }
            set
            {
                _isAutoactivation = value;
                OnPropertyChanged("IsAutoactivation");
            }
        }

        public RelayCommand ChangeAutoactivationCommand { get; private set; }
        void OnChangeAutoactivation()
        {
            if (IsAutoactivation)
            {
                FiresecEventSubscriber.NewJournalRecordEvent += new Action<JournalRecord>(OnNewEvent<JournalRecord>);
                IsAutoactivation = false;
            }
            else
            {
                FiresecEventSubscriber.NewJournalRecordEvent -= new Action<JournalRecord>(OnNewEvent<JournalRecord>);
                IsAutoactivation = true;
            }
        }

        void OnNewEvent<T>(T obj)
        {
            if (!App.Current.MainWindow.IsActive)
            {
                App.Current.MainWindow.WindowState = WindowState.Maximized;
                App.Current.MainWindow.Activate();
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