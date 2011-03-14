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
using System.Threading;
using System.IO;
using System.Xml.Serialization;

namespace DeviveModelManager
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            StreamReader reader = new StreamReader("SetNewConfig.xml");
            string message = reader.ReadToEnd();
            reader.Close();

            byte[] bytes = Encoding.UTF8.GetBytes(message);
            MemoryStream memoryStream = new MemoryStream(bytes);
            XmlSerializer serializer = new XmlSerializer(typeof(ComServer.CoreConfig.config));
            ComServer.CoreConfig.config config = (ComServer.CoreConfig.config)serializer.Deserialize(memoryStream);
            memoryStream.Close();

            ComServer.ComServer.SetNewConfig(config);
            //ComServer.ComServer.DeviceWriteConfig(config, "0");

            textBox1.Text = message;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            textBox1.Text = ComServer.NativeComServer.GetCoreConfig();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            textBox1.Text = ComServer.NativeComServer.GetCoreState();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            textBox1.Text = ComServer.NativeComServer.GetMetaData();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            textBox1.Text = ComServer.NativeComServer.GetCoreDeviceParams();
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            textBox1.Text = ComServer.NativeComServer.ReadEvents(0, 100);
        }
    }
}
