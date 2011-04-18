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

namespace FiresecDirect
{
    public partial class FiresecDirectWindow : Window
    {
        public FiresecDirectWindow()
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
            XmlSerializer serializer = new XmlSerializer(typeof(Firesec.CoreConfig.config));
            Firesec.CoreConfig.config config = (Firesec.CoreConfig.config)serializer.Deserialize(memoryStream);
            memoryStream.Close();

            //Firesec.FiresecClient.SetNewConfig(config);
            //Firesec.FiresecClient.DeviceWriteConfig(config, "0");

            textBox1.Text = message;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            textBox1.Text = Firesec.NativeFiresecClient.GetCoreConfig();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            textBox1.Text = Firesec.NativeFiresecClient.GetCoreState();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            textBox1.Text = Firesec.NativeFiresecClient.GetMetaData();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            textBox1.Text = Firesec.NativeFiresecClient.GetCoreDeviceParams();
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            textBox1.Text = Firesec.NativeFiresecClient.ReadEvents(0, 100);
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            //Firesec.Metadata.config metadataConfig = Firesec.FiresecClient.GetMetaData();
            //XmlSerializer serializer = new XmlSerializer(typeof(Firesec.Metadata.config));
            //FileStream fileStream = new FileStream("..\\..\\metadata.xml", FileMode.Create);
            //serializer.Serialize(fileStream, metadataConfig);
        }
    }
}
