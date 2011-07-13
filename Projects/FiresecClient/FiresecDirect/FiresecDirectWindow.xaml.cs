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
            //Firesec.NativeFiresecClient.Connect("adm", "");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Firesec.NativeFiresecClient.Connect("adm", "");
        }

        private void OnSetNewConfig(object sender, RoutedEventArgs e)
        {
            StreamReader reader = new StreamReader("SetNewConfig.xml");
            string message = reader.ReadToEnd();
            reader.Close();

            //byte[] bytes = Encoding.UTF8.GetBytes(message);
            //MemoryStream memoryStream = new MemoryStream(bytes);
            //XmlSerializer serializer = new XmlSerializer(typeof(Firesec.CoreConfig.config));
            //Firesec.CoreConfig.config config = (Firesec.CoreConfig.config)serializer.Deserialize(memoryStream);
            //memoryStream.Close();

            //Firesec.FiresecClient.SetNewConfig(config);
            //Firesec.FiresecClient.DeviceWriteConfig(config, "0");

            textBox1.Text = message;
        }

        private void OnGetCoreConfig(object sender, RoutedEventArgs e)
        {
            string coreConfig = Firesec.NativeFiresecClient.GetCoreConfig();
            textBox1.Text = coreConfig;

            FileStream fileStream = new FileStream("D:/CoreConfig.xml", FileMode.Create);
            StreamWriter streamWriter = new StreamWriter(fileStream);
            streamWriter.Write(coreConfig);
            fileStream.Close();
        }

        private void OnGetCoreState(object sender, RoutedEventArgs e)
        {
            textBox1.Text = Firesec.NativeFiresecClient.GetCoreState();
        }

        private void OnGetMetaData(object sender, RoutedEventArgs e)
        {
            textBox1.Text = Firesec.NativeFiresecClient.GetMetaData();
        }

        private void OnGetCoreDeviceParams(object sender, RoutedEventArgs e)
        {
            textBox1.Text = Firesec.NativeFiresecClient.GetCoreDeviceParams();
        }

        private void OnReadEvents(object sender, RoutedEventArgs e)
        {
            textBox1.Text = Firesec.NativeFiresecClient.ReadEvents(0, 100);
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
        }

        private void OnBoltOpen(object sender, RoutedEventArgs e)
        {
            Firesec.NativeFiresecClient.ExecuteCommand("0\\0\\6\\13", "BoltOpen");
        }

        private void OnBoltClose(object sender, RoutedEventArgs e)
        {

        }

        private void OnBoltStop(object sender, RoutedEventArgs e)
        {

        }

        private void OnBoltAutoOn(object sender, RoutedEventArgs e)
        {

        }

        private void OnBoltAutoOff(object sender, RoutedEventArgs e)
        {

        }

        private void OnAddToIgnoreList(object sender, RoutedEventArgs e)
        {
            Firesec.NativeFiresecClient.AddToIgnoreList(new List<string>() { "0\\0\\0\\0" });
        }

        private void OnRemoveFromIgnoreList(object sender, RoutedEventArgs e)
        {

        }

        private void OnAddCustomMessage(object sender, RoutedEventArgs e)
        {
            Firesec.NativeFiresecClient.AddUserMessage("message");
        }
    }
}
