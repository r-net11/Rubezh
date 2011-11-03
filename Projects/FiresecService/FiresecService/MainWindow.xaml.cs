using System;
using System.Windows;
using FiresecService;
using FiresecService.Imitator;
using System.Diagnostics;

namespace FiresecServiceRunner
{
    public partial class MainWindow : Window
    {
        public static MainWindow Current { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            Current = this;
            AnalizeCommandLine();
        }

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

        void AnalizeCommandLine()
        {
            var commandLineArgs = Environment.GetCommandLineArgs();
            bool start = false;
            bool convertConfiguration = false;
            for (int i = 0; i != commandLineArgs.Length; ++i)
            {
                switch (commandLineArgs[i])
                {
                    case "/Start":
                        start = true;
                        break;
                    case "/Convert":
                        convertConfiguration = true;
                        break;
                    default:
                        break;
                }
            }
            if (start)
            {
                FiresecManager.ConnectFiresecCOMServer("adm", "");
                FiresecServiceManager.Open();
            }
            if (convertConfiguration)
            {
                ConfigurationConverter.Convert();
            }
        }
    }
}