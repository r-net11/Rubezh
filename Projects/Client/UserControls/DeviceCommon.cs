using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client
{
    public partial class DeviceCommon : UserControl
    {
        public DeviceCommon()
        {
            InitializeComponent();
        }

        protected x.MHconfigTypeDevice innerDevice;
        string deviceType;
        string deviceName;
        string deviceId;

        public virtual x.MHconfigTypeDevice InnerDevice
        {
            get { return innerDevice; }
            set
            {
                innerDevice = value;
                DeviceType = innerDevice.type;
                DeviceName = innerDevice.deviceName;
                DeviceId = innerDevice.deviceId;
            }
        }

        public string DeviceType
        {
            get { return deviceType; }
            set
            {
                deviceType = value;
                TypeTextBox.Text = deviceType;
            }
        }

        public string DeviceName
        {
            get { return deviceName; }
            set
            {
                deviceName = value;
                NameTextBox.Text = deviceName;
            }
        }

        public string DeviceId
        {
            get { return deviceId; }
            set
            {
                deviceId = value;
                IdTextBox.Text = deviceId;
            }
        }

        public virtual x.DeviceType GetState()
        {
            return null;
        }
    }
}
