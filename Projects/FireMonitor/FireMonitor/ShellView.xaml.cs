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
using System.Windows.Shapes;
using Infrastructure;
using Infrastructure.Common;
using AlarmModule.Events;
using Infrastructure.Events;
using System.Diagnostics;
using System.ComponentModel;
using CustomWindow;
using FiresecClient;
using FiresecAPI.Models;

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

        private void Header_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
                this.DragMove();
        }

        private void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
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

        private void EssentialWindow_Closing(object sender, CancelEventArgs e)
        {
            if (FiresecManager.CurrentPermissions.Any(x => x.PermissionType == PermissionType.Oper_Logout) == false)
            {
                MessageBox.Show("Нет прав для выхода из программы");
                e.Cancel = true;
                return;
            }

            if (FiresecManager.CurrentPermissions.Any(x => x.PermissionType == PermissionType.Oper_LogoutNoPassword))
            {
                return;
            }

            bool result = ServiceFactory.Get<ISecurityService>().Check();
            if (result == false)
            {
                e.Cancel = true;
                return;
            }
        }

        private void EssentialWindow_Closed(object sender, EventArgs e)
        {
            FiresecManager.Disconnect();
        }
    }
}
