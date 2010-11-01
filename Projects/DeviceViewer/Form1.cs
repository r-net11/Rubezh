using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;
using Common;
using System.Windows.Forms.Integration;
using System.Threading;

namespace DeviceViewer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            InitialiseConfiguration();
        }

        public void InitialiseConfiguration()
        {
            ComDeviceManager.Load();
            RefreshTreeView();

            DeviceWather deviceWather = new DeviceWather();
            deviceWather.PropertyChanged += new Action<string>(deviceWather_PropertyChanged);
            deviceWather.Start();
        }

        void deviceWather_PropertyChanged(string obj)
        {
            Invoke(new Action<string>(RefreshDeviceState), obj);
        }

        void RefreshDeviceState(string message)
        {
            richTextBox1.Text = message;
        }

        void RefreshTreeView()
        {
            ComDevice rootDevice = ComDeviceManager.Devices.Find(x => x.Parent == null);

            TreeNode rootNode = deviceTreeView.Nodes.Add(rootDevice.PlaceInTree, rootDevice.DeviceName);
            AddNode(rootNode, rootDevice);

            deviceTreeView.ExpandAll();
        }

        void AddNode(TreeNode parentNode, ComDevice parentDevice)
        {
            if (parentDevice.Children != null)
                foreach (ComDevice childDevice in parentDevice.Children)
                {
                    TreeNode childNode = parentNode.Nodes.Add(childDevice.PlaceInTree, childDevice.DeviceName);
                    AddNode(childNode, childDevice);
                }
        }
        
        private void deviceTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            ComDevice device = ComDeviceManager.Devices.First(x => x.PlaceInTree == e.Node.Name);

            dataGridView1.Rows.Clear();
            if (device.Parameters != null)
                foreach (ComState state in device.States)
                {
                    int index = dataGridView1.Rows.Add(state.Id, state.Name, state.Priority, state.IsActive);
                    if (state.IsActive)
                        dataGridView1.Rows[index].DefaultCellStyle.BackColor = Color.SkyBlue;
                }

            ZoneTextBox.Text = "";
            if (device.InnerType.inZ != null)
                ZoneTextBox.Text = device.InnerType.inZ[0].idz;

            PlaceInTreeTextBox.Text = device.PlaceInTree;
            DriverIdTextBox.Text = device.DriverId;
            MetadataDriverIdTextBox.Text = device.MetadataDriverId;
            AddressTextBox.Text = device.Address;
            ParentAddressTextBox.Text = device.ParentAddress;
        }
    }
}
