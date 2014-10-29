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
	}
}