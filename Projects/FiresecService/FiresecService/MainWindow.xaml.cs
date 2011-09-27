using System;
using System.Windows;
using FiresecService;
using FiresecService.Imitator;

namespace FiresecServiceRunner
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Current = this;
        }

        public static MainWindow Current { get; private set; }
        public static void AddMessage(string message)
        {
            Current._textBox.AppendText(message);
            Current._textBox.AppendText(Environment.NewLine);
        }

        void OnConnect(object sender, RoutedEventArgs e)
        {
            FiresecManager.ConnectFiresecCOMServer("adm", "");
            FiresecServiceManager.Open();
        }

        void OnConvertConfiguration(object sender, RoutedEventArgs e)
        {
            ConfigurationConverter.Convert();
        }

        void OnConvertJournal(object sender, RoutedEventArgs e)
        {
            JournalDataConverter.Convert();
        }

        void OnShowImitator(object sender, RoutedEventArgs e)
        {
            var imitatorView = new ImitatorView();
            imitatorView.Show();
        }
    }
}