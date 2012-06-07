using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace DevicesModule.ViewModels
{
	public class ZoneDetailsViewModel : SaveCancelDialogViewModel
    {
        bool _isNew;
        public Zone Zone;

        public ZoneDetailsViewModel(Zone zone = null)
        {
            if (zone == null)
            {
                _isNew = true;
                Title = "Создание новой зоны";

                Zone = new Zone()
                {
                    Name = "Новая зона",
                    No = 1
                };
                if (FiresecManager.DeviceConfiguration.Zones.Count != 0)
                    Zone.No = FiresecManager.DeviceConfiguration.Zones.Select(x => x.No).Max() + 1;
            }
            else
            {
                _isNew = false;
                Title = string.Format("Свойства зоны: {0}", zone.PresentationName);
                Zone = zone;
            }
            CopyProperties();
        }

        void CopyProperties()
        {
            ZoneType = Zone.ZoneType;
            Name = Zone.Name;
            No = Zone.No;
            Description = Zone.Description;
            DetectorCount = Zone.DetectorCount;
            EvacuationTime = Zone.EvacuationTime;
            AutoSet = Zone.AutoSet;
            Delay = Zone.Delay;
            Skipped = Zone.Skipped;
            GuardZoneType = Zone.GuardZoneType;
        }

        public List<ZoneType> AvailableZoneTypes
        {
            get { return Enum.GetValues(typeof(ZoneType)).Cast<ZoneType>().ToList(); }
        }

        ZoneType _zoneType;
        public ZoneType ZoneType
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

        ulong _no;
        public ulong No
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

        int _detectorCount;
        public int DetectorCount
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
                return (GuardZoneType != GuardZoneType.Ordinary);
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

        public List<GuardZoneType> AvailableGuardZoneTypes
        {
            get { return Enum.GetValues(typeof(GuardZoneType)).Cast<GuardZoneType>().ToList(); }
        }

        GuardZoneType _guardZoneType;
        public GuardZoneType GuardZoneType
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
                return ZoneType == ZoneType.Fire;
            }
        }

        public bool IsGuardZone
        {
            get
            {
                return ZoneType == ZoneType.Guard;
            }
        }

		protected override bool Save()
		{
            if (Zone.No != No && FiresecManager.DeviceConfiguration.Zones.Any(x => x.No == No))
            {
                MessageBoxService.Show("Зона с таким номером уже существует");
				return false;
            }

            if (Zone.No != No)
            {
                foreach (var device in FiresecManager.DeviceConfiguration.Devices)
                {
                    if (device.ZoneNo == Zone.No)
                        device.ZoneNo = No;
                }
            }

            Zone.ZoneType = ZoneType;
            Zone.Name = Name;
            Zone.No = No;
            Zone.Description = Description;
            Zone.DetectorCount = DetectorCount;
            Zone.EvacuationTime = EvacuationTime;
            Zone.AutoSet = AutoSet;
            Zone.Delay = Delay;
            Zone.Skipped = Skipped;
            Zone.GuardZoneType = GuardZoneType;
			return base.Save();
		}
    }
}