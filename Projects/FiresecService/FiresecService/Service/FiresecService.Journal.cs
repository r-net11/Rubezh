using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using RubezhAPI;
using RubezhAPI.Journal;
using RubezhAPI.SKD;
using RubezhDAL.DataClasses;
using System.Diagnostics;
using Infrastructure.Automation;

namespace FiresecService.Service
{
	public partial class FiresecService
	{
		public static Thread CurrentThread;

		#region Add
		void AddJournalMessage(JournalEventNameType journalEventNameType, string objectName, JournalEventDescriptionType journalEventDescriptionType = JournalEventDescriptionType.NULL, string userName = null, Guid? uid = null)
		{
			var journalItem = CreateJournalItem(journalEventNameType, objectName, journalEventDescriptionType, uid); 
			journalItem.UserName = userName != null ? userName : UserName;
			AddCommonJournalItems(new List<JournalItem>() { journalItem });
		}

		public static void InsertJournalMessage(JournalEventNameType journalEventNameType, string objectName, JournalEventDescriptionType journalEventDescriptionType = JournalEventDescriptionType.NULL, string userName = null, Guid? uid = null)
		{
			var journalItem = CreateJournalItem(journalEventNameType, objectName, journalEventDescriptionType, uid);
			journalItem.UserName = userName;
			AddCommonJournalItems(new List<JournalItem>() { journalItem });
		}

		static JournalItem CreateJournalItem(JournalEventNameType journalEventNameType, string objectName, JournalEventDescriptionType journalEventDescriptionType = JournalEventDescriptionType.NULL, Guid? uid = null)
		{
			return new JournalItem()
			{
				SystemDateTime = DateTime.Now,
				JournalEventNameType = journalEventNameType,
				JournalEventDescriptionType = journalEventDescriptionType,
				JournalSubsystemType = EventDescriptionAttributeHelper.ToSubsystem(journalEventNameType),
				JournalObjectType = JournalObjectType.None,
				ObjectUID = uid != null ? uid.Value : Guid.Empty,
				ObjectName = objectName,
			};
		}

		public static void AddCommonJournalItems(List<JournalItem> journalItems)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				databaseService.JournalTranslator.AddRange(journalItems);
			}
			FiresecService.NotifyNewJournalItems(journalItems);
			foreach (var journalItem in journalItems)
			{
				AutomationProcessor.RunOnJournal(journalItem);
			}
		}

		public OperationResult<bool> AddJournalItem(JournalItem journalItem)
		{
			try
			{
				journalItem.UserName = UserName;
				journalItem.JournalSubsystemType = EventDescriptionAttributeHelper.ToSubsystem(journalItem.JournalEventNameType);
				AddCommonJournalItems(new List<JournalItem>() { journalItem });
			}
			catch (Exception e)
			{
				return OperationResult<bool>.FromError(e.Message);
			}
			return new OperationResult<bool>(true);
		}
		#endregion

		#region Get
		public OperationResult<DateTime> GetMinJournalDateTime()
		{
			using (var dbService = new DbService())
			{
				return dbService.JournalTranslator.GetMinDate();
			}
		}

		public OperationResult<List<JournalItem>> GetFilteredJournalItems(JournalFilter filter)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.JournalTranslator.GetFilteredJournalItems(filter);
			}
		}

		public OperationResult<List<JournalItem>> GetArchivePage(ArchiveFilter filter, int page)
		{
			using(var dbService = new RubezhDAL.DataClasses.DbService())
			{
				return dbService.JournalTranslator.GetArchivePage(filter, page);
			}
		}

		public OperationResult<int> GetArchiveCount(ArchiveFilter filter)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				return databaseService.JournalTranslator.GetArchiveCount(filter);
			}
		}
		#endregion
	}
}