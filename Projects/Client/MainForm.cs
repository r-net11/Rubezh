using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using Client.UserControls;
using System.Windows.Forms.Integration;
using Common;
using AssadEmulator;

namespace Client
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        public delegate void ShowDeviceDelegate(Device device);

        public static MainForm form;
        NetManager netManager;
        DeviceViewer.Form1 DeviceViewerForm;

        private void MainForm_Load(object sender, EventArgs e)
        {
            form = this;
            Logger.Logger.Create();
            DeviceTypesManager.LoadTypes();
            netManager = new NetManager();
        }

        private void startButton_Click(object sender, EventArgs e)
        {             
            startButton.Enabled = false;
            netManager.Start();
            netManager.SendBroadcastUdp();

            AssadEmulator.App.Create();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            netManager.Stop();
        }

        private void deviceViewerButton_Click(object sender, EventArgs e)
        {
            Common.EventAggregator.PropertyChanged += new Common.EventAggregator.PropertyChangedDelegate(DeviceManager.EventAggregator_PropertyChanged);
            DeviceViewerForm = new DeviceViewer.Form1();
            DeviceViewerForm.Show();
        }

        public static void Config(x.MHconfigTypeDevice device, bool all)
        {
            DeviceManager.Config(device, all);
            form.Invoke(new Action(form.RefreshTreeView));
        }

        public static void RemoveDevice(x.MHremoveDeviceType content, string refMessageId)
        {
            DeviceManager.RemoveDevice(content, refMessageId);
            form.Invoke(new Action(form.RefreshTreeView));
        }

        void RefreshTreeView()
        {
            deviceTreeView.Nodes.Clear();

            Device device = DeviceManager.Devices[0];
            TreeNode node = deviceTreeView.Nodes.Add(device.InnerDevice.deviceId, device.InnerDevice.deviceName);
            AddChildNode(node, device);

            deviceTreeView.ExpandAll();
        }

        void AddChildNode(TreeNode parentNode, Client.Device parentDevice)
        {
            foreach (Client.Device childDevice in parentDevice.Children)
            {
                TreeNode childNode = parentNode.Nodes.Add(childDevice.InnerDevice.deviceId, childDevice.InnerDevice.deviceName);
                AddChildNode(childNode, childDevice);
            }
        }

        void ShowDevice(Device device)
        {
            DeviceControl deviceControl = new DeviceControl();
            deviceControl.InnerDevice = device;
            deviceControl.ShowUI();
            panelDevice.Controls.Clear();
            panelDevice.Controls.Add(deviceControl);

            deviceViewerTextBox.Text = deviceControl.InnerDevice.Address;
            parentAddressTextBox.Text = deviceControl.InnerDevice.ParentAddress;
        }

        private void deviceTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            Device device = DeviceManager.Devices.First(x => x.DeviceId == e.Node.Name);
            ShowDevice(device);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Logger.Logger.form.Show();
        }
    }
}