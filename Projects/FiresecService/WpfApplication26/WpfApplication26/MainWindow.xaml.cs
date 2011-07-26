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
using System.ServiceModel;
using FiresecClient;
using System.IO;

namespace WpfApplication26
{
    public partial class MainWindow : Window,  IFiresecCallback
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        DuplexChannelFactory<IFiresecService> _duplexChannelFactory;
        public static IFiresecService FiresecService;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NetTcpBinding binding = new NetTcpBinding();
            binding.MaxBufferSize = Int32.MaxValue;
            binding.MaxReceivedMessageSize = Int32.MaxValue;
            binding.MaxBufferPoolSize = Int32.MaxValue;
            binding.ReaderQuotas.MaxStringContentLength = Int32.MaxValue;
            EndpointAddress endpointAddress = new EndpointAddress("net.tcp://localhost:8000/FiresecService");
            _duplexChannelFactory = new DuplexChannelFactory<IFiresecService>(new InstanceContext(this), binding, endpointAddress);
            FiresecService = _duplexChannelFactory.CreateChannel();

            var configuration = FiresecService.GetCoreConfig();
            configuration.Update();

            var states = FiresecService.GetCurrentStates();


            Stream stream = FiresecService.GetFile();
            FileStream fileStream = new FileStream("D:/yyy.txt", FileMode.Create);
            stream.CopyTo(fileStream);
            fileStream.Close();
        }

        public void NewEvent(string name)
        {
        }
    }
}
