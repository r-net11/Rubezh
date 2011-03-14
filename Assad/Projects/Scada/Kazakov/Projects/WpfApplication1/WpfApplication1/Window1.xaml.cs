//#define SERIALIZE // НЕ Использовать!!!
//#define XMLMODIFY

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

        Svg2Xaml svg2xaml;
        string sRet, sRet2;
        int cadrDuration1 = 100;
        int cadrDuration2 = 100;
        int cadrDuration3 = 100;

        DispatcherTimer dispatcherTimer = new DispatcherTimer();
        bool tick = false;

        private void button1_Click(object sender, RoutedEventArgs e)
        {

            //dispatcherTimer.Start();

            return;
            DeviceManager deviceManager = new DeviceManager();
            Device dev1 = new Device();
            Device dev2 = new Device();
            State state1 = new State();
            State state2 = new State();
            Cadr cadr1 = new Cadr();
            Cadr cadr2 = new Cadr();
            state1.Id = "норма";
            state2.Id = "тревога";
            cadr1.Image = textbox1.Text;
            cadr2.Image = textbox2.Text;
            state1.Cadrs = new List<Cadr>();
            state2.Cadrs = new List<Cadr>();
            state1.Cadrs.Add(cadr1);
            state2.Cadrs.Add(cadr2);
            dev1.States = new List<State>();
            dev2.States = new List<State>();
            deviceManager.Devices = new List<Device>();
            dev1.Id = "Дев1";
            dev2.Id = "Дев2";
            dev1.States.Add(state1);
            dev1.States.Add(state2);
            dev2.States.Add(state1);
            dev2.States.Add(state2);
            deviceManager.Devices.Add(dev1);
            deviceManager.Devices.Add(dev2);

            // *********Serializing***********
#if SERIALIZE            
            FileStream filexml = new FileStream("file.xml", FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            XmlSerializer serializer = new XmlSerializer(typeof(DeviceManager));
            serializer.Serialize(filexml, deviceManager);
            filexml.Close();
#endif


        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            // *********DeSerializing***********
            FileStream filexml = new FileStream("file.xml", FileMode.Open, FileAccess.Read, FileShare.Read);
            XmlSerializer serializer = new XmlSerializer(typeof(DeviceManager));
            DeviceManager deviceManager = (DeviceManager)serializer.Deserialize(filexml);
            filexml.Close();

            // *********XMLMODIFY***********
#if XMLMODIFY
            {

                Device device = deviceManager.Devices.FirstOrDefault(x => x.Id == "Дев1");
                State state = device.States.FirstOrDefault(x => x.Id == "тревога");
                Cadr cadr = new Cadr();
                cadr.Id = 1;
                cadr.Image = Cadrs.cadr1_1_2;
                cadr.Duration = 500;
                state.Cadrs.Add(cadr);

                //state.Cadrs[1].Image = Cadrs.cadr1_1_2;
                //state.Cadrs[0].Duration = 500;
                //state.Cadrs[1].Duration = 500;
                state.TickEnable = true;
                /*
                //ADD Device
                    Device newDevice = new Device();
                    newDevice.Id = "999";
                    newDevice.States = new List<State>();
                    newDevice.States.Add(new State() { Id = "666" });
                    deviceManager.Devices.Add(newDevice);

                //REMOVE Device
                    deviceManager.Devices.Remove(deviceManager.Devices.FirstOrDefault(x => x.Id == "999"));
                 */

                //Serializing
                FileStream _filexml = new FileStream("file.xml", FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                XmlSerializer _serializer = new XmlSerializer(typeof(DeviceManager));
                _serializer.Serialize(_filexml, deviceManager);
                _filexml.Close();

                return;
            }
#endif

        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            FileStream filexml = new FileStream("file.xml", FileMode.Open, FileAccess.Read, FileShare.Read);
            XmlSerializer serializer = new XmlSerializer(typeof(DeviceManager));
            DeviceManager deviceManager = (DeviceManager)serializer.Deserialize(filexml);
            filexml.Close();

            svg2xaml = new Svg2Xaml();
            string sPath = @"D:\Казаков Р.Б\svg_temlates\";
            string sFile_xsl = sPath + @"\svg2xaml.xsl";

            string id = textbox1.Text;
            string stateId = textbox2.Text;

            Device device = deviceManager.Devices.FirstOrDefault(x => x.Id == id);
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
            sRet = svg2xaml.XSLT_Transform(state.Cadrs[0].Image, sFile_xsl);

            dispatcherTimer.Stop();
            if (state.TickEnable)
            {
                cadrDuration2 = state.Cadrs[1].Duration;
                sRet2 = svg2xaml.XSLT_Transform(state.Cadrs[1].Image, sFile_xsl);
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

        // Add new device
        private void button4_Click(object sender, RoutedEventArgs e)
        {
            // *********DeSerializing***********
            FileStream filexml = new FileStream("file.xml", FileMode.Open, FileAccess.Read, FileShare.Read);
            XmlSerializer serializer = new XmlSerializer(typeof(DeviceManager));
            DeviceManager deviceManager = (DeviceManager)serializer.Deserialize(filexml);
            filexml.Close();
            
            // *********Adding new device***********
            Device newDevice = new Device();
            State newState = new State();
            Cadr newCadr = new Cadr();

            if (deviceManager.Devices.FirstOrDefault(x => x.Id == textbox1.Text) != null)
            {
                MessageBox.Show("Устройство с таким именем уже существует в базе");
                return;
            }

            newDevice.Id = textbox1.Text;
            newState.Id = textbox2.Text;
            newCadr.Id = 0;
            newCadr.Duration = Convert.ToInt32(textbox3.Text);
            newCadr.Image = textbox4.Text;

            newState.Cadrs = new List<Cadr>();
            newState.Cadrs.Add(newCadr);

            newDevice.States = new List<State>();
            newDevice.States.Add(newState);

            deviceManager.Devices.Add(newDevice);

            //Serializing
            FileStream _filexml = new FileStream("file.xml", FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            XmlSerializer _serializer = new XmlSerializer(typeof(DeviceManager));
            _serializer.Serialize(_filexml, deviceManager);
            _filexml.Close();

            return;
        }

        // *********Remove existing device***********
        private void button5_Click(object sender, RoutedEventArgs e)
        {
            // *********DeSerializing***********
            FileStream filexml = new FileStream("file.xml", FileMode.Open, FileAccess.Read, FileShare.Read);
            XmlSerializer serializer = new XmlSerializer(typeof(DeviceManager));
            DeviceManager deviceManager = (DeviceManager)serializer.Deserialize(filexml);
            filexml.Close();

            if (deviceManager.Devices.Remove(deviceManager.Devices.FirstOrDefault(x => x.Id == textbox1.Text)))
            { }
            else
                MessageBox.Show("Устроство с таким именем отсутсвует в базе");

            //Serializing
            FileStream _filexml = new FileStream("file.xml", FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            XmlSerializer _serializer = new XmlSerializer(typeof(DeviceManager));
            _serializer.Serialize(_filexml, deviceManager);
            _filexml.Close();
        }

        // *********Modify existing device***********
        private void button6_Click(object sender, RoutedEventArgs e)
        {
            // *********DeSerializing***********
            FileStream filexml = new FileStream("file.xml", FileMode.Open, FileAccess.Read, FileShare.Read);
            XmlSerializer serializer = new XmlSerializer(typeof(DeviceManager));
            DeviceManager deviceManager = (DeviceManager)serializer.Deserialize(filexml);
            filexml.Close();

            Device device = new Device();
            State state = new State();
            Cadr cadr = new Cadr();
            if ((device = deviceManager.Devices.FirstOrDefault(x => x.Id == textbox1.Text)) != null)
            {
                if ((state = device.States.FirstOrDefault(x => x.Id == textbox2.Text)) != null)
                {
                    if(textbox4.Text.Length != 0)
                        state.Cadrs[Convert.ToInt32(textbox5.Text)].Image = textbox4.Text;

                    if (textbox3.Text.Length != 0)
                        state.Cadrs[Convert.ToInt32(textbox5.Text)].Duration = Convert.ToInt32(textbox3.Text);
                }
                else
                    MessageBox.Show("Состояние с таким именем отсутствует в базе");
            }
            else
                MessageBox.Show("Устроство с таким именем отсутсвует в базе");

            //Serializing
            FileStream _filexml = new FileStream("file.xml", FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            XmlSerializer _serializer = new XmlSerializer(typeof(DeviceManager));
            _serializer.Serialize(_filexml, deviceManager);
            _filexml.Close();
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
