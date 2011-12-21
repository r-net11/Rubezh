using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Common;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;

namespace FireMonitor
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
            {
                ChangeMaximizedState();
            }
        }

        void ChangeMaximizedState()
        {
            if (App.Current.MainWindow.WindowState == System.Windows.WindowState.Normal)
                App.Current.MainWindow.WindowState = System.Windows.WindowState.Maximized;
            else
                App.Current.MainWindow.WindowState = System.Windows.WindowState.Normal;
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
            var helperPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"Firesec\FS_CL_OPER.HLP");
            Process.Start(helperPath);
        }

        void OnShowAbout(object sender, RoutedEventArgs e)
        {
        }

        public IViewPart MainContent
        {
            get { return _mainRegionHost.Content as IViewPart; }
            set { _mainRegionHost.DataContext = _mainRegionHost.Content = value; }
        }

        public IViewPart AlarmGroups
        {
            get { return _alarmGroups.Content as IViewPart; }
            set { _alarmGroups.DataContext = _alarmGroups.Content = value; }
        }

        void OnWindow_Closing(object sender, CancelEventArgs e)
        {
            AlarmPlayerHelper.Dispose();
            ClientSettings.SaveSettings();

            if (FiresecManager.CurrentUser.Permissions.Any(x => x == PermissionType.Oper_LogoutWithoutPassword))
            {
                return;
            }

            if (FiresecManager.CurrentUser.Permissions.Any(x => x == PermissionType.Oper_Logout) == false)
            {
                DialogBox.DialogBox.Show("Нет прав для выхода из программы");
                e.Cancel = true;
                return;
            }

            if (ServiceFactory.SecurityService.Validate() == false)
            {
                e.Cancel = true;
                return;
            }
        }

        void OnlWindow_Closed(object sender, EventArgs e)
        {
            FiresecManager.Disconnect();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}