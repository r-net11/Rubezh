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
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
        }

        string sRet, sRet2;
        string sFile_xsl = "svg2xaml.xsl";
        int cadrDuration1 = 100;
        int cadrDuration2 = 100;
        DeviceManager deviceManager;

        DispatcherTimer dispatcherTimer = new DispatcherTimer();
        bool tick = false;

     
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            // *********DeSerializing***********
            FileStream filexml = new FileStream("file.xml", FileMode.Open, FileAccess.Read, FileShare.Read);
            XmlSerializer serializer = new XmlSerializer(typeof(DeviceManager));
            deviceManager = (DeviceManager)serializer.Deserialize(filexml);
            filexml.Close();
        }

        private void comboBox1_MouseEnter(object sender, MouseEventArgs e)
        {
            comboBox1.ItemsSource = deviceManager.Devices;
            comboBox1.DisplayMemberPath = "Id";
            comboBox1.SelectedValuePath = "Id";
        }

        private void comboBox2_MouseEnter(object sender, MouseEventArgs e)
        {
            string devId = comboBox1.Text;
            if (devId != "")
            {
                Device device = deviceManager.Devices.FirstOrDefault(x => x.Id == devId);
                comboBox2.ItemsSource = device.States;
                comboBox2.DisplayMemberPath = "Id";
                comboBox2.SelectedValuePath = "Id";
            }
        }

        private void comboBox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            string devId = comboBox1.Text;
            Device device = deviceManager.Devices.FirstOrDefault(x => x.Id == devId);
            string stateId = ((State)(comboBox2.SelectedItem)).Id+"";

            State state = device.States.FirstOrDefault(x => x.Id == stateId);
            if (state == null)
            {
                MessageBox.Show("Нет такого состояния");
                return;
            }
            cadrDuration1 = state.Cadrs[0].Duration;
            sRet = Svg2Xaml.XSLT_Transform(state.Cadrs[0].Image, sFile_xsl);

            dispatcherTimer.Stop();
            if (state.TickEnable)
            {
                cadrDuration2 = state.Cadrs[1].Duration;
                sRet2 = Svg2Xaml.XSLT_Transform(state.Cadrs[1].Image, sFile_xsl);
                dispatcherTimer.Start();
            }
            else
            {
                StringReader stringReader = new StringReader(sRet);
                XmlReader xmlReader = XmlReader.Create(stringReader);
                Canvas readerLoadButton = (Canvas)XamlReader.Load(xmlReader);
                contentControl1.Content = readerLoadButton;
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            contentControl1.Width = stackPanel1.ActualWidth;
            contentControl1.Height = stackPanel1.ActualHeight;
        }
        /****************Таймер*****************/
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            CommandManager.InvalidateRequerySuggested();
            tick = !tick;
            if (tick)
            {
                try
                {
                    dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, cadrDuration1);
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
                    dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, cadrDuration2);
                    StringReader stringReader = new StringReader(sRet2);
                    XmlReader xmlReader = XmlReader.Create(stringReader);
                    Canvas readerLoadButton = (Canvas)XamlReader.Load(xmlReader);
                    contentControl1.Content = readerLoadButton;
                }
                catch { }
            }
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
        public string Id { get; set; }
        public List<State> States { get; set; }
    }

    [Serializable]
    public class State
    {
        public string Id { get; set; }
        public bool TickEnable { get; set; }
        public List<Cadr> Cadrs { get; set; }
    }

    [Serializable]
    public class Cadr
    {
        public int Id { get; set; }
        public int Duration { get; set; }
        public string Image { get; set; }
    }
}
