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

            var app = new System.Windows.Application();
            app.Resources.MergedDictionaries.Add(ResourceHelper.GetResourceDictionary());
            app.ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown;
            // Load the ressources
            //var uriStyles = new System.Uri("DataGridStyle.xaml", UriKind.Relative);
            //var resourcesStyles = System.Windows.Application.LoadComponent(uriStyles) as System.Windows.ResourceDictionary;
            // Merge it on application level
            
            //System.Windows.Application.GetResourceStream()
        }
        CurrentDeviceViewModel _currentDeviceViewModel;
        CurrentDeviceView _currentDeviceView;

        public Guid DeviceId { get; set; }

        private void Form1_Load(object sender, EventArgs e)
        {
            StartFiresecClient();
            InitializeCurrentDevice();
        }

        private void InitializeCurrentDevice()
        {
            _currentDeviceViewModel = new CurrentDeviceViewModel();
            _currentDeviceView = new CurrentDeviceView();
            _currentDeviceView.DataContext = _currentDeviceViewModel;

            elementHost.Child = _currentDeviceView;


            if (DeviceId != Guid.Empty)
            {
                _currentDeviceViewModel.Inicialize(DeviceId);
            }
        }

        private void StartFiresecClient()
        {
            FiresecManager.Connect("net.tcp://localhost:9000/FiresecCallbackService/", "net.tcp://localhost:8000/FiresecService/", "adm", "");
            FiresecManager.ActiveXFetch();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            _currentDeviceViewModel.SelectDevice();
            DeviceId = _currentDeviceViewModel.DeviceId;
            if (DeviceId != Guid.Empty)
            {
                _currentDeviceViewModel.Inicialize(DeviceId);
            }
            
        }
    }
}
