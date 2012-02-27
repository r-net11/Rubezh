using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [DataContract]
    public class Clause
    {
        public Clause()
        {
            Zones = new List<ulong>();
            Operation = ZoneLogicOperation.Any;
        }

        public Device Device { get; set; }

        [DataMember]
        public ZoneLogicState State { get; set; }

        [DataMember]
        public ZoneLogicOperation? Operation { get; set; }

        [DataMember]
        public List<ulong> Zones { get; set; }

        [DataMember]
        public Guid DeviceUID { get; set; }

        public bool CanSelectOperation
        {
            get
            {
                return (State == ZoneLogicState.Fire) ||
                    (State == ZoneLogicState.Attention) ||
                    (State == ZoneLogicState.MPTAutomaticOn) ||
                    (State == ZoneLogicState.MPTOn) ||
                    (State == ZoneLogicState.Alarm) ||
                    (State == ZoneLogicState.GuardSet) ||
                    (State == ZoneLogicState.GuardUnSet);
            }
        }
    }
}