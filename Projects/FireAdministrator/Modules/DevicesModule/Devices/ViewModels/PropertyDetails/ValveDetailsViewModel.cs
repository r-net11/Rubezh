using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class ValveDetailsViewModel : SaveCancelDialogContent
    {
        public ValveDetailsViewModel(Device device)
        {
            Title = "Свойства Задвижки";

            _device = device;

            var actionProperty = _device.Properties.FirstOrDefault(x => x.Name == "Action");
            if ((actionProperty == null) || (actionProperty.Value == null))
                SelectiedAction = "0";
            else
                SelectiedAction = actionProperty.Value;

            var timeoutProperty = _device.Properties.FirstOrDefault(x => x.Name == "Timeout");
            if ((timeoutProperty == null) || (timeoutProperty.Value == null))
                Timeout = 0;
            else
                Timeout = int.Parse(timeoutProperty.Value);
        }

        Device _device;

        public List<string> Actions
        {
            get
            {
                var actions = new List<string>() { "0", "1" };
                return actions;
            }
        }

        string _selectiedAction;
        public string SelectiedAction
        {
            get { return _selectiedAction; }
            set
            {
                _selectiedAction = value;
                OnPropertyChanged("SelectiedAction");
            }
        }

        int _timeout;
        public int Timeout
        {
            get { return _timeout; }
            set
            {
                _timeout = value;
                OnPropertyChanged("Timeout");
            }
        }

        protected override void Save(ref bool cancel)
        {
            var actionProperty = _device.Properties.FirstOrDefault(x => x.Name == "Action");
            if (actionProperty == null)
            {
                actionProperty = new Property() { Name = "Action" };
                _device.Properties.Add(actionProperty);
            }
            actionProperty.Value = SelectiedAction;

            var timeoutProperty = _device.Properties.FirstOrDefault(x => x.Name == "Timeout");
            if (timeoutProperty == null)
            {
                timeoutProperty = new Property() { Name = "Timeout" };
                _device.Properties.Add(timeoutProperty);
            }
            timeoutProperty.Value = Timeout.ToString();
        }
    }
}