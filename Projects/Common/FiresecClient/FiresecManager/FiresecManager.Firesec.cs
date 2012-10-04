using Firesec;

namespace FiresecClient
{
	public partial class FiresecManager
	{
		public static FiresecDriver FiresecDriver { get; private set; }
		static int lastJournalNo;

		public static void InitializeFiresecDriver(string FS_Address, int FS_Port, string FS_Login, string FS_Password)
		{
			lastJournalNo = FiresecService.FiresecService.GetJournalLastId().Result;
			FiresecDriver = new FiresecDriver(lastJournalNo, FS_Address, FS_Port, FS_Login, FS_Password);
		}

		public static void SynchrinizeJournal()
		{
			var journalRecords = FiresecDriver.Watcher.SynchrinizeJournal(lastJournalNo);
			if (journalRecords.Count > 0)
			{
				FiresecService.AddJournalRecords(journalRecords);
			}
		}
	}
}