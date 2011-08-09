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

namespace CurrentDeviceModule.Views
{
    /// <summary>
    /// Логика взаимодействия для DeviceControlView.xaml
    /// </summary>
    public partial class CurrentDeviceView : UserControl
    {
        public CurrentDeviceView()
        {
            InitializeComponent();
            ResourceDictionary rd = new ResourceDictionary() { Source = new System.Uri("pack://application:,,,/Infrastructure.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;component/Themes/DataGridStyle.xaml") };
            Resources.MergedDictionaries.Add(rd);
        }
    }
}
