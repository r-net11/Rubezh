using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using ClientApi;

namespace ServiceVisualizer
{
    public class ZoneViewModel : BaseViewModel
    {
        public ZoneViewModel()
        {
        }

        Zone zone;

        public void SetZone(Zone zone)
        {
            this.zone = zone;
            ZoneName = zone.Name;
            zoneNo = zone.Id;
            ZoneDescription = zone.Description;
            ZoneDetectorCount = zone.DetectorCount;
            ZoneEvacuationTime = zone.EvacuationTime;

        }

        string zoneName;
        public string ZoneName
        {
            get { return zoneName; }
            set
            {
                zoneName = value;
                OnPropertyChanged("ZoneName");
            }
        }

        string zoneNo;
        public string ZoneNo
        {
            get { return zoneNo; }
            set
            {
                zoneNo = value;
                OnPropertyChanged("ZoneNo");
            }
        }

        string zoneDescription;
        public string ZoneDescription
        {
            get { return zoneDescription; }
            set
            {
                zoneDescription = value;
                OnPropertyChanged("ZoneDescription");
            }
        }

        string zoneDetectorCount;
        public string ZoneDetectorCount
        {
            get { return zoneDetectorCount; }
            set
            {
                zoneDetectorCount = value;
                OnPropertyChanged("ZoneDetectorCount");
            }
        }

        string zoneEvacuationTime;
        public string ZoneEvacuationTime
        {
            get { return zoneEvacuationTime; }
            set
            {
                zoneEvacuationTime = value;
                OnPropertyChanged("ZoneEvacuationTime");
            }
        }
    }
}
