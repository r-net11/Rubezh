using System;
using System.Linq;
using Infrastructure.Common;
using FiresecAPI.Models;

namespace DevicesModule.ViewModels
{
    public class PumpDetailsViewModel : DialogContent
    {
        Device _device;

        public PumpDetailsViewModel(Device device)
        {
            Title = "Свойства насоса";
            SaveCommand = new RelayCommand(OnSave);
            CancelCommand = new RelayCommand(OnCancel);

            _device = device;

            var timeoutProperty = _device.Properties.FirstOrDefault(x => x.Name == "Time");
            if ((timeoutProperty == null) || (timeoutProperty.Value == null))
                Timeout = 0;
            else
                Timeout = Convert.ToInt32(timeoutProperty.Value);
        }

        public string PropertyName
        {
            get
            {
                switch (_device.Driver.DriverName)
                {
                    case "Насос":
                        return "Время выхода на режим, с";

                    case "Жокей-насос":
                        return "Время восстановления давления, мин";

                    case "Компрессор":
                        return "Время восстановления давления, мин";

                    case "Насос компенсации утечек":
                        return "Время аварии пневмоемкости, мин";
                }
                return "";
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

        public RelayCommand SaveCommand { get; private set; }
        void OnSave()
        {
            var timeoutProperty = _device.Properties.FirstOrDefault(x => x.Name == "Time");
            if (timeoutProperty == null)
            {
                timeoutProperty = new Property() { Name = "Time" };
                _device.Properties.Add(timeoutProperty);
            }
            timeoutProperty.Value = Timeout.ToString();

            Close(true);
        }

        public RelayCommand CancelCommand { get; private set; }
        void OnCancel()
        {
            Close(false);
        }
    }
}
