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
            ZoneUIDs = new List<Guid>();
            State = ZoneLogicState.Fire;
            Operation = ZoneLogicOperation.Any;
			Devices = new List<Device>();
            Zones = new List<Zone>();
			DeviceUIDs = new List<Guid>();
        }

        public List<Device> Devices { get; set; }
        public List<Zone> Zones { get; set; }

        [DataMember]
        public ZoneLogicState State { get; set; }

        [DataMember]
        public ZoneLogicOperation? Operation { get; set; }

        [DataMember]
        public List<Guid> ZoneUIDs { get; set; }

		[DataMember]
		public List<Guid> DeviceUIDs { get; set; }

        [DataMember]
        public ZoneLogicMROMessageNo ZoneLogicMROMessageNo { get; set; }

        [DataMember]
        public ZoneLogicMROMessageType ZoneLogicMROMessageType { get; set; }

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

        public Clause Clone()
        {
            return new Clause()
            {
                Devices = Devices,
                Zones = Zones,
                State = State,
                Operation = Operation,
                ZoneUIDs = ZoneUIDs,
                DeviceUIDs = DeviceUIDs,
                ZoneLogicMROMessageNo = ZoneLogicMROMessageNo,
                ZoneLogicMROMessageType = ZoneLogicMROMessageType
            };
        }
    }
}