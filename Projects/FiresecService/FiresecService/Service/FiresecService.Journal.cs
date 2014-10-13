using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecAPI.Journal;
using FiresecAPI.SKD;
using FiresecService.Processor;

namespace FiresecService.Service
{
	public partial class FiresecService
	{
		public static Thread CurrentThread;

		#region Add
		public static void AddGKJournalItem(GKJournalItem gkJournalItem)
		{
			var journalItem = new JournalItem();
			journalItem.SystemDateTime = gkJournalItem.SystemDateTime;
			journalItem.DeviceDateTime = gkJournalItem.DeviceDateTime;
			journalItem.JournalEventNameType = gkJournalItem.JournalEventNameType;
			journalItem.JournalEventDescriptionType = gkJournalItem.JournalEventDescriptionType;
			journalItem.JournalSubsystemType = EventDescriptionAttributeHelper.ToSubsystem(gkJournalItem.JournalEventNameType);
			journalItem.ObjectUID = gkJournalItem.ObjectUID;
			journalItem.ObjectName = gkJournalItem.ObjectName;
			journalItem.UserName = gkJournalItem.UserName;

			journalItem.NameText = gkJournalItem.Name;
			journalItem.DescriptionText = gkJournalItem.Description;
			if (!string.IsNullOrEmpty(gkJournalItem.AdditionalDescription))
				journalItem.DescriptionText = gkJournalItem.AdditionalDescription;

			if (gkJournalItem.GKJournalRecordNo.HasValue && gkJournalItem.GKJournalRecordNo.Value > 0)
				journalItem.JournalDetalisationItems.Add(new JournalDetalisationItem("Запись ГК", gkJournalItem.GKJournalRecordNo.Value.ToString()));
			if (gkJournalItem.GKObjectNo > 0)
				journalItem.JournalDetalisationItems.Add(new JournalDetalisationItem("Компонент ГК", gkJournalItem.GKObjectNo.ToString()));
			if (!string.IsNullOrEmpty(gkJournalItem.GKIpAddress))
				journalItem.JournalDetalisationItems.Add(new JournalDetalisationItem("IP-адрес ГК", gkJournalItem.GKIpAddress.ToString()));

			switch (gkJournalItem.JournalObjectType)
			{
				case GKJournalObjectType.System:
					journalItem.JournalObjectType = JournalObjectType.None;
					break;

				case GKJournalObjectType.GK:
				case GKJournalObjectType.Device:
				case GKJournalObjectType.Pim:
				case GKJournalObjectType.GkUser:
					journalItem.JournalObjectType = JournalObjectType.GKDevice;
					break;

				case GKJournalObjectType.Zone:
					journalItem.JournalObjectType = JournalObjectType.GKZone;
					break;

				case GKJournalObjectType.Direction:
					journalItem.JournalObjectType = JournalObjectType.GKDirection;
					break;

				case GKJournalObjectType.Delay:
					journalItem.JournalObjectType = JournalObjectType.GKDelay;
					break;

				case GKJournalObjectType.PumpStation:
					journalItem.JournalObjectType = JournalObjectType.GKPumpStation;
					break;

				case GKJournalObjectType.MPT:
					journalItem.JournalObjectType = JournalObjectType.GKMPT;
					break;

				case GKJournalObjectType.GuardZone:
					journalItem.JournalObjectType = JournalObjectType.GKGuardZone;
					break;

				case GKJournalObjectType.Door:
					journalItem.JournalObjectType = JournalObjectType.GKDoor;
					break;
			}

			AddCommonJournalItem(journalItem);
		}

		void AddJournalMessage(JournalEventNameType journalEventNameType, JournalEventDescriptionType journalEventDescriptionType = JournalEventDescriptionType.NULL)
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
				UserName = UserName,
			};

			AddCommonJournalItem(journalItem);
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
				UserName = UserName,
			};

			AddCommonJournalItem(journalItem);
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
				UserName = UserName,
			};

			AddCommonJournalItem(journalItem);
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
				UserName = UserName,
			};

			AddCommonJournalItem(journalItem);
		}

		public static void AddCommonJournalItem(JournalItem journalItem)
		{
			DBHelper.Add(journalItem);
			FiresecService.NotifyNewJournalItems(new List<JournalItem>() { journalItem });
			AutomationProcessorRunner.RunOnJournal(journalItem);
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
				using (var dataContext = new SqlConnection(ConfigurationManager.ConnectionStrings["SKDDriver.Properties.Settings.SKDConnectionString"].ConnectionString))
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
					DBHelper.BeginGetFilteredArchive(archiveFilter, archivePortionUID, false);

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

		public OperationResult<List<JournalEventDescriptionType>> GetDistinctEventDescriptions()
		{
			return DBHelper.GetDistinctEventDescriptions();
		}

		public OperationResult<List<JournalEventNameType>> GetDistinctEventNames()
		{
			return DBHelper.GetDistinctEventNames();
		}
		#endregion
	}
}