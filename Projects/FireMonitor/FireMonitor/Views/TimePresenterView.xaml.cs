using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Controls;

namespace FireMonitor
{
    public partial class TimePresenterView : UserControl, INotifyPropertyChanged
    {
        readonly Timer _realTimeDaemon;

        public TimePresenterView()
        {
            InitializeComponent();
            DataContext = this;
            _realTimeDaemon = new Timer(new TimerCallback(UpdateTime), null, 0, 1000);
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
        void UpdateTime(object obj)
        {
            Dispatcher.BeginInvoke((Action<string>) (x => RealTime = x),
                                    System.Windows.Threading.DispatcherPriority.Send,
                                    DateTime.Now.ToString());
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}