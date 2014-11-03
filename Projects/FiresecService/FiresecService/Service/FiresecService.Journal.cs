using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecAPI.Journal;
using FiresecAPI.SKD;

namespace FiresecService.Service
{
	public partial class FiresecService
	{
		public static Thread CurrentThread;

		#region Add
		public static void AddGKJournalItem(JournalItem journalItem)
		{
			AddCommonJournalItem(journalItem);
		}

		void AddJournalMessage(JournalEventNameType journalEventNameType, string objectName, JournalEventDescriptionType journalEventDescriptionType = JournalEventDescriptionType.NULL)
		{
			var journalItem = new JournalItem()
			{
				SystemDateTime = DateTime.Now,
				JournalEventNameType = journalEventNameType,
				JournalEventDescriptionType = JournalEventDescriptionType.NULL,
				JournalSubsystemType = EventDescriptionAttributeHelper.ToSubsystem(journalEventNameType),
				JournalObjectType = JournalObjectType.None,
				ObjectUID = Guid.Empty,
				ObjectName = objectName,
				UserName = UserName,
			};

			AddCommonJournalItem(journalItem);
		}
		
		//void AddJournalMessage(JournalEventNameType journalEventNameType, JournalEventDescriptionType journalEventDescriptionType = JournalEventDescriptionType.NULL)
		//{
		//    AddJournalMessage(journalEventNameType, null, journalEventDescriptionType);
		//}

		void AddSKDJournalMessage(JournalEventNameType journalEventNameType, SKDDevice device)
		{
			var journalItem = new JournalItem()
			{
				SystemDateTime = DateTime.Now,
				JournalEventNameType = journalEventNameType,
				JournalEventDescriptionType = JournalEventDescriptionType.NULL,
				JournalSubsystemType = EventDescriptionAttributeHelper.ToSubsystem(journalEventNameType),
				JournalObjectType = device != null ? JournalObjectType.SKDDevice : JournalObjectType.None,
				ObjectUID = device != null ? device.UID : Guid.Empty,
				ObjectName = device != null ? device.Name : null,
				UserName = UserName,
			};

			AddCommonJournalItem(journalItem);
		}

		void AddSKDJournalMessage(JournalEventNameType journalEventNameType, SKDZone zone)
		{
			var journalItem = new JournalItem()
			{
				SystemDateTime = DateTime.Now,
				JournalEventNameType = journalEventNameType,
				JournalEventDescriptionType = JournalEventDescriptionType.NULL,
				JournalSubsystemType = EventDescriptionAttributeHelper.ToSubsystem(journalEventNameType),
				JournalObjectType = zone != null ? JournalObjectType.SKDZone : JournalObjectType.None,
				ObjectUID = zone != null ? zone.UID : Guid.Empty,
				ObjectName = zone != null ? zone.Name : null,
				UserName = UserName,
			};

			AddCommonJournalItem(journalItem);
		}

		void AddSKDJournalMessage(JournalEventNameType journalEventNameType, SKDDoor door)
		{
			var journalItem = new JournalItem()
			{
				SystemDateTime = DateTime.Now,
				JournalEventNameType = journalEventNameType,
				JournalEventDescriptionType = JournalEventDescriptionType.NULL,
				JournalSubsystemType = EventDescriptionAttributeHelper.ToSubsystem(journalEventNameType),
				JournalObjectType = door != null ? JournalObjectType.SKDDoor : JournalObjectType.None,
				ObjectUID = door != null ? door.UID : Guid.Empty,
				ObjectName = door != null ? door.Name : null,
				UserName = UserName,
			};

			AddCommonJournalItem(journalItem);
		}

		public static void AddCommonJournalItem(JournalItem journalItem)
		{
			DBHelper.Add(journalItem);
			FiresecService.NotifyNewJournalItems(new List<JournalItem>() { journalItem });
			ProcedureRunner.RunOnJournal(journalItem);
		}

		public OperationResult<bool> AddJournalItem(JournalItem journalItem)
		{
			try
			{
				journalItem.UserName = UserName;
				journalItem.JournalSubsystemType = EventDescriptionAttributeHelper.ToSubsystem(journalItem.JournalEventNameType);
				DBHelper.Add(journalItem);
				FiresecService.NotifyNewJournalItems(new List<JournalItem>() { journalItem });
			}
			catch (Exception e)
			{
				return new OperationResult<bool>(e.Message);
			}
			return new OperationResult<bool>() { Result = true };
		}
		#endregion

		#region Get
		public OperationResult<DateTime> GetMinJournalDateTime()
		{
			try
			{
				using (var dataContext = new SqlConnection(PatchManager.JournalConnectionString))
				{
					var query = "SELECT MIN(SystemDate) FROM Journal";
					var sqlCeCommand = new SqlCommand(query, dataContext);
					dataContext.Open();
					var reader = sqlCeCommand.ExecuteReader();
					var result = DateTime.Now;
					if (reader.Read())
					{
						if (!reader.IsDBNull(0))
						{
							result = reader.GetDateTime(0);
						}
					}
					dataContext.Close();
					return new OperationResult<DateTime>() { Result = result };
				}
			}
			catch (Exception e)
			{
				return new OperationResult<DateTime>(e.Message);
			}
		}

		public OperationResult<List<JournalItem>> GetFilteredJournalItems(JournalFilter filter)
		{
			return DBHelper.GetFilteredJournalItems(filter);
		}

		public OperationResult BeginGetFilteredArchive(ArchiveFilter archiveFilter, Guid archivePortionUID)
		{
			try
			{
				if (CurrentThread != null)
				{
					DBHelper.IsAbort = true;
					CurrentThread.Join(TimeSpan.FromMinutes(1));
					CurrentThread = null;
				}
				DBHelper.IsAbort = false;
				var thread = new Thread(new ThreadStart((new Action(() =>
				{
					DBHelper.ArchivePortionReady -= DatabaseHelper_ArchivePortionReady;
					DBHelper.ArchivePortionReady += DatabaseHelper_ArchivePortionReady;
					DBHelper.BeginGetFilteredArchive(archiveFilter, archivePortionUID);

				}))));
				thread.Name = "FiresecService.GetFilteredArchive";
				thread.IsBackground = true;
				CurrentThread = thread;
				thread.Start();
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		void DatabaseHelper_ArchivePortionReady(List<JournalItem> journalItems, Guid archivePortionUID)
		{
			FiresecService.NotifyArchiveCompleted(journalItems, archivePortionUID);
		}
		#endregion
	}
}