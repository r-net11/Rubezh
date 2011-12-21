using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using FiresecService;
using FiresecService.Infrastructure;
using FiresecService.ViewModels;
using Infrastructure.Common;

namespace FiresecServiceRunner
{
    public partial class MainWindow : Window
    {
        public static MainWindow Current { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            Current = this;
            DirectoryInfo dirInfo = new DirectoryInfo(Environment.GetCommandLineArgs()[0]);
            Environment.CurrentDirectory = dirInfo.FullName.Replace(dirInfo.Name, "");
            AnalizeCommandLine();

            var resourceService = new ResourceService();
            resourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));
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
            UserDialogService.ShowModalWindow(new ImitatorViewModel());
        }

        void AnalizeCommandLine()
        {
            var commandLineArgs = Environment.GetCommandLineArgs();
            bool start = false;
            bool convertConfiguration = false;
            bool exit = false;
            bool convertJournal = false;
            bool hide = false;
            bool service = false;
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
                    case "/ConvertJournal":
                        convertJournal = true;
                        break;
                    case "/Exit":
                        exit = true;
                        break;
                    case "/Hide":
                        hide = true;
                        break;
                    case "/service":
                        service = true;
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
            if (start && convertConfiguration)
                ConfigurationConverter.Convert();
            if (convertJournal)
                JournalDataConverter.Convert();
            if (hide)
                this.Hide();
            if (exit)
                this.Close();
        }

        private void OnShow(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Normal;
        }

        private void OnHide(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        void OnClose(object sender, RoutedEventArgs e)
        {
            Close();
        }

        void OnMinimize(object sender, RoutedEventArgs e)
        {
            WindowState = System.Windows.WindowState.Minimized;
        }

        void Header_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
                this.DragMove();
        }

        void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (this.Width + e.HorizontalChange > 10)
                this.Width += e.HorizontalChange;
            if (this.Height + e.VerticalChange > 10)
                this.Height += e.VerticalChange;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            _notificationIcon.Dispose();
            base.OnClosing(e);
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.ShowInTaskbar = false;
            }
            if (this.WindowState == WindowState.Normal)
            {
                this.ShowInTaskbar = true;
            }
        }

        void OnWindow_Closing(object sender, CancelEventArgs e)
        {
        }

        void OnWindow_Closed(object sender, EventArgs e)
        {
        }
    }
}