using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Common;
using Controls.MessageBox;
using FireAdministrator.ViewModels;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;

namespace FireAdministrator.Views
{
    public partial class ShellView : Window, INotifyPropertyChanged
    {
        public ShellView()
        {
            InitializeComponent();
            DataContext = this;
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

        void Header_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
                ChangeMaximizedState();
        }

        void ChangeMaximizedState()
        {
            if (Application.Current.MainWindow.WindowState == System.Windows.WindowState.Normal)
                Application.Current.MainWindow.WindowState = System.Windows.WindowState.Maximized;
            else
                Application.Current.MainWindow.WindowState = System.Windows.WindowState.Normal;
        }

        void OnClose(object sender, RoutedEventArgs e)
        {
            Close();
        }

        void OnMinimize(object sender, RoutedEventArgs e)
        {
            WindowState = System.Windows.WindowState.Minimized;
        }

        void OnMaximize(object sender, RoutedEventArgs e)
        {
            ChangeMaximizedState();
        }

        void OnShowHelp(object sender, RoutedEventArgs e)
        {
            Process.Start("Manual.pdf");
        }

        void OnShowAbout(object sender, RoutedEventArgs e)
        {
            var aboutViewModel = new AboutViewModel();
            ServiceFactory.UserDialogs.ShowModalWindow(aboutViewModel);
        }

        public IViewPart MainContent
        {
            get { return _mainRegionHost.Content as IViewPart; }
            set { _mainRegionHost.DataContext = _mainRegionHost.Content = value; }
        }

        public object Menu
        {
            get { return _menu.Content; }
            set { _menu.DataContext = _menu.Content = value; }
        }

        public object ValidatoinArea
        {
            get { return _validationArea.Content; }
            set { _validationArea.DataContext = _validationArea.Content = value; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        void Window_Closing(object sender, CancelEventArgs e)
        {
            AlarmPlayerHelper.Dispose();

            if (ServiceFactory.SaveService.HasChanges)
            {
                var result = MessageBoxService.ShowQuestion("Сохранить изменения в настройках?");
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        this._menuView.SetNewConfig();
                        return;

                    case MessageBoxResult.No:
                        return;

                    case MessageBoxResult.Cancel:
                        e.Cancel = true;
                        return;
                }
            }
        }

        void Window_Closed(object sender, System.EventArgs e)
        {
            FiresecManager.Disconnect();
            Application.Current.Shutdown();
        }
    }
}