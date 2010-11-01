using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client.UserControls
{
    public partial class DynamicDevice : UserControl
    {
        public DynamicDevice()
        {
            InitializeComponent();
        }

        x.MHconfigTypeDevice innerDevice;
        public x.MHconfigTypeDevice InnerDevice
        {
            get { return innerDevice; }
            set
            {
                innerDevice = value;

                labelType.Text = innerDevice.type;
                labelName.Text = innerDevice.deviceName;
                labelId.Text = innerDevice.deviceId;

                foreach (x.MHconfigTypeDeviceParam param in innerDevice.param)
                {
                    string paramName = param.param;
                    string paramValue = param.value;

                    Control[] controls = Controls.Find("parameter_" + paramName, true);
                    if (controls.Count() > 0)
                    {
                        Control control = controls[0];
                        if (control != null)
                            control.Text = paramValue;
                    }
                }
            }
        }

        public void SetCommand(x.MHdeviceControlType controlType, string refMessageId)
        {
            listBoxCommandValues.Items.Add(controlType.cmdId);

            x.CPconfirmationType confirmation = new x.CPconfirmationType();
            confirmation.commandId = "MHDeviceControl";
            confirmation.status = x.CommandStatus.OK;
            TcpServer.Send(confirmation, refMessageId);
        }

        public x.DeviceType GetState()
        {
            x.DeviceType deviceType = new x.DeviceType();
            deviceType.deviceId = innerDevice.deviceId;


            if (innerModel.state == null)
                return null;

            deviceType.state = new x.DeviceTypeState[innerModel.state.Count()];
            for (int i = 0; i < innerModel.state.Count(); i++ )
            {
                try
                {
                    deviceType.state[i] = new x.DeviceTypeState();
                    deviceType.state[i].state = innerModel.state[i].state;

                    Control control = Controls.Find("state_" + innerModel.state[i].state, true)[0];
                    deviceType.state[i].value = control.Text;
                }
                catch
                {
                }
            }

            return deviceType;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            x.CPeventType eventType = new x.CPeventType();

            eventType.deviceId = InnerDevice.deviceId;
            eventType.eventTime = DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss");
            eventType.eventId = "событие 1";
            eventType.alertLevel = "0";

            eventType.state = new x.CPeventTypeState[innerModel.state.Count()];
            for (int i = 0; i < innerModel.state.Count(); i++)
            {
                try
                {
                    eventType.state[i] = new x.CPeventTypeState();
                    eventType.state[i].state = innerModel.state[i].state;

                    Control control = Controls.Find("state_" + innerModel.state[i].state, true)[0];
                    eventType.state[i].value = control.Text;
                }
                catch
                {
                }
            }

            TcpServer.Send(eventType, null);
        }


        Label labelType;
        Label labelName;
        Label labelId;
        ListBox listBoxCommandValues;

        x.modelInfoType innerModel;

        public void CreateInterface(x.modelInfoType model)
        {
            innerModel = model;

            int top = 0;


            labelType = new Label();
            labelType.Text = "Тип Устройства";
            labelType.Left = 150;
            labelType.Top = top;
            labelType.Font = new Font(new FontFamily("Arial"), 8, FontStyle.Bold);
            Controls.Add(labelType);

            top += 20;

            labelName = new Label();
            labelName.Text = "Имя Устройства";
            labelName.Left = 150;
            labelName.Top = top;
            labelName.Font = new Font(new FontFamily("Arial"), 8, FontStyle.Bold);
            Controls.Add(labelName);

            top += 20;

            labelId = new Label();
            labelId.Text = "Идентификатор Устройства";
            labelId.Left = 150;
            labelId.Top = top;
            labelId.Font = new Font(new FontFamily("Arial"), 8, FontStyle.Bold);
            Controls.Add(labelId);

            top += 20;

            Label labelParameters = new Label();
            labelParameters.Text = "параметры";
            labelParameters.Left = 150;
            labelParameters.Top = top;
            labelParameters.Font = new Font(new FontFamily("Arial"), 8, FontStyle.Bold);
            Controls.Add(labelParameters);

            top += 30;

            if (model.param != null)
                for (int i = 0; i < model.param.Count(); i++)
                {
                    Label labelParameterName = new Label();
                    labelParameterName.Text = model.param[i].param;
                    labelParameterName.Left = 10;
                    labelParameterName.Top = top;
                    Controls.Add(labelParameterName);

                    TextBox textBoxParameterValue = new TextBox();
                    textBoxParameterValue.Text = "здесь будет параметр " + i.ToString();
                    textBoxParameterValue.Left = 150;
                    textBoxParameterValue.Top = top;
                    textBoxParameterValue.Width = 200;
                    textBoxParameterValue.Name = "parameter_" + model.param[i].param;
                    Controls.Add(textBoxParameterValue);

                    top += 30;
                }

            Label labelStates = new Label();
            labelStates.Text = "состояния";
            labelStates.Left = 150;
            labelStates.Top = top;
            labelStates.Font = new Font(new FontFamily("Arial"), 8, FontStyle.Bold);
            Controls.Add(labelStates);

            top += 30;

            if (model.state != null)
                for (int i = 0; i < model.state.Count(); i++)
                {
                    Label labelStateName = new Label();
                    labelStateName.Text = model.state[i].state;
                    labelStateName.Left = 10;
                    labelStateName.Top = top;
                    Controls.Add(labelStateName);

                    TextBox textBoxStateValue = new TextBox();
                    textBoxStateValue.Text = "здесь будет состояние " + i.ToString();
                    textBoxStateValue.Left = 150;
                    textBoxStateValue.Top = top;
                    textBoxStateValue.Width = 200;
                    textBoxStateValue.Name = "state_" + model.state[i].state;
                    Controls.Add(textBoxStateValue);

                    top += 30;
                }

            Label labelCommands = new Label();
            labelCommands.Text = "команды";
            labelCommands.Left = 150;
            labelCommands.Top = top;
            labelCommands.Font = new Font(new FontFamily("Arial"), 8, FontStyle.Bold);
            Controls.Add(labelCommands);

            top += 30;

            listBoxCommandValues = new ListBox();
            listBoxCommandValues.Left = 150;
            listBoxCommandValues.Top = top;
            listBoxCommandValues.Width = 200;
            Controls.Add(listBoxCommandValues);

            top += 100;

            Label labelEvents = new Label();
            labelEvents.Text = "события";
            labelEvents.Left = 150;
            labelEvents.Top = top;
            labelEvents.Font = new Font(new FontFamily("Arial"), 8, FontStyle.Bold);
            Controls.Add(labelEvents);

            top += 30;

            if (model.@event != null)
                for (int i = 0; i < model.@event.Count(); i++)
                {
                    Button buttonEvent = new Button();
                    buttonEvent.Text = model.@event[i].@event;
                    buttonEvent.Left = 150;
                    buttonEvent.Top = top;
                    buttonEvent.Width = 200;
                    buttonEvent.Click += button1_Click;
                    Controls.Add(buttonEvent);

                    top += 30;
                }
        }
    }
}
