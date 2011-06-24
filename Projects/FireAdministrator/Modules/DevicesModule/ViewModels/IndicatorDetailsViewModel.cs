using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using FiresecClient;
using System.Collections.ObjectModel;
using FiresecClient.Models;

namespace DevicesModule.ViewModels
{
    public class IndicatorDetailsViewModel : DialogContent
    {
        public IndicatorDetailsViewModel()
        {
            Title = "Свойства индикатора";
            SaveCommand = new RelayCommand(OnSave);
            CancelCommand = new RelayCommand(OnCancel);
        }

        public void Initialize(Device device)
        {
            if (device.Properties == null)
                return;

            Logic = device.Properties[0].Value;

            var indicatorLogic = FiresecInternalClient.GetIndicatorLogic(Logic);
            if (indicatorLogic == null)
                return;

            Zones = new ObservableCollection<string>();
            if (indicatorLogic.zone != null)
            {
                IsZone = true;

                foreach (var zone in indicatorLogic.zone)
                    Zones.Add(zone);
            }

            if (indicatorLogic.device != null)
            {
                IsDevice = true;

                var indicatorDevice = indicatorLogic.device[0];
                DeviceId = indicatorDevice.UID;
                OnColor = indicatorDevice.state1;
                OffColor = indicatorDevice.state2;
                FailureColor = indicatorDevice.state3;
                ConnectionColor = indicatorDevice.state4;
            }
        }

        bool _isZone;
        public bool IsZone
        {
            get { return _isZone; }
            set
            {
                _isZone = value;
                OnPropertyChanged("IsZone");
            }
        }

        bool _isDevice;
        public bool IsDevice
        {
            get { return _isDevice; }
            set
            {
                _isDevice = value;
                OnPropertyChanged("IsDevice");
            }
        }

        string _logic;
        public string Logic
        {
            get { return _logic; }
            set
            {
                _logic = value;
                OnPropertyChanged("Logic");
            }
        }

        ObservableCollection<string> _zones;
        public ObservableCollection<string> Zones
        {
            get { return _zones; }
            set
            {
                _zones = value;
                OnPropertyChanged("Zones");
            }
        }

        public ObservableCollection<string> Colors
        {
            get
            {
                ObservableCollection<string> colors = new ObservableCollection<string>();
                colors.Add("0");
                colors.Add("1");
                colors.Add("2");
                colors.Add("3");
                colors.Add("4");
                colors.Add("5");
                colors.Add("6");
                return colors;
            }
        }

        string _onColor;
        public string OnColor
        {
            get { return _onColor; }
            set
            {
                _onColor = value;
                OnPropertyChanged("OnColor");
            }
        }

        string _offColor;
        public string OffColor
        {
            get { return _offColor; }
            set
            {
                _offColor = value;
                OnPropertyChanged("OffColor");
            }
        }

        string _failureColor;
        public string FailureColor
        {
            get { return _failureColor; }
            set
            {
                _failureColor = value;
                OnPropertyChanged("FailureColor");
            }
        }

        string _connectionColor;
        public string ConnectionColor
        {
            get { return _connectionColor; }
            set
            {
                _connectionColor = value;
                OnPropertyChanged("ConnectionColor");
            }
        }

        string _deviceId;
        public string DeviceId
        {
            get { return _deviceId; }
            set
            {
                _deviceId = value;
                OnPropertyChanged("DeviceId");
            }
        }

        public RelayCommand SaveCommand { get; private set; }
        void OnSave()
        {
            Close(true);
        }

        public RelayCommand CancelCommand { get; private set; }
        void OnCancel()
        {
            Close(false);
        }
    }
}
