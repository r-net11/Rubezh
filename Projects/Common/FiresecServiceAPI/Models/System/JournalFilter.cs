using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [DataContract]
    public class JournalFilter
    {
        public JournalFilter()
        {
            LastRecordsCount = 100;
            LastDaysCount = 10;
            Events = new List<StateType>();
            Categories = new List<DeviceCategoryType>();
        }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int LastRecordsCount { get; set; }

        [DataMember]
        public int LastDaysCount { get; set; }

        [DataMember]
        public bool IsLastDaysCountActive { get; set; }

        [DataMember]
        public List<StateType> Events { get; set; }

        [DataMember]
        public List<DeviceCategoryType> Categories { get; set; }

        public bool CheckDaysConstraint(DateTime dateTime)
        {
            if (IsLastDaysCountActive)
                return (DateTime.Now.Date - dateTime.Date).Days < LastDaysCount;
            return true;
        }
    }
}