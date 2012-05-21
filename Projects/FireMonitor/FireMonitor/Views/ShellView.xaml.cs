using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Common;
using Infrastructure.Common.MessageBox;
using FireMonitor.ViewModels;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using System.Collections.Generic;

namespace FireMonitor.Views
{
    public partial class ShellView : Window, INotifyPropertyChanged
    {
        public ShellView()
        {
            InitializeComponent();
            DataContext = this;
            FiresecManager.UserChanged += new Action(OnUserChanged);
			_navigation.User = FiresecManager.CurrentUser;
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
            if (CanClose())
            {
                AlarmPlayerHelper.Dispose();
                ClientSettings.SaveSettings();

                FiresecManager.Disconnect();
                Close();
                Application.Current.Shutdown();
                Environment.Exit(1);
            }
        }

        bool CanClose()
        {
            if (FiresecManager.CurrentUser.Permissions.Any(x => x == PermissionType.Oper_LogoutWithoutPassword))
            {
                return true;
            }

            if (FiresecManager.CurrentUser.Permissions.Any(x => x == PermissionType.Oper_Logout) == false)
            {
                MessageBoxService.Show("Нет прав для выхода из программы");
                return false;
            }

            return ServiceFactory.SecurityService.Validate();
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

        void OnUserChanged()
        {
            OnPropertyChanged("UserName");
        }

        public string UserName
        {
            get { return FiresecManager.CurrentUser.Name; }
        }

        public IViewPart AlarmGroups
        {
            get { return _alarmGroups.Content as IViewPart; }
            set { _alarmGroups.DataContext = _alarmGroups.Content = value; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

		public List<NavigationItem> Navigation
		{
			get { return _navigation.Navigation; }
			set { _navigation.Navigation = value; }
		}
	}
}