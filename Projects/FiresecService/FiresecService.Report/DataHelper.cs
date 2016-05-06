using StrazhDAL;
using System;
using System.Data.SqlClient;

namespace FiresecService.Report
{
	internal static class DataHelper
	{
		public static Guid GetDefaultOrganisation()
		{
			using (var connection = new SqlConnection(SKDDatabaseService.SkdConnectionString))
			{
				var command = new SqlCommand("SELECT TOP 1 UID FROM Organisation", connection);
				connection.Open();
				var uid = command.ExecuteScalar();
				return uid == null ? Guid.Empty : (Guid)uid;
			}
		}
	}
}