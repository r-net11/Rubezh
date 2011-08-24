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
using System.Windows.Navigation;
using System.Windows.Shapes;
using FiresecClient;
using System.ComponentModel;


namespace FireMonitor.Views
{
    /// <summary>
    /// Логика взаимодействия для InstructionView.xaml
    /// </summary>
    public partial class InstructionView : UserControl, INotifyPropertyChanged
    {
        public InstructionView()
        {
            InitializeComponent();
            FiresecEventSubscriber.DeviceStateChangedEvent += new Action<string>(OnDeviceStateChanged);
        }

        public void OnDeviceStateChanged(string deviceId)
        {
            
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
