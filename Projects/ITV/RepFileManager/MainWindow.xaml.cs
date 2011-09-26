using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Xml.Serialization;
using FiresecClient;
using ItvIntergation.Ngi;
using System.Windows.Media;

namespace RepFileManager
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            bool result = FiresecManager.Connect("adm", "");
            if (result == false)
            {
                MessageBox.Show("Не удается соединиться с сервером");
                return;
            }

            Directory.CreateDirectory("BMP");

            var repositoryModule = new repositoryModule();
            repositoryModule.name = "Rubezh devices";
            repositoryModule.version = "1.0.0";
            repositoryModule.port = "1234";
            var repository = new repository();
            repository.module = repositoryModule;

            var devices = new List<repositoryModuleDevice>();
            foreach (var driver in FiresecManager.Drivers)
            {
                var repDevice = new RepDevice();
                repDevice.Initialize(driver);
                devices.Add(repDevice.Device);
            }

            repositoryModule.device = devices.ToArray();

            var serializer = new XmlSerializer(typeof(repositoryModule));
            using (var fileStream = File.CreateText("Rubezh.rep"))
            {
                serializer.Serialize(fileStream, repositoryModule);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
        }
    }
}
