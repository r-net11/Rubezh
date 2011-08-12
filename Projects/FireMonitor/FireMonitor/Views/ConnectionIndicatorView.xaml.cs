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

namespace FireMonitor
{
    public partial class ConnectionIndicatorView : UserControl
    {
        public ConnectionIndicatorView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            SafeFiresecService.ConnectionLost += new Action(OnConnectionLost);
            SafeFiresecService.ConnectionAppeared += new Action(OnConnectionAppeared);
        }

        void SetConnectionState(string state)
        {
            _textBlock.Text = state;
        }

        void OnConnectionLost()
        {
            Dispatcher.Invoke(new Action<string>(SetConnectionState), "Потеря связи");
        }

        void OnConnectionAppeared()
        {
            Dispatcher.Invoke(new Action<string>(SetConnectionState), "0");
        }
    }
}
