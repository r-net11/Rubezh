﻿//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Runtime.Serialization;
//using ServiceApi;

//namespace ServiseProcessor
//{
//    public class Zone
//    {
//        public string Name { get; set; }
//        public string No { get; set; }
//        public string DetectorCount { get; set; }
//        public string EvacuationTime { get; set; }
//        public string Description { get; set; }
//        public string ValidationError { get; set; }
//        public List<Device> Devices { get; set; }
//        public string State { get; set; }
//        public bool ZoneChanged { get; set; }

//        public ShortZone ToShortZone()
//        {
//            ShortZone shortZone = new ShortZone();
//            shortZone.No = this.No;
//            shortZone.Name = this.Name;
//            shortZone.DetectorCount = this.DetectorCount;
//            shortZone.EvacuationTime = this.EvacuationTime;
//            shortZone.Description = this.Description;
//            return shortZone;
//        }

//        public ShortZoneState ToShortZoneState()
//        {
//            ShortZoneState shortZoneState = new ShortZoneState();
//            shortZoneState.No = this.No;
//            shortZoneState.State = this.State;
//            return shortZoneState;
//        }
//    }
//}
