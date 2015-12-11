using Infrastructure.Automation;
using RubezhAPI;
using RubezhAPI.Journal;
using RubezhDAL.DataClasses;
using System;
using System.Collections.Generic;
using System.Threading;

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
			FiresecService.NotifyJournalItems(journalItems, true);
			foreach (var journalItem in journalItems)
			{
				AutomationProcessor.RunOnJournal(journalItem);
			}
		}

		public OperationResult<bool> AddJournalItem(Guid clientUID, JournalItem journalItem)
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
		public OperationResult<DateTime> GetMinJournalDateTime(Guid clientUID)
		{
			using (var dbService = new DbService())
			{
				return dbService.JournalTranslator.GetMinDate();
			}
		}

		public OperationResult<List<JournalItem>> GetFilteredJournalItems(Guid clientUID, JournalFilter filter)
		{
			using (var dbService = new RubezhDAL.DataClasses.DbService())
			{
				return dbService.JournalTranslator.GetFilteredJournalItems(filter);
			}
		}

		public OperationResult<bool> BeginGetJournal(JournalFilter filter, Guid clientUid, Guid journalClientUid)
		{
			ServerTaskRunner.Add(null, "Чтение журнала событий", () =>
			{
				using (var dbService = new RubezhDAL.DataClasses.DbService())
				{
					var result = dbService.JournalTranslator.GetFilteredJournalItems(filter);
					FiresecService.NotifyOperationResult_GetJournal(result, clientUid, journalClientUid);
				}
			});
			return new OperationResult<bool>(true);
		}

		public OperationResult<bool> BeginGetArchivePage(JournalFilter filter, int page, Guid clientUid)
		{
			ServerTaskRunner.Add(null, "Чтение архива", () =>
			{
				using (var dbService = new RubezhDAL.DataClasses.DbService())
				{
					var result = dbService.JournalTranslator.GetArchivePage(filter, page);
					FiresecService.NotifyOperationResult_GetArchivePage(result, clientUid);
				}
			});
			return new OperationResult<bool>(true);
		}

		public OperationResult<int> GetArchiveCount(Guid clientUID, JournalFilter filter)
		{
			using (var dbService = new RubezhDAL.DataClasses.DbService())
			{
				return dbService.JournalTranslator.GetArchiveCount(filter);
			}
		}
		#endregion
	}
}