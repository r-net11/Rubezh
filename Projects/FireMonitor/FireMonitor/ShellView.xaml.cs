using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows.Controls;
using CustomWindow;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Microsoft.Windows.Controls;
using Infrastructure;

namespace FireMonitor
{
    public partial class ShellView : EssentialWindow, INotifyPropertyChanged
    {
        readonly Thread realTimeDaemon;

        public ShellView()
        {
            InitializeComponent();
            DataContext = this;
            realTimeDaemon = new Thread(UpdateTime);
            realTimeDaemon.Start();
        }

        string _realTime;
        public string RealTime
        {
            get { return _realTime; }
            private set
            {
                _realTime = value;
                OnPropertyChanged("RealTime");
            }
        }

        void UpdateTime()
        {
            while (true)
            {
                Dispatcher.BeginInvoke((Action<string>) (x => RealTime = x),
                                        System.Windows.Threading.DispatcherPriority.Send,
                                        DateTime.Now.ToString());
                Thread.Sleep(500);
            }
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
            realTimeDaemon.Abort();
            realTimeDaemon.Join();
            FiresecManager.Disconnect();
        }
    }
}
