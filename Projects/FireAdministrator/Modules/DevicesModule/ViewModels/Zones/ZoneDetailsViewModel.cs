using System.Collections.Generic;
using System.Linq;
using FiresecClient;
using Infrastructure.Common;
using FiresecAPI.Models;

namespace DevicesModule.ViewModels
{
    public class ZoneDetailsViewModel : DialogContent
    {
        public ZoneDetailsViewModel(Zone zone)
        {
            Title = "Свойства зоны";
            SaveCommand = new RelayCommand(OnSave);
            CancelCommand = new RelayCommand(OnCancel);

            _zone = zone;
            ZoneType = zone.ZoneType;
            Name = zone.Name;
            No = zone.No;
            Description = zone.Description;
            DetectorCount = zone.DetectorCount;
            EvacuationTime = zone.EvacuationTime;
            AutoSet = zone.AutoSet;
            Delay = zone.Delay;
            Skipped = zone.Skipped;
            GuardZoneType = zone.GuardZoneType;
        }

        Zone _zone;

        public List<string> AvailableZoneTypes
        {
            get
            {
                List<string> values = new List<string>();
                values.Add("0");
                values.Add("1");
                return values;
            }
        }

        string _zoneType;
        public string ZoneType
        {
            get { return _zoneType; }
            set
            {
                _zoneType = value;
                OnPropertyChanged("ZoneType");
                OnPropertyChanged("IsFireZone");
                OnPropertyChanged("IsGuardZone");
            }
        }

        string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        string _no;
        public string No
        {
            get { return _no; }
            set
            {
                _no = value;
                OnPropertyChanged("No");
            }
        }

        string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged("Description");
            }
        }

        string _detectorCount;
        public string DetectorCount
        {
            get { return _detectorCount; }
            set
            {
                _detectorCount = value;
                OnPropertyChanged("DetectorCount");
            }
        }

        string _evacuationTime;
        public string EvacuationTime
        {
            get { return _evacuationTime; }
            set
            {
                _evacuationTime = value;
                OnPropertyChanged("EvacuationTime");
            }
        }

        string _delay;
        public string Delay
        {
            get { return _delay; }
            set
            {
                _delay = value;
                OnPropertyChanged("Delay");
            }
        }

        public bool CanAutoSet
        {
            get
            {
                return (GuardZoneType != "0");
            }
        }

        string _autoSet;
        public string AutoSet
        {
            get { return _autoSet; }
            set
            {
                _autoSet = value;
                OnPropertyChanged("AutoSet");
            }
        }

        bool _skipped;
        public bool Skipped
        {
            get { return _skipped; }
            set
            {
                _skipped = value;
                OnPropertyChanged("Skipped");
            }
        }

        public List<string> AvailableGuardZoneTypes
        {
            get
            {
                List<string> values = new List<string>();
                values.Add("0");
                values.Add("1");
                values.Add("2");
                return values;
            }
        }

        string _guardZoneType;
        public string GuardZoneType
        {
            get { return _guardZoneType; }
            set
            {
                _guardZoneType = value;
                OnPropertyChanged("GuardZoneType");
                OnPropertyChanged("CanAutoSet");
            }
        }

        public bool IsFireZone
        {
            get
            {
                return ZoneType == "0";
            }
        }

        public bool IsGuardZone
        {
            get
            {
                return ZoneType == "1";
            }
        }

        public RelayCommand SaveCommand { get; private set; }
        void OnSave()
        {
            if ((_zone.No != No) && (FiresecManager.Configuration.Zones.Any(x => x.No == No)))
            {
                System.Windows.MessageBox.Show("Зона с таким номером уже существует");
                return;
            }

            if (_zone.No != No)
            {
                foreach (var device in FiresecManager.Configuration.Devices)
                {
                    if (device.ZoneNo == _zone.No)
                        device.ZoneNo = No;
                }
            }

            _zone.ZoneType = ZoneType;
            _zone.Name = Name;
            _zone.No = No;
            _zone.Description = Description;
            _zone.DetectorCount = DetectorCount;
            _zone.EvacuationTime = EvacuationTime;
            _zone.AutoSet = AutoSet ;
            _zone.Delay = Delay;
            _zone.Skipped = Skipped;
            _zone.GuardZoneType = GuardZoneType;
            Close(true);
        }

        public RelayCommand CancelCommand { get; private set; }
        void OnCancel()
        {
            Close(false);
        }
    }
}
