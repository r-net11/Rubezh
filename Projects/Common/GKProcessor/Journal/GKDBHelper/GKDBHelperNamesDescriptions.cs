using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.IO;
using System.Linq;
using System.Threading;
using Common;
using FiresecAPI;
using Infrastructure.Common;
using XFiresecAPI;

namespace GKProcessor
{
	public static partial class GKDBHelper
	{
		static List<string> eventNames;
		public static List<string> EventNames
		{
			get
			{
				if (eventNames == null)
					eventNames = GetDistinctGKJournalNames();
				return eventNames;
			}
			set { eventNames = value; }
		}

		static List<string> eventDescriptions;
		public static List<string> EventDescriptions
		{
			get
			{
				if (eventDescriptions == null)
					eventDescriptions = GetDistinctGKJournalDescriptions();
				return eventDescriptions;
			}
			set { eventDescriptions = value; }
		}

		static List<string> GetDistinctGKJournalDescriptions()
		{
			try
			{
				var result = new List<string>();
				if (!File.Exists(AppDataFolderHelper.GetDBFile("GkJournalDatabase.sdf")))
					return result;
				var connection = new SqlCeConnection(ConnectionString);
				var sqlCeCommand = new SqlCeCommand(@"SELECT EventDescription FROM EventDescriptions", connection);
				connection.Open();
				var reader = sqlCeCommand.ExecuteReader();
				while (reader.Read())
				{
					if (!reader.IsDBNull(0))
						result.Add(reader.GetString(0));
				}
				connection.Close();
				connection.Dispose();
				return result;
			}
			catch
			{
				return new List<string>();
			}
		}

		static List<string> GetDistinctGKJournalNames()
		{
			try
			{
				var result = new List<string>();
				if (!File.Exists(AppDataFolderHelper.GetDBFile("GkJournalDatabase.sdf")))
					return result;
				var connection = new SqlCeConnection(ConnectionString);
				var sqlCeCommand = new SqlCeCommand(@"SELECT EventName FROM EventNames", connection);
				connection.Open();
				var reader = sqlCeCommand.ExecuteReader();
				while (reader.Read())
				{
					if (!reader.IsDBNull(0))
						result.Add(reader.GetString(0));
				}
				connection.Close();
				connection.Dispose();
				return result;
			}
			catch
			{
				return new List<string>();
			}
		}

		static void UpdateNamesDescriptions(List<JournalItem> journalItems)
		{
			var commands = new List<string>();
			foreach (var item in journalItems)
			{
				var name = item.Name;
				if (!EventNames.Any(x => name == x))
				{
					EventNames.Add(name);
					commands.Add(@"Insert Into EventNames (EventName) Values ('" + name + "')");
				}

				var description = item.Description;
				if (!EventDescriptions.Any(x => description == x))
				{
					EventDescriptions.Add(description);
					commands.Add(@"Insert Into EventDescriptions (EventDescription) Values ('" + description + "')");
				}
			}
			ExecuteNonQuery(commands);
		}
	}
}