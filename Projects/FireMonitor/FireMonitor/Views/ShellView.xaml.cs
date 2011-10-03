using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using Common;
using CustomWindow;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using System.Windows.Input;

namespace FireMonitor
{
    public partial class ShellView : EssentialWindow, INotifyPropertyChanged
    {
        public ShellView()
        {
            InitializeComponent();
            DataContext = this;
        }

        protected override Decorator GetWindowButtonsPlaceholder()
        {
            return WindowButtonsPlaceholder;
        }

        void Header_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
                this.DragMove();
        }

        void Header_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                if (App.Current.MainWindow.WindowState == System.Windows.WindowState.Normal)
                    App.Current.MainWindow.WindowState = System.Windows.WindowState.Maximized;
                else
                    App.Current.MainWindow.WindowState = System.Windows.WindowState.Normal;
            }
        }

        void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (this.Width + e.HorizontalChange > 10)
                this.Width += e.HorizontalChange;
            if (this.Height + e.VerticalChange > 10)
                this.Height += e.VerticalChange;
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

        public IViewPart Alarm
        {
            get { return _alarm.Content as IViewPart; }
            set { _alarm.DataContext = _alarm.Content = value; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        void EssentialWindow_Closing(object sender, CancelEventArgs e)
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

            if (ServiceFactory.Get<ISecurityService>().Validate() == false)
            {
                e.Cancel = true;
                return;
            }
        }

        void EssentialWindow_Closed(object sender, EventArgs e)
        {
            FiresecManager.Disconnect();
        }
    }
}