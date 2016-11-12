using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace StrazhAPI.SKD
{
    [DataContract]
    public class SKDDoorWeeklyInterval
    {
        public SKDDoorWeeklyInterval()
        {
            WeeklyIntervalParts = new List<SKDDoorWeeklyIntervalPart>();
        }

        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public List<SKDDoorWeeklyIntervalPart> WeeklyIntervalParts { get; set; }

        public static List<SKDDoorWeeklyIntervalPart> CreateParts()
        {
            var result = new List<SKDDoorWeeklyIntervalPart>
            {
                new SKDDoorWeeklyIntervalPart {DayOfWeek = SKDDayOfWeek.Monday},
                new SKDDoorWeeklyIntervalPart {DayOfWeek = SKDDayOfWeek.Tuesday},
                new SKDDoorWeeklyIntervalPart {DayOfWeek = SKDDayOfWeek.Wednesday},
                new SKDDoorWeeklyIntervalPart {DayOfWeek = SKDDayOfWeek.Thursday},
                new SKDDoorWeeklyIntervalPart {DayOfWeek = SKDDayOfWeek.Friday},
                new SKDDoorWeeklyIntervalPart {DayOfWeek = SKDDayOfWeek.Saturday},
                new SKDDoorWeeklyIntervalPart {DayOfWeek = SKDDayOfWeek.Sunday}
            };
            return result;
        }

        public override bool Equals(object obj)
        {
            if (obj is SKDDoorWeeklyInterval)
                return ((SKDDoorWeeklyInterval)obj).ID == ID;
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public void InvalidateDayIntervals()
        {
            var uids = SKDManager.TimeIntervalsConfiguration.DoorDayIntervals.Select(item => item.UID).ToList();
            WeeklyIntervalParts.ForEach(x =>
            {
                if (!uids.Contains(x.DayIntervalUID))
                    x.DayIntervalUID = Guid.Empty;
            });
        }
    }
}