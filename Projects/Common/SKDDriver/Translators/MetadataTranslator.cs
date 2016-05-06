using StrazhDAL.DataAccess;
using System;
using System.Linq;

namespace StrazhDAL
{
	public class MetadataTranslator
	{
		private DataAccess.SKDDataContext Context;

		public MetadataTranslator(SKDDatabaseService databaseService)
		{
			Context = databaseService.Context;
		}

		public int GetJournalNo()
		{
			if (Context.JournalMetadatas.Count() == 0)
				return 0;

			return Context.JournalMetadatas.Max(x => x.No);
		}

		public void AddJournalMetadata(int no, DateTime startDateTime, DateTime endDateTime)
		{
			var journalMetadata = new JournalMetadata();
			journalMetadata.UID = Guid.NewGuid();
			journalMetadata.No = no;
			journalMetadata.StartDateTime = startDateTime;
			journalMetadata.EndDateTime = endDateTime;
			Context.JournalMetadatas.InsertOnSubmit(journalMetadata);
			Context.SubmitChanges();
		}

		public int GetPassJournalNo()
		{
			if (Context.PassJournalMetadatas.Count() == 0)
				return 0;

			return Context.PassJournalMetadatas.Max(x => x.No);
		}

		public void AddPassJournalMetadata(int no, DateTime startDateTime, DateTime endDateTime)
		{
			var passJournalMetadata = new PassJournalMetadata();
			passJournalMetadata.UID = Guid.NewGuid();
			passJournalMetadata.No = no;
			passJournalMetadata.StartDateTime = startDateTime;
			passJournalMetadata.EndDateTime = endDateTime;
			Context.PassJournalMetadatas.InsertOnSubmit(passJournalMetadata);
			Context.SubmitChanges();
		}
	}
}