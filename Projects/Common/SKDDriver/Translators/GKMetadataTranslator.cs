using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SKDDriver.DataAccess;

namespace SKDDriver
{
	public class GKMetadataTranslator
	{
		DataAccess.SKDDataContext Context;

		public GKMetadataTranslator(SKDDatabaseService databaseService)
		{
			Context = databaseService.Context;
		}

		public int GetLastJournalNo(string gkIPAddress)
		{
			if (string.IsNullOrEmpty(gkIPAddress))
			{
				return -1;
			}
			var gkMetadata = Context.GKMetadatas.FirstOrDefault(x => x.IPAddress == gkIPAddress);
			if (gkMetadata != null)
			{
				return gkMetadata.LastJournalNo;
			}
			return -1;
		}

		public void SetLastJournalNo(string gkIPAddress, int lastJournalNo)
		{
			if (string.IsNullOrEmpty(gkIPAddress))
			{
				return;
			}
			var gkMetadata = Context.GKMetadatas.FirstOrDefault(x => x.IPAddress == gkIPAddress);
			if (gkMetadata != null)
			{
				gkMetadata.LastJournalNo = lastJournalNo;
				Context.SubmitChanges();
			}
			else
			{
				gkMetadata = new GKMetadata()
				{
					UID = Guid.NewGuid(),
					IPAddress = gkIPAddress,
					SerialNo = "",
					LastJournalNo = lastJournalNo
				};
				Context.GKMetadatas.InsertOnSubmit(gkMetadata);
				Context.SubmitChanges();
			}
		}
	}
}