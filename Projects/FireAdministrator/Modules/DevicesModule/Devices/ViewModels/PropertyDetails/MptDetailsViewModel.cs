using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class MptDetailsViewModel : SaveCancelDialogContent
    {
        public MptDetailsViewModel(Device device)
        {
            Title = "Параметры модуля пожаротушения";
            _device = device;

            int timeout = 0;

            var timeoutProperty = _device.Properties.FirstOrDefault(x => x.Name == "Timeout");
            if ((timeoutProperty == null) || (timeoutProperty.Value == null))
                timeout = 0;
            else
                timeout = int.Parse(timeoutProperty.Value);

            TimeoutMinutes = timeout / 60;
            TimeoutSeconds = timeout % 60;

            var actionProperty = _device.Properties.FirstOrDefault(x => x.Name == "Config");
            if ((actionProperty == null) || (actionProperty.Value == null))
                IsAutoBlock = false;
            else
                IsAutoBlock = true;
        }

        Device _device;

        int _timeoutMinutes;
        public int TimeoutMinutes
        {
            get { return _timeoutMinutes; }
            set
            {
                _timeoutMinutes = value;
                OnPropertyChanged("TimeoutMinutes");
            }
        }

        int _timeoutSeconds;
        public int TimeoutSeconds
        {
            get { return _timeoutSeconds; }
            set
            {
                _timeoutSeconds = value;
                OnPropertyChanged("TimeoutSeconds");
            }
        }

        bool _isAutoBlock;
        public bool IsAutoBlock
        {
            get { return _isAutoBlock; }
            set
            {
                _isAutoBlock = value;
                OnPropertyChanged("IsAutoBlock");
            }
        }

        protected override void Save(ref bool cancel)
        {
            var timeoutProperty = _device.Properties.FirstOrDefault(x => x.Name == "Timeout");
            if (timeoutProperty == null)
            {
                timeoutProperty = new Property() { Name = "Timeout" };
                _device.Properties.Add(timeoutProperty);
            }
            timeoutProperty.Value = (TimeoutMinutes * 60 + TimeoutSeconds).ToString();

            var actionProperty = _device.Properties.FirstOrDefault(x => x.Name == "Config");
            if (actionProperty == null)
            {
                actionProperty = new Property()
                {
                    Name = "Action",
                    Value = "1"
                };
                _device.Properties.Add(actionProperty);
            }
            if (IsAutoBlock)
            {
                _device.Properties.Remove(actionProperty);
            }
        }
    }
}