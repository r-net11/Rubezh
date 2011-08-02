using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace FiresecAPI.Models
{
    [Serializable]
    public class JournalFilter
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string LastRecordsCount { get; set; }

        [XmlAttribute]
        public string LastDaysCount { get; set; }

        public List<State> Events { get; set; }

        public List<DeviceCategory> Categories { get; set; }

        public JournalFilter Copy()
        {
            var copyJournalFilter = new JournalFilter();
            copyJournalFilter.Name = Name;
            copyJournalFilter.LastRecordsCount = LastRecordsCount;
            copyJournalFilter.LastDaysCount = LastDaysCount;

            copyJournalFilter.Events = new List<State>();
            foreach (var state in Events)
            {
                copyJournalFilter.Events.Add(new State() { Id = state.Id });
            }

            copyJournalFilter.Categories = new List<DeviceCategory>();
            foreach (var category in Categories)
            {
                copyJournalFilter.Categories.Add(new DeviceCategory() { Id = category.Id });
            }

            return copyJournalFilter;
        }
    }
}
