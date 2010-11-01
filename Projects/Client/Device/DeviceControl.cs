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
    public partial class DeviceControl : UserControl
    {
        public DeviceControl()
        {
            InitializeComponent();
        }

        public Device InnerDevice { get; set; }

        public void buttonEvent_Click(object sender, EventArgs e)
        {
            string eventName = (sender as Button).Text;
            InnerDevice.FireEvent(eventName);
        }

        public void ShowUI()
        {
            labelType.Text = InnerDevice.DeviceType;
            labelName.Text = InnerDevice.DeviceName;
            labelId.Text = InnerDevice.DeviceId;

            int top = 80;

            if (InnerDevice.Parameters != null)
                for (int i = 0; i < InnerDevice.Parameters.Count(); i++)
                {
                    Label labelParameterName = new Label();
                    labelParameterName.Text = InnerDevice.Parameters[i].param;
                    labelParameterName.Left = 10;
                    labelParameterName.Top = top;
                    Controls.Add(labelParameterName);

                    TextBox textBoxParameterValue = new TextBox();
                    textBoxParameterValue.Text = InnerDevice.Parameters[i].value;
                    textBoxParameterValue.Left = 150;
                    textBoxParameterValue.Top = top;
                    textBoxParameterValue.Width = 200;
                    textBoxParameterValue.Name = "parameter_" + InnerDevice.Parameters[i].param;
                    Controls.Add(textBoxParameterValue);

                    top += 30;
                }

            labelStates.Top = top;
            top += 20;

            //if (InnerDevice.States != null)
            //    for (int i = 0; i < InnerDevice.States.Count(); i++)
            //    {
            Label labelStateName = new Label();
            //labelStateName.Text = InnerDevice.States[i].Name;
            labelStateName.Text = "Сосояние";
            labelStateName.Left = 10;
            labelStateName.Top = top;
            Controls.Add(labelStateName);

            ComboBox comboBoxStates = new ComboBox();
            comboBoxStates.Left = 150;
            comboBoxStates.Top = top;
            comboBoxStates.Width = 200;
            //comboBoxStates.Name = "state_" + InnerDevice.States[i].Name;
            comboBoxStates.Name = "State";
            comboBoxStates.Items.Add("Тревога");
            comboBoxStates.Items.Add("Внимание (предтревожное)");
            comboBoxStates.Items.Add("Неисправность");
            comboBoxStates.Items.Add("Требуется обслуживание");
            comboBoxStates.Items.Add("Обход устройств");
            comboBoxStates.Items.Add("Неизвестно");
            comboBoxStates.Items.Add("Норма(*)");
            comboBoxStates.Items.Add("орма");
            comboBoxStates.Items.Add("Нет состояния");
            comboBoxStates.SelectedIndex = 8;
            Controls.Add(comboBoxStates);

            InnerDevice.State.PropertyChanged += (CommonState newState) =>
            {
                comboBoxStates.Text = newState.CurrentState;
            };

            top += 30;
            //}

            labelCommands.Top = top;
            top += 20;
            listBoxCommands.Top = top;

            foreach (string command in InnerDevice.Commands)
            {
                listBoxCommands.Items.Add(command);
            }
            top += 100;
            labelEvents.Top = top;
            top += 20;

            if (InnerDevice.DeviceEvents != null)
                for (int i = 0; i < InnerDevice.DeviceEvents.Count(); i++)
                {
                    Button buttonEvent = new Button();
                    buttonEvent.Text = InnerDevice.DeviceEvents[i].@event;
                    buttonEvent.Left = 150;
                    buttonEvent.Top = top;
                    buttonEvent.Width = 200;
                    buttonEvent.Click += buttonEvent_Click;
                    Controls.Add(buttonEvent);

                    top += 30;
                }
            Height = top;
        }
    }
}
