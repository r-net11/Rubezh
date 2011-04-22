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
using System.Xml.Serialization;
using System.Xml;
using System.Windows.Markup;
using System.Windows.Threading;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    [Serializable]
    public partial class Window1 : Window
    {

        public Window1()
        {
            InitializeComponent();
        }

        Svg2Xaml svg2xaml;
        string sRet, sRet2;

        bool tick = false;

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            //listBox1.Items.Add(DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Second.ToString());
            CommandManager.InvalidateRequerySuggested();
            //listBox1.Items.MoveCurrentToLast();
            //listBox1.SelectedItem = listBox1.Items.CurrentItem;
            //listBox1.ScrollIntoView(listBox1.Items.CurrentItem);
            tick = !tick;
            if (tick)
            {
                try
                {
                    StringReader stringReader = new StringReader(sRet);
                    XmlReader xmlReader = XmlReader.Create(stringReader);
                    Canvas readerLoadButton = (Canvas)XamlReader.Load(xmlReader);
                    contentControl1.Content = readerLoadButton;
                }
                catch { }
            }

            else
            {
                try
                {
                    StringReader stringReader = new StringReader(sRet2);
                    XmlReader xmlReader = XmlReader.Create(stringReader);
                    Canvas readerLoadButton = (Canvas)XamlReader.Load(xmlReader);
                    contentControl1.Content = readerLoadButton;
                }
                catch { }

            }

        }

        private void bt1_Click(object sender, RoutedEventArgs e)
        {
            svg2xaml = new Svg2Xaml();
            string sPath = @"D:\Казаков Р.Б\svg_temlates\";
            string sFile_xsl = sPath + @"\svg2xaml.xsl";

            // Test 2. OK 030409
            //string sFile_xml = sPath + @"\cadr1.svg";     // Or *.xml
            //string sFile_xml2 = sPath + @"\cadr2.svg";     // Or *.xml
            //string sFile_out = sPath + @"\temp.xaml";
            //string sFile_out2 = sPath + @"\temp2.xaml";

            string input1 = textbox1.Text;
            string input2 = textbox2.Text;


            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 300);

            sRet = svg2xaml.XSLT_Transform(input1, sFile_xsl);
            sRet2 = svg2xaml.XSLT_Transform(input2, sFile_xsl);

            /*
            try
            {
                StringReader stringReader = new StringReader(textbox1.Text);
                XmlReader xmlReader = XmlReader.Create(stringReader);
                Canvas readerLoadButton = (Canvas)XamlReader.Load(xmlReader);
                contentControl1.Content = readerLoadButton;
            }
            catch { }
            */
            dispatcherTimer.Start();
            return;

            DeviceManager deviceManager = new DeviceManager();
            Device dev1 = new Device();
            Device dev2 = new Device();
            State state1 = new State();
            State state2 = new State();
            Cadr cadr1 = new Cadr();
            Cadr cadr2 = new Cadr();
            cadr1.Image = "empty";
            cadr1.Image = textbox1.Text;
            state1.Cadrs = new List<Cadr>();
            state1.Cadrs.Add(cadr1);
            dev1.States = new List<State>();
            dev2.States = new List<State>();
            deviceManager.Devices = new List<Device>();
            dev1.Id = 1;
            dev2.Id = 2;
            dev1.States.Add(state1);
            dev2.States.Add(state2);
            deviceManager.Devices.Add(dev1);
            deviceManager.Devices.Add(dev2);

            FileStream filexml = new FileStream("file.xml", FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            XmlSerializer serializer = new XmlSerializer(typeof(DeviceManager));
            serializer.Serialize(filexml, deviceManager);
            filexml.Close();
        }

        private void bt2_Click(object sender, RoutedEventArgs e)
        {
            FileStream filexml = new FileStream("file.xml", FileMode.Open, FileAccess.Read, FileShare.Read);
            XmlSerializer serializer = new XmlSerializer(typeof(DeviceManager));
            DeviceManager deviceManager = (DeviceManager)serializer.Deserialize(filexml);
            filexml.Close();
        }
    }



    [Serializable]
    public class DeviceManager
    {
        public List<Device> Devices;
    }

    [Serializable]
    public class Device
    {
        public int Id { get; set; }
        public List<State> States { get; set; }
    }

    [Serializable]
    public class State
    {
        public int Id { get; set; }
        public List<Cadr> Cadrs { get; set; }
    }

    [Serializable]
    public class Cadr
    {
        public int Duration { get; set; }
        public string Image { get; set; }
    }
}
