using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;

namespace FireMonitor
{
    public partial class AutoActivationView : UserControl, INotifyPropertyChanged
    {
        public AutoActivationView()
        {
            InitializeComponent();
            DataContext = this;

            ChangeAutoActivationCommand = new RelayCommand(OnChangeAutoActivation);
            ChangePlansAutoActivationCommand = new RelayCommand(OnChangePlansAutoActivation);
            IsAutoActivation = true;
            IsPlansAutoActivation = true;
            FiresecEventSubscriber.NewJournalRecordEvent += new Action<JournalRecord>(OnNewJournalRecord);
        }

        bool _isAutoActivation;
        public bool IsAutoActivation
        {
            get { return _isAutoActivation; }
            set
            {
                _isAutoActivation = value;
                OnPropertyChanged("IsAutoActivation");
            }
        }

        bool _isPlansAutoActivation;
        public bool IsPlansAutoActivation
        {
            get { return _isPlansAutoActivation; }
            set
            {
                _isPlansAutoActivation = value;
                OnPropertyChanged("IsPlansAutoActivation");
            }
        }

        public RelayCommand ChangeAutoActivationCommand { get; private set; }
        void OnChangeAutoActivation()
        {
            IsAutoActivation = !IsAutoActivation;

            //if (IsAutoActivation)
            //{
            //    FiresecEventSubscriber.NewJournalRecordEvent += new Action<JournalRecord>(OnNewEvent);
            //    IsAutoActivation = false;
            //}
            //else
            //{
            //    FiresecEventSubscriber.NewJournalRecordEvent -= new Action<JournalRecord>(OnNewEvent);
            //    IsAutoActivation = true;
            //}
        }

        public RelayCommand ChangePlansAutoActivationCommand { get; private set; }
        void OnChangePlansAutoActivation()
        {
            IsPlansAutoActivation = !IsPlansAutoActivation;

            //if (IsAutoActivation)
            //{
            //    FiresecEventSubscriber.NewJournalRecordEvent += new Action<JournalRecord>(OnNewEvent);
            //    IsAutoActivation = false;
            //}
            //else
            //{
            //    FiresecEventSubscriber.NewJournalRecordEvent -= new Action<JournalRecord>(OnNewEvent);
            //    IsAutoActivation = true;
            //}
        }

        void OnNewJournalRecord(JournalRecord journalRecord)
        {
            if (IsAutoActivation)
            {
                if (!App.Current.MainWindow.IsActive)
                {
                    App.Current.MainWindow.WindowState = WindowState.Maximized;
                    App.Current.MainWindow.Activate();
                }
            }
            if (IsPlansAutoActivation)
            {
                ServiceFactory.Events.GetEvent<ShowPlansEvent>().Publish(null);
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