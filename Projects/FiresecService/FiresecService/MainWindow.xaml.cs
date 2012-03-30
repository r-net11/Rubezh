using System;
using System.IO;
using System.Windows;
using FiresecService;
using FiresecService.ViewModels;
using Infrastructure.Common;
using Controls.MessageBox;
using System.Configuration;

namespace FiresecServiceRunner
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            try
            {
                InitializeAppSettings();

                var resourceService = new ResourceService();
                resourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));
                _mainView.DataContext = new MainViewModel();

                DirectoryInfo directoryInfo = new DirectoryInfo(Environment.GetCommandLineArgs()[0]);
                Environment.CurrentDirectory = directoryInfo.FullName.Replace(directoryInfo.Name, "");

                this.WindowState = WindowState.Minimized;
            }
            catch (Exception e)
            {
                MessageBoxService.ShowException(e);
                Application.Current.Shutdown();
            }
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
            if (MessageBoxService.ShowQuestion("Вы уверены, что хотите остановить сервер?") == MessageBoxResult.Yes)
            {
                Close();
                Application.Current.Shutdown();
            }
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

        static void InitializeAppSettings()
        {
            AppSettings.OldFiresecLogin = ConfigurationManager.AppSettings["OldFiresecLogin"] as string;
            AppSettings.OldFiresecPassword = ConfigurationManager.AppSettings["OldFiresecPassword"] as string;
            AppSettings.ServiceAddress = ConfigurationManager.AppSettings["ServiceAddress"] as string;
            AppSettings.IsDebug = Convert.ToBoolean(ConfigurationManager.AppSettings["IsDebug"] as string);
        }
    }
}