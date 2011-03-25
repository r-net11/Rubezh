using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Windows.Markup;
using System.Windows.Threading;

namespace RubezhDevices
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    [Serializable]
    public partial class RubezhDevice : UserControl
    {
        public RubezhDevice()
        {
            InitializeComponent();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            // *********DeSerializing***********
            FileStream filexml = new FileStream(deviceLibrary_xml, FileMode.Open, FileAccess.Read, FileShare.Read);
            XmlSerializer serializer = new XmlSerializer(typeof(DeviceManager));
            deviceManager = (DeviceManager)serializer.Deserialize(filexml);
            filexml.Close();

            svgWidth = 500.0;
            svgHeight = 500.0;
        }

        string cadr1, cadr2;
        int cadrDuration1 = 100;
        int cadrDuration2 = 100;
        DeviceManager deviceManager;

        Canvas readerLoadButton = new Canvas();
        double svgWidth, svgHeight;
        static public string svg2xaml_xsl = @"c:\Rubezh\Assad\Projects\ActivexDevices\Library\svg2xaml.xsl";
        static public string deviceLibrary_xml = @"c:\Rubezh\Assad\Projects\ActivexDevices\Library\DeviceLibrary.xml";

        DispatcherTimer dispatcherTimer = new DispatcherTimer();
        bool tick = false;

        string driverId;
        public string DriverId
        {
            get { return driverId; }
            set
            {
                driverId = value;
                string stateId = "Базовый рисунок";
                ShowDevice(driverId, stateId);
            }
        }

        void ShowDevice(string driverId, string stateId)
        {
            string driverName = FiresecMetadata.DriversHelper.GetDriverNameById(driverId);
            Device device = deviceManager.Devices.FirstOrDefault(x => x.Id == driverName);
            if (device == null)
            {
                MessageBox.Show("Нет такого устройства");
                return;
            }

            State state = device.States.FirstOrDefault(x => x.Id == stateId);
            if (state == null)
            {
                MessageBox.Show("Нет такого состояния");
                return;
            }

            cadrDuration1 = state.Cadrs[0].Duration;
            cadr1 = Svg2Xaml.XSLT_Transform(state.Cadrs[0].Image, svg2xaml_xsl);

            dispatcherTimer.Stop();
            if (state.Cadrs.Count > 1)
            {
                cadrDuration2 = state.Cadrs[1].Duration;
                cadr2 = Svg2Xaml.XSLT_Transform(state.Cadrs[1].Image, svg2xaml_xsl);
                dispatcherTimer.Start();
            }
            else
            {
                StringReader stringReader = new StringReader(cadr1);
                XmlReader xmlReader = XmlReader.Create(stringReader);
                readerLoadButton = (Canvas)XamlReader.Load(xmlReader);
                contentControl1.Content = readerLoadButton;
            }
        }
     
        private void comboBoxStates_MouseEnter(object sender, MouseEventArgs e)
        {
            string driverName = FiresecMetadata.DriversHelper.GetDriverNameById(DriverId);
            if (driverName != "")
            {
                Device device = deviceManager.Devices.FirstOrDefault(x => x.Id == driverName);
                comboBoxStates.ItemsSource = device.States;
                comboBoxStates.DisplayMemberPath = "Id";
                comboBoxStates.SelectedValuePath = "Id";
            }
        }

        private void comboBoxStates_DropDownClosed(object sender, EventArgs e)
        {
            if (comboBoxStates.Text == "")
                return;
            string devId = DriverId;
            string stateId = ((State)(comboBoxStates.SelectedItem)).Id+"";
            ShowDevice(driverId, stateId);            
            comboBoxStates.Visibility = System.Windows.Visibility.Hidden;
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            double widthCoeff = grid1.ActualWidth / svgWidth;
            double heightCoeff = grid1.ActualHeight / svgHeight;
            double minCoeff = Math.Min(widthCoeff, heightCoeff);
            ScaleTransform scaleTransform1 = new ScaleTransform(minCoeff, minCoeff, 0, 0);
            contentControl1.RenderTransform = scaleTransform1;
            comboBoxStates.Width = Math.Min(contentControl1.ActualWidth, contentControl1.ActualHeight);
            //textBox1.FontSize = (grid1.ActualWidth + grid1.ActualHeight - 50) / 20;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            comboBoxStates.Visibility = System.Windows.Visibility.Hidden;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)e.OriginalSource;
            string theHeader = menuItem.Header.ToString();
            comboBoxStates.Width = Math.Min(contentControl1.ActualWidth, contentControl1.ActualHeight);
            comboBoxStates.Visibility = System.Windows.Visibility.Visible;
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
                    StringReader stringReader = new StringReader(cadr1);
                    XmlReader xmlReader = XmlReader.Create(stringReader);
                    Canvas readerLoadButton = (Canvas)XamlReader.Load(xmlReader);
                    contentControl1.Content = readerLoadButton;
                    contentControl1.UpdateLayout();
                    contentControl1.InvalidateVisual();
                    UpdateLayout();
                    InvalidateVisual();
                }
                catch { }
            }

            else
            {
                try
                {
                    dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, cadrDuration2);
                    StringReader stringReader = new StringReader(cadr2);
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
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Id { get; set; }
        public List<State> States { get; set; }
    }

    [Serializable]
    public class State
    {
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Id { get; set; }
        public List<Cadr> Cadrs { get; set; }
    }

    [Serializable]
    public class Cadr
    {
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int Id { get; set; }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int Duration { get; set; }
        public string Image { get; set; }
    }
}
