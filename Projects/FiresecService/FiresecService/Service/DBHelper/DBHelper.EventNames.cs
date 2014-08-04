using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Common;
using FiresecAPI;
using FiresecAPI.Journal;

namespace FiresecService
{
	public static partial class DBHelper
	{
		public static OperationResult<List<JournalEventNameType>> GetDistinctEventNames()
		{
			try
			{
				var result = new List<JournalEventNameType>();
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
				return new OperationResult<List<JournalEventNameType>> { Result = result };
			}
			catch (Exception e)
			{
				Logger.Error(e, "FiresecService.GetDistinctGKJournalNames");
				return new OperationResult<List<JournalEventNameType>>(e.Message);
			}
		}

		public static OperationResult<List<JournalEventDescriptionType>> GetDistinctEventDescriptions()
		{
			
			try
			{
				var result = new List<JournalEventDescriptionType>();
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
								result.Add((JournalEventDescriptionType)reader.GetValue(0));
						}
					}
				}
				return new OperationResult<List<JournalEventDescriptionType>> { Result = result };
			}
			catch (Exception e)
			{
				Logger.Error(e, "FiresecService.GetDistinctGKJournalNames");
				return new OperationResult<List<JournalEventDescriptionType>>(e.Message);
			}
		}

		public static void UpdateNamesDescriptions(List<JournalItem> journalItems)
		{
			try
			{
				var commands = new List<string>();
				var eventNames = GetDistinctEventNames().Result;
				var eventDescription = GetDistinctEventDescriptions().Result;
				foreach (var item in journalItems)
				{
					var name = item.JournalEventNameType;
					if (!eventNames.Any(x => name == x))
					{
						eventNames.Add(name);
						commands.Add(@"Insert Into EventNames (EventName) Values ('" + (int)name + "')");
					}

					var description = item.JournalEventDescriptionType;
					if (!eventDescription.Any(x => description == x))
					{
						eventDescription.Add(description);
						commands.Add(@"Insert Into EventDescriptions (EventDescription) Values ('" + (int)description + "')");
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