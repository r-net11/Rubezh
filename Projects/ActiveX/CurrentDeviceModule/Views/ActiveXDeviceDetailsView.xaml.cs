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
using System.IO;

namespace CurrentDeviceModule.Views
{
    /// <summary>
    /// Логика взаимодействия для ActiveXDeviceDetailsView.xaml
    /// </summary>
    public partial class ActiveXDeviceDetailsView : UserControl
    {
        public ActiveXDeviceDetailsView()
        {
            InitializeComponent();
            //try
            //{
            //    InitializeComponent();
            //}
            //catch (Exception ex)
            //{
            //    var Source = new System.Uri("pack://application:,,,/Controls,Version=1.0.0.0,Culture=neutral,PublicKeyToken=null;component/Themes/DataGridStyle.xaml");
            //    ListView listView = new ListView();
            //    listView.Items.Add(ex.Message);
            //    listView.Items.Add(ex.StackTrace);
            //    listView.Items.Add(ex.TargetSite);
            //    listView.Items.Add(ex.InnerException);
            //    listView.Items.Add(Source.AbsoluteUri);
            //    listView.Items.Add(Source.AbsolutePath);
            //    listView.Items.Add(Source.LocalPath);
            //    listView.Items.Add(Source.IsFile);
            //    //listView.Items.Add(ex.FusionLog);
            //    //listView.Items.Add(ex.Data);
            //    Window window = new Window();
            //    window.Content = listView;
            //    window.ShowDialog();
            //}
        }
    }
}
