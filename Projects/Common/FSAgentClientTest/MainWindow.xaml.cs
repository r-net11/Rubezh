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
using System.Collections.ObjectModel;
using FSAgentClient;
using FiresecAPI.Models;
using System.Diagnostics;

namespace FSAgentClientTest
{
    public partial class MainWindow : Window
    {
        public static MainWindow Current;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            JournalItems = new ObservableCollection<string>();
            Current = this;
        }

        public ObservableCollection<string> JournalItems { get; private set; }
        FSAgent FSAgent;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FSAgent = new FSAgent("net.pipe://127.0.0.1/FSAgent/");
            FSAgent.Start();
            FSAgent.NewJournalRecords += new Action<List<JournalRecord>>(OnNewJournalRecords);
        }

        static void OnNewJournalRecords(List<JournalRecord> journalRecords)
        {
            try
            {
                foreach (var journalRecord in journalRecords)
                {
                    MainWindow.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        MainWindow.Current.JournalItems.Insert(0, journalRecord.SystemTime.ToString() + " - " + journalRecord.Description);
                        ;
                    }));
                    Trace.WriteLine(journalRecord.Description);
                }
            }
            catch (Exception e)
            {
            }
        }
    }
}