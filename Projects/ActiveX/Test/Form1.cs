using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CurrentDeviceModule.Views;
using CurrentDeviceModule.ViewModels;
using FiresecClient;

namespace Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        CurrentDeviceViewModel _currentDeviceViewModel;
        CurrentDeviceView _currentDeviceView;

        public Guid DeviceId { get; set; }

        private void Form1_Load(object sender, EventArgs e)
        {
            StartFiresecClient();
            InitializeCurrentDevice();
        }
        System.Windows.Application app;
        private void InitializeCurrentDevice()
        {
            _currentDeviceViewModel = new CurrentDeviceViewModel();
            _currentDeviceView = new CurrentDeviceView();
            _currentDeviceView.DataContext = _currentDeviceViewModel;

            app = new System.Windows.Application();
            var resources = System.Windows.Application.LoadComponent(new Uri("DataGridStyle.xaml", UriKind.Relative)) as System.Windows.ResourceDictionary;

            app.Resources.MergedDictionaries.Add(resources);

            //Uri uri = new Uri("pack://application:,,,/Controls, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;component/Themes/DataGridStyle.xaml");
            //_currentDeviceView.Resources.Source = uri;
            //StreamResourceInfo sri = System.Windows.Application.GetResourceStream(uri);
            //ResourceDictionary resources = (ResourceDictionary)ResourceHelper.BamlReader(sri.Stream);
            //ResourceDictionary rd = new ResourceDictionary() { Source = new System.Uri("pack://application:,,,/Controls, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;component/Themes/DataGridStyle.xaml") };
            //_currentDeviceView.Resources.MergedDictionaries.Add(resources);
            elementHost.Child = _currentDeviceView;
            
            if (DeviceId != Guid.Empty)
            {
                _currentDeviceViewModel.Inicialize(DeviceId);
            }
        }

        private void StartFiresecClient()
        {
            FiresecManager.Connect("adm", "");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _currentDeviceViewModel.SelectDevice();
            DeviceId = _currentDeviceViewModel.DeviceId;
        }
    }
}
