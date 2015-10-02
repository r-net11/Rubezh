using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using FiresecAPI;
using FiresecAPI.Journal;
using FiresecAPI.SKD;
using SKDDriver.DataClasses;
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
			var journalItem = new JournalItem()
			{
				SystemDateTime = DateTime.Now,
				JournalEventNameType = journalEventNameType,
				JournalEventDescriptionType = journalEventDescriptionType,
				JournalSubsystemType = EventDescriptionAttributeHelper.ToSubsystem(journalEventNameType),
				JournalObjectType = JournalObjectType.None,
				ObjectUID = uid != null ? uid.Value : Guid.Empty,
				ObjectName = objectName,
			};
			if (userName != null)
			{
				journalItem.UserName = userName;
			}
			else
			{
				journalItem.UserName = UserName;
			}

			AddCommonJournalItems(new List<JournalItem>() { journalItem });
		}

		public static void AddCommonJournalItems(List<JournalItem> journalItems)
		{
			using (var dbService = new SKDDriver.DataClasses.DbService())
			{
				dbService.JournalTranslator.AddRange(journalItems);
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
			using (var dbService = new SKDDriver.DataClasses.DbService())
			{
				return dbService.JournalTranslator.GetFilteredJournalItems(filter);
			}
		}

		public OperationResult<List<JournalItem>> GetArchivePage(ArchiveFilter filter, int page)
		{
			using(var dbService = new SKDDriver.DataClasses.DbService())
			{
				return dbService.JournalTranslator.GetArchivePage(filter, page);
			}
		}

		public OperationResult<int> GetArchiveCount(ArchiveFilter filter)
		{
			using (var dbService = new SKDDriver.DataClasses.DbService())
			{
				return dbService.JournalTranslator.GetArchiveCount(filter);
			}
		}
		#endregion
	}
}