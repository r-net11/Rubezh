using System;
using XFiresecAPI;

namespace Common.GK
{
    public class JournalItem
    {
        public JournalItemType JournalItemType { get; set; }
        public DateTime DateTime { get; set; }
        public Guid ObjectUID { get; set; }
        public string Name { get; set; }
		public JournalYesNoType YesNo { get; set; }
        public string Description { get; set; }
        public int ObjectState { get; set; }
        public ushort GKObjectNo { get; set; }
		public string GKIpAddress { get; set; }
		public int? GKJournalRecordNo { get; set; }
        public XStateClass StateClass { get; set; }
		public string UserName { get; set; }

		public InternalJournalItem InternalJournalItem { get; set; }
    }
}