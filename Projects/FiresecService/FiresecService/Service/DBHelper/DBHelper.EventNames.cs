using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Common;
using FiresecAPI.GK;
using FiresecAPI.Journal;

namespace FiresecService
{
	public static partial class DBHelper
	{
		public static List<JournalEventNameType> GetDistinctEventNames()
		{
			var result = new List<JournalEventNameType>();
			try
			{
				lock (locker)
				{
					using (var dataContext = new SqlConnection(ConnectionString))
					{
						var sqlCommand = new SqlCommand(@"SELECT EventName FROM EventNames", dataContext);
						dataContext.Open();
						var reader = sqlCommand.ExecuteReader();
						while (reader.Read())
						{
							if (!reader.IsDBNull(0))
								result.Add((JournalEventNameType)reader.GetValue(0));
						}
					}
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "FiresecService.GetDistinctGKJournalNames");
			}
			return result;
		}

		public static List<EventDescription> GetDistinctEventDescriptions()
		{
			var result = new List<EventDescription>();
			try
			{
				lock (locker)
				{
					using (var dataContext = new SqlConnection(ConnectionString))
					{
						var sqlCommand = new SqlCommand(@"SELECT EventDescription FROM EventDescriptions", dataContext);
						dataContext.Open();
						var reader = sqlCommand.ExecuteReader();
						while (reader.Read())
						{
							if (!reader.IsDBNull(0))
								result.Add((EventDescription)reader.GetValue(0));
						}
					}
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "FiresecService.GetDistinctGKJournalNames");
			}
			return result;
		}

		public static void UpdateNamesDescriptions(List<FiresecAPI.Journal.JournalItem> journalItems)
		{
			try
			{
				var commands = new List<string>();
				var eventNames = GetDistinctEventNames();
				var eventDescription = GetDistinctEventDescriptions();
				foreach (var item in journalItems)
				{
					var name = item.JournalEventNameType;
					if (!eventNames.Any(x => name == x))
					{
						eventNames.Add(name);
						commands.Add(@"Insert Into EventNames (EventName) Values ('" + name + "')");
					}

					var description = item.Description;
					if (!eventDescription.Any(x => description == x))
					{
						eventDescription.Add(description);
						commands.Add(@"Insert Into EventDescriptions (EventDescription) Values ('" + description + "')");
					}
				}
				using (var dataContext = new SqlConnection(ConnectionString))
				{
					dataContext.Open();
					foreach (var command in commands)
					{
						var sqlCommand = new SqlCommand(command, dataContext);
						sqlCommand.ExecuteNonQuery();
					}
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "FiresecService.UpdateNamesDescriptions");
			}
		}
	}
}
