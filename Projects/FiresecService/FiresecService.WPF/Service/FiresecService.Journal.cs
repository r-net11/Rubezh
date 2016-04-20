using Infrastructure.Automation;
using RubezhAPI;
using RubezhAPI.Journal;
using RubezhDAL.DataClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace FiresecService.Service
{
	public partial class FiresecService
	{
		public static Thread CurrentThread;

		#region Add
		public static void AddJournalMessage(JournalEventNameType journalEventNameType, string objectName, Guid? objectUID, Guid? clientUID, JournalEventDescriptionType journalEventDescriptionType = JournalEventDescriptionType.NULL)
		{
			var journalItem = new JournalItem()
			{
				SystemDateTime = DateTime.Now,
				JournalEventNameType = journalEventNameType,
				JournalEventDescriptionType = journalEventDescriptionType,
				JournalSubsystemType = EventDescriptionAttributeHelper.ToSubsystem(journalEventNameType),
				JournalObjectType = JournalObjectType.None,
				ObjectUID = objectUID != null ? objectUID.Value : Guid.Empty,
				ObjectName = objectName,
				UserName = GetUserName(clientUID)
			};
			AddCommonJournalItems(new List<JournalItem>() { journalItem }, clientUID);
		}

		public static void AddCommonJournalItems(List<JournalItem> journalItems, Guid? clientUID)
		{
			using (var databaseService = new RubezhDAL.DataClasses.DbService())
			{
				databaseService.JournalTranslator.AddRange(journalItems);
			}
			FiresecService.NotifyJournalItems(journalItems, true);
			var user = clientUID.HasValue ? ConfigurationCashHelper.SecurityConfiguration.Users.FirstOrDefault(x => x.Login == GetLogin(clientUID.Value)) : null;
			foreach (var journalItem in journalItems)
			{
				AutomationProcessor.RunOnJournal(journalItem, user, clientUID ?? UID);
			}
		}

		public OperationResult<bool> AddJournalItem(Guid clientUID, JournalItem journalItem)
		{
			try
			{
				journalItem.UserName = GetUserName(clientUID);
				journalItem.JournalSubsystemType = EventDescriptionAttributeHelper.ToSubsystem(journalItem.JournalEventNameType);
				AddCommonJournalItems(new List<JournalItem>() { journalItem }, clientUID);
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

		public OperationResult<bool> BeginGetArchivePage(JournalFilter filter, int page, Guid clientUid, string userName)
		{
			ServerTaskRunner.Add(null, "Чтение архива", () =>
			{
				using (var dbService = new RubezhDAL.DataClasses.DbService())
				{
					var result = dbService.JournalTranslator.GetArchivePage(filter, page);
					FiresecService.NotifyOperationResult_GetArchivePage(result, clientUid, userName);
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