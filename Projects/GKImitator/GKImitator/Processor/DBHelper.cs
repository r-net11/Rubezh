using System;
using System.IO;
using System.Runtime.Serialization;
using Common;
using Infrastructure.Common;

namespace GKImitator.Processor
{
	public static class DBHelper
	{
		static string FileName = AppDataFolderHelper.GetServerAppDataPath("GKImitatorJournal.xml");
		public static ImitatorJournalItemCollection ImitatorJournalItemCollection { get; set; }

		static DBHelper()
		{
			Load();
		}

		public static void Load()
		{
			try
			{
				ImitatorJournalItemCollection = new ImitatorJournalItemCollection();
				if (File.Exists(FileName))
				{
					using (var fileStream = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
					{
						var dataContractSerializer = new DataContractSerializer(typeof(ImitatorJournalItemCollection));
						ImitatorJournalItemCollection = (ImitatorJournalItemCollection)dataContractSerializer.ReadObject(fileStream);
					}
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "JournalHelper.Load");
			}
		}

		public static void Save()
		{
			try
			{
				var dataContractSerializer = new DataContractSerializer(typeof(ImitatorJournalItemCollection));
				using (var fileStream = new FileStream(FileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
				{
					dataContractSerializer.WriteObject(fileStream, ImitatorJournalItemCollection);
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "JournalHelper.Save");
			}
		}
	}
}