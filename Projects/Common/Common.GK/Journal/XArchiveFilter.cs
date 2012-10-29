using System;
using System.Collections.Generic;
using XFiresecAPI;

namespace Common.GK
{
    public class XArchiveFilter
    {
        public XArchiveFilter()
        {
            StartDate = DateTime.Now.AddDays(-1);
            EndDate = DateTime.Now;
            JournalItemTypes = new List<JournalItemType>();
            StateClasses = new List<XStateClass>();
            GKAddresses = new List<string>();
			EventNames = new List<string>();
        }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<JournalItemType> JournalItemTypes { get; set; }
        public List<XStateClass> StateClasses { get; set; }
        public List<string> GKAddresses { get; set; }
		public List<string> EventNames { get; set; }
    }
}