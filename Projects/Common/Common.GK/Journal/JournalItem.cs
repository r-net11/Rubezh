using System;
using System.Collections.Generic;
using System.Linq;
using FiresecClient;
using XFiresecAPI;
using Common.GK.DB;

namespace Common.GK
{
    public class JournalItem
    {
        public JournalItemType JournalItemType { get; set; }
        public DateTime DateTime { get; set; }
        public Guid ObjectUID { get; set; }
        public string Name { get; set; }
        public string YesNo { get; set; }
        public string Description { get; set; }
        public int ObjectState { get; set; }
        public ushort GKObjectNo { get; set; }
		public string GKIpAddress { get; set; }
		public int? GKJournalRecordNo { get; set; }
        public XStateClass StateClass { get; set; }

		public static JournalItem FromJournal(Journal journal)
		{
			var journalItem = new JournalItem()
			{
				GKIpAddress = journal.GKIpAddress,
				GKJournalRecordNo = journal.GKJournalRecordNo,
				Name = journal.Name,
				YesNo = journal.YesNo,
				Description = journal.Description,
			};
			if (journal.DateTime.HasValue)
				journalItem.DateTime = journal.DateTime.Value;
			if (journal.ObjectUID.HasValue)
				journalItem.ObjectUID = journal.ObjectUID.Value;
			if (journal.ObjectState.HasValue)
				journalItem.ObjectState = journal.ObjectState.Value;
			if (journal.JournalItemType.HasValue)
			{
				journalItem.JournalItemType = (JournalItemType)journal.JournalItemType.Value;
			}
            journalItem.StateClass = (XStateClass)journal.StateClass;

			return journalItem;
		}

        public Journal ToJournal(JournalItem journalItem)
        {
            var journal = new Journal();
            journal.JournalItemType = (byte)JournalItemType;
            journal.DateTime = DateTime;
            journal.ObjectUID = ObjectUID;
            journal.Name = Name;
            journal.YesNo = YesNo;
            journal.Description = Description;
            journal.ObjectState = ObjectState;
            return journal;
        }
    }
}