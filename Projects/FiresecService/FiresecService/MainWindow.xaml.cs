using System;
using System.IO;
using System.Windows;
using FiresecService;
using FiresecService.ViewModels;
using Infrastructure.Common;

namespace FiresecServiceRunner
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var resourceService = new ResourceService();
            resourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));
            _mainView.DataContext = new MainViewModel();

            DirectoryInfo directoryInfo = new DirectoryInfo(Environment.GetCommandLineArgs()[0]);
            Environment.CurrentDirectory = directoryInfo.FullName.Replace(directoryInfo.Name, "");
            AnalizeCommandLine();

            this.WindowState = WindowState.Minimized;
        }

        void AnalizeCommandLine()
        {
            var commandLineArgs = Environment.GetCommandLineArgs();
            for (int i = 0; i != commandLineArgs.Length; ++i)
            {
                switch (commandLineArgs[i])
                {
                    case "/Convert":
                        ConfigurationConverter.Convert();
                        break;
                    case "/ConvertJournal":
                        JournalDataConverter.Convert();
                        break;
                }
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
            if (MessageBox.Show("Вы уверены, что хотите остановить сервер?", "Вы уверены, что хотите остановить сервер?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                Close();
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
    }
}