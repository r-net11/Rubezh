using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;

namespace Client
{
    public partial class DeviceLine : DeviceCommon
    {
        public DeviceLine()
        {
            InitializeComponent();
        }

        //x.MHconfigTypeDevice device;
        public override x.MHconfigTypeDevice InnerDevice
        {
            get { return innerDevice; }
            set
            {
                base.InnerDevice = value;
                //innerDevice = value;
                textBox1.Text = innerDevice.param.First(x => x.param == "порт").value;
                comboBox2.Text = innerDevice.param.First(x => x.param == "скорость").value;
            }
        }

        public void SetCommand(x.MHdeviceControlType controlType, string refMessageId)
        {
            listBox1.Items.Add(controlType.cmdId);

            x.CPconfirmationType confirmation = new x.CPconfirmationType();
            confirmation.commandId = "MHDeviceControl";
            confirmation.status = x.CommandStatus.OK;
            TcpServer.Send(confirmation, refMessageId);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            x.CPeventType eventType = new x.CPeventType();

            eventType.deviceId = InnerDevice.deviceId;
            eventType.eventTime = DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss");
            eventType.eventId = "событие 1";
            eventType.alertLevel = "0";

            eventType.state = new x.CPeventTypeState[2];
            eventType.state[0] = new x.CPeventTypeState();
            eventType.state[0].state = "состояние";
            eventType.state[0].value = comboBox3.Text;
            eventType.state[1] = new x.CPeventTypeState();
            eventType.state[1].state = "ошибка";
            eventType.state[1].value = comboBox4.Text;

            TcpServer.Send(eventType, null);
        }

        public override x.DeviceType GetState()
        {
            x.DeviceType deviceType = new x.DeviceType();
            deviceType.deviceId = DeviceId;
            deviceType.state = new x.DeviceTypeState[2];
            deviceType.state[0] = new x.DeviceTypeState();
            deviceType.state[0].state = "состояние";
            deviceType.state[0].value = comboBox3.Text;
            deviceType.state[1] = new x.DeviceTypeState();
            deviceType.state[1].state = "ошибка";
            deviceType.state[1].value = comboBox4.Text;

            return deviceType;
        }
    }
}
