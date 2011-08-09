using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace FiresecDirect
{
    public partial class FiresecDirectWindow : Window
    {
        public FiresecDirectWindow()
        {
            InitializeComponent();
        }

        void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Firesec.NativeFiresecClient.Connect("adm", "");
        }

        void OnSetNewConfig(object sender, RoutedEventArgs e)
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

        void OnGetCoreConfig(object sender, RoutedEventArgs e)
        {
            string coreConfig = Firesec.NativeFiresecClient.GetCoreConfig();
            textBox1.Text = coreConfig;

            FileStream fileStream = new FileStream("D:/CoreConfig.txt", FileMode.Create);
            StreamWriter streamWriter = new StreamWriter(fileStream);
            streamWriter.Write(coreConfig);
            fileStream.Close();
        }

        void OnGetCoreState(object sender, RoutedEventArgs e)
        {
            textBox1.Text = Firesec.NativeFiresecClient.GetCoreState();
        }

        void OnGetMetaData(object sender, RoutedEventArgs e)
        {
            textBox1.Text = Firesec.NativeFiresecClient.GetMetaData();
        }

        void OnGetCoreDeviceParams(object sender, RoutedEventArgs e)
        {
            textBox1.Text = Firesec.NativeFiresecClient.GetCoreDeviceParams();
        }

        void OnReadEvents(object sender, RoutedEventArgs e)
        {
            textBox1.Text = Firesec.NativeFiresecClient.ReadEvents(0, 100);
        }

        void Button_Click_6(object sender, RoutedEventArgs e)
        {
        }

        void OnBoltOpen(object sender, RoutedEventArgs e)
        {
            Firesec.NativeFiresecClient.ExecuteCommand("0\\0\\6\\13", "BoltOpen");
        }

        void OnBoltClose(object sender, RoutedEventArgs e)
        {
        }

        void OnBoltStop(object sender, RoutedEventArgs e)
        {
        }

        void OnBoltAutoOn(object sender, RoutedEventArgs e)
        {
        }

        void OnBoltAutoOff(object sender, RoutedEventArgs e)
        {
        }

        void OnAddToIgnoreList(object sender, RoutedEventArgs e)
        {
            Firesec.NativeFiresecClient.AddToIgnoreList(new List<string>() { "0\\0\\0\\0" });
        }

        void OnRemoveFromIgnoreList(object sender, RoutedEventArgs e)
        {
        }

        void OnAddCustomMessage(object sender, RoutedEventArgs e)
        {
            Firesec.NativeFiresecClient.AddUserMessage("message");
        }
    }
}