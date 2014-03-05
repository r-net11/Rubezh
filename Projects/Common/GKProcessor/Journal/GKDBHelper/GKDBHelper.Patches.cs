using System.Collections.Generic;
using System.Data;
using System.Data.SqlServerCe;
using System.IO;
using Infrastructure.Common;

namespace GKProcessor
{
	public static partial class GKDBHelper
	{
		static bool IsPatchesTableExists()
		{
			if (!File.Exists(AppDataFolderHelper.GetDBFile("GkJournalDatabase.sdf")))
				return false;
			var connection = new SqlCeConnection(ConnectionString);
			var sqlCeCommand = new SqlCeCommand(
				"SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = 'Patches'",
				connection);
			connection.Open();
			var reader = sqlCeCommand.ExecuteReader(CommandBehavior.CloseConnection);
			bool result = reader.Read();
			connection.Close();
			connection.Dispose();
			return result;
		}

		static void CreatePatchesTable()
		{
			if (!File.Exists(AppDataFolderHelper.GetDBFile("GkJournalDatabase.sdf")))
				return;
			var connection = new SqlCeConnection(ConnectionString);
			var sqlCeCommand = new SqlCeCommand(
				"CREATE TABLE Patches (Id nvarchar(20) not null)",
				connection);
			connection.Open();
			sqlCeCommand.ExecuteNonQuery();
			connection.Close();
			connection.Dispose();
		}

		public static void AddPatchIndexToDB(string Index)
		{
			if (!File.Exists(AppDataFolderHelper.GetDBFile("GkJournalDatabase.sdf")))
				return;
			if (string.IsNullOrEmpty(Index))
				return;
			var connection = new SqlCeConnection(ConnectionString);
			var sqlCeCommand = new SqlCeCommand("Insert Into Patches (Id) Values (@p1)", connection);
			sqlCeCommand.Parameters.AddWithValue("@p1", (object)Index);
			connection.Open();
			sqlCeCommand.ExecuteNonQuery();
			connection.Close();
			connection.Dispose();
		}

		static List<string> GetAllPatches()
		{
			if (!File.Exists(AppDataFolderHelper.GetDBFile("GkJournalDatabase.sdf")))
				return null;
			var connection = new SqlCeConnection(ConnectionString);
			var sqlCeCommand = new SqlCeCommand("SELECT * FROM Patches", connection);
			var patchIndexes = new List<string>();
			connection.Open();
			var reader = sqlCeCommand.ExecuteReader(CommandBehavior.CloseConnection);
			while (reader.Read())
			{
				patchIndexes.Add(reader.GetValue(0).ToString());
			}
			connection.Close();
			connection.Dispose();
			return patchIndexes;
		}

		public static List<string> ReadAllPatches()
		{
			if (IsPatchesTableExists())
				return GetAllPatches();
			else
			{
				CreatePatchesTable();
				return new List<string>();
			}
		}

		public static void DeletePatch(string patchName)
		{
			ExecuteNonQuery("DELETE FROM Patches WHERE Id = '" + patchName +"'");
		}
	}
}