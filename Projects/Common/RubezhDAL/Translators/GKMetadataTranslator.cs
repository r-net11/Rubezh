using System;
using System.Linq;

namespace RubezhDAL.DataClasses
{
	public class GKMetadataTranslator
	{
		DatabaseContext Context;

		public GKMetadataTranslator(DbService databaseService)
		{
			Context = databaseService.Context;
		}

		public int GetLastJournalNo(string gkIPAddress)
		{
			if (string.IsNullOrEmpty(gkIPAddress))
			{
				return -1;
			}
			var gkMetadata = Context.GKMetadatas.FirstOrDefault(x => x.IpAddress == gkIPAddress);
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
			var gkMetadata = Context.GKMetadatas.FirstOrDefault(x => x.IpAddress == gkIPAddress);
			if (gkMetadata != null)
			{
				gkMetadata.LastJournalNo = lastJournalNo;
				Context.SaveChanges();
			}
			else
			{
				gkMetadata = new GKMetadata()
				{
					UID = Guid.NewGuid(),
					IpAddress = gkIPAddress,
					SerialNo = "",
					LastJournalNo = lastJournalNo
				};
				Context.GKMetadatas.Add(gkMetadata);
				Context.SaveChanges();
			}
		}
	}
}
