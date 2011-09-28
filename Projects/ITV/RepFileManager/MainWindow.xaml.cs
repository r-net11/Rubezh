using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Xml.Serialization;
using FiresecClient;
using ItvIntergation.Ngi;
using System.Windows.Media;
using FiresecAPI.Models;
using System.Diagnostics;

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
            var result = FiresecManager.Connect("adm", "");
            if (result != null)
            {
                MessageBox.Show(result);
                return;
            }
            FiresecManager.SelectiveFetch();

            Directory.CreateDirectory("BMP");

            var repositoryModule = new repositoryModule();
            repositoryModule.name = "Rubezh devices";
            repositoryModule.version = "1.0.0";
            repositoryModule.port = "1234";
            var repository = new repository();
            repository.module = repositoryModule;

            var devices = new List<repositoryModuleDevice>();

            var commonPanelDevice = new CommonPanelDevice();
            devices.Add(commonPanelDevice.Device);

            var commonCommunicationDevice = new CommonCommunicationDevice();
            devices.Add(commonCommunicationDevice.Device);

            //return;

            foreach (var driver in FiresecManager.Drivers)
            {
                switch (driver.DriverType)
                {
                    case DriverType.MS_1:
                    case DriverType.MS_2:
                        continue;

                    case DriverType.BUNS:
                    case DriverType.Rubezh_10AM:
                    case DriverType.Rubezh_2AM:
                    case DriverType.Rubezh_2OP:
                    case DriverType.Rubezh_4A:
                        continue;

                    // все устройства, подключаемые через USB игнорить совсем
                    case DriverType.USB_BUNS:
                    case DriverType.USB_Rubezh_2AM:
                    case DriverType.USB_Rubezh_4A:
                    case DriverType.USB_Rubezh_2OP:
                        continue;
                }

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
