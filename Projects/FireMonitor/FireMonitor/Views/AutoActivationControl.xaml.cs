using System;
using System.Windows;
using System.Windows.Controls;
using FiresecAPI.Models;
using FiresecClient;

namespace FireMonitor
{
    public partial class AutoActivationControl : UserControl
    {
        public AutoActivationControl()
        {
            InitializeComponent();
        }

        void autoActivationButton_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            FiresecEventSubscriber.NewJournalRecordEvent += new Action<JournalRecord>(OnNewEvent<JournalRecord>);
            FiresecEventSubscriber.DeviceStateChangedEvent += new Action<Guid>(OnNewEvent<Guid>);
        }

        void autoActivationButton_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            FiresecEventSubscriber.NewJournalRecordEvent -= new Action<JournalRecord>(OnNewEvent<JournalRecord>);
            FiresecEventSubscriber.DeviceStateChangedEvent -= new Action<Guid>(OnNewEvent<Guid>);
        }

        void OnNewEvent<T>(T obj)
        {
            if (!App.Current.MainWindow.IsActive)
            {
                App.Current.MainWindow.WindowState = WindowState.Maximized;
                App.Current.MainWindow.Activate();
            }
        }
    }
}