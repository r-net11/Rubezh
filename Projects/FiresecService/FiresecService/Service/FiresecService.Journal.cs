﻿using System;
using System.Collections.Generic;
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
		public static void AddGKJournalItem(XJournalItem xJournalItem)
		{
			var journalItem = new JournalItem();
			journalItem.SystemDateTime = xJournalItem.SystemDateTime;
			journalItem.DeviceDateTime = xJournalItem.DeviceDateTime;
			journalItem.JournalEventNameType = xJournalItem.JournalEventNameType;
			journalItem.NameText = xJournalItem.Name;
			journalItem.DescriptionText = xJournalItem.Description;
			journalItem.JournalSubsystemType = EventDescriptionAttributeHelper.ToSubsystem(xJournalItem.JournalEventNameType);
			journalItem.ObjectUID = xJournalItem.ObjectUID;
			journalItem.ObjectName = xJournalItem.ObjectName;
			journalItem.StateClass = xJournalItem.StateClass;

			switch (xJournalItem.JournalObjectType)
			{
				case XJournalObjectType.System:
					journalItem.JournalObjectType = JournalObjectType.None;
					break;

				case XJournalObjectType.GK:
				case XJournalObjectType.Device:
				case XJournalObjectType.Pim:
				case XJournalObjectType.GkUser:
					journalItem.JournalObjectType = JournalObjectType.GKDevice;
					break;

				case XJournalObjectType.Zone:
					journalItem.JournalObjectType = JournalObjectType.GKZone;
					break;

				case XJournalObjectType.Direction:
					journalItem.JournalObjectType = JournalObjectType.GKDirection;
					break;

				case XJournalObjectType.Delay:
					journalItem.JournalObjectType = JournalObjectType.GKDelay;
					break;

				case XJournalObjectType.PumpStation:
					journalItem.JournalObjectType = JournalObjectType.GKPumpStation;
					break;

				case XJournalObjectType.MPT:
					journalItem.JournalObjectType = JournalObjectType.GKMPT;
					break;

				case XJournalObjectType.GuardZone:
					journalItem.JournalObjectType = JournalObjectType.GKGuardZone;
					break;
			}

			AddJournalItem(journalItem);
		}

		void AddSKDJournalMessage(JournalEventNameType journalEventNameType)
		{
			var journalItem = new JournalItem()
			{
				SystemDateTime = DateTime.Now,
				DeviceDateTime = DateTime.Now,
				JournalEventNameType = journalEventNameType,
				JournalEventDescriptionType = JournalEventDescriptionType.NULL,
				JournalSubsystemType = EventDescriptionAttributeHelper.ToSubsystem(journalEventNameType),
				JournalObjectType = JournalObjectType.None,
				ObjectUID = Guid.Empty,
				ObjectName = null,
				StateClass = EventDescriptionAttributeHelper.ToStateClass(journalEventNameType),
				UserName = UserName,
			};

			AddJournalItem(journalItem);
		}

		void AddSKDJournalMessage(JournalEventNameType journalEventNameType, SKDDevice device)
		{
			var journalItem = new JournalItem()
			{
				SystemDateTime = DateTime.Now,
				DeviceDateTime = DateTime.Now,
				JournalEventNameType = journalEventNameType,
				JournalEventDescriptionType = JournalEventDescriptionType.NULL,
				JournalSubsystemType = EventDescriptionAttributeHelper.ToSubsystem(journalEventNameType),
				JournalObjectType = device != null ? JournalObjectType.SKDDevice : JournalObjectType.None,
				ObjectUID = device != null ? device.UID : Guid.Empty,
				ObjectName = device != null ? device.Name : null,
				StateClass = EventDescriptionAttributeHelper.ToStateClass(journalEventNameType),
				UserName = UserName,
			};

			AddJournalItem(journalItem);
		}

		void AddSKDJournalMessage(JournalEventNameType journalEventNameType, SKDZone zone)
		{
			var journalItem = new JournalItem()
			{
				SystemDateTime = DateTime.Now,
				DeviceDateTime = DateTime.Now,
				JournalEventNameType = journalEventNameType,
				JournalEventDescriptionType = JournalEventDescriptionType.NULL,
				JournalSubsystemType = EventDescriptionAttributeHelper.ToSubsystem(journalEventNameType),
				JournalObjectType = zone != null ? JournalObjectType.SKDZone : JournalObjectType.None,
				ObjectUID = zone != null ? zone.UID : Guid.Empty,
				ObjectName = zone != null ? zone.Name : null,
				StateClass = EventDescriptionAttributeHelper.ToStateClass(journalEventNameType),
				UserName = UserName,
			};

			AddJournalItem(journalItem);
		}

		void AddSKDJournalMessage(JournalEventNameType journalEventNameType, SKDDoor door)
		{
			var journalItem = new JournalItem()
			{
				SystemDateTime = DateTime.Now,
				DeviceDateTime = DateTime.Now,
				JournalEventNameType = journalEventNameType,
				JournalEventDescriptionType = JournalEventDescriptionType.NULL,
				JournalSubsystemType = EventDescriptionAttributeHelper.ToSubsystem(journalEventNameType),
				JournalObjectType = door != null ? JournalObjectType.SKDDoor : JournalObjectType.None,
				ObjectUID = door != null ? door.UID : Guid.Empty,
				ObjectName = door != null ? door.Name : null,
				StateClass = EventDescriptionAttributeHelper.ToStateClass(journalEventNameType),
				UserName = UserName,
			};

			AddJournalItem(journalItem);
		}

		public static void AddJournalItem(JournalItem journalItem)
		{
			DBHelper.Add(journalItem);
			FiresecService.NotifyNewJournalItems(new List<JournalItem>() { journalItem });
		}
		#endregion

		#region Get
		public OperationResult<DateTime> GetMinJournalDateTime()
		{
			using (var dataContext = new SqlConnection("Data Source=.\\SQLEXPRESS;Initial Catalog=SKD;Integrated Security=True;Language='English'"))
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

		public OperationResult<List<JournalItem>> GetFilteredJournalItems(JournalFilter filter)
		{
			var journalItems = DBHelper.GetFilteredJournalItems(filter);
			return new OperationResult<List<JournalItem>>() { Result = journalItems };
		}

		public void BeginGetFilteredArchive(ArchiveFilter archiveFilter, Guid archivePortionUID)
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
				DBHelper.BeginGetFilteredArchive(archiveFilter, archivePortionUID, false);

			}))));
			thread.Name = "FiresecService.GetFilteredArchive";
			thread.IsBackground = true;
			CurrentThread = thread;
			thread.Start();
		}

		void DatabaseHelper_ArchivePortionReady(List<JournalItem> journalItems, Guid archivePortionUID)
		{
			FiresecService.NotifySKDArchiveCompleted(journalItems, archivePortionUID);
		}

		public List<JournalEventDescriptionType> GetDistinctEventDescriptions()
		{
			return DBHelper.GetDistinctEventDescriptions();
		}

		public List<JournalEventNameType> GetDistinctEventNames()
		{
			return DBHelper.GetDistinctEventNames();
		}
		#endregion
	}
}