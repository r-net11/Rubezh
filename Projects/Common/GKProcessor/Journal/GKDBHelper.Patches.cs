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

		public static void AddColumn(string columnName, string columnType, string tableName)
		{
			if (!File.Exists(AppDataFolderHelper.GetDBFile("GkJournalDatabase.sdf")))
				return;
			var connection = new SqlCeConnection(ConnectionString);
			if (!IsColumnExists(columnName, tableName))
			{
				var sqlCeCommand = new SqlCeCommand("alter table " + tableName + " add column " + columnName.ToString() + " " + columnType.ToString(), connection);
				connection.Open();
				sqlCeCommand.ExecuteNonQuery();
			}
			connection.Close();
			connection.Dispose();
		}

		public static void AlterColumnType(string columnName, string columnType, string tableName)
		{
			if (!File.Exists(AppDataFolderHelper.GetDBFile("GkJournalDatabase.sdf")))
				return;
			var connection = new SqlCeConnection(ConnectionString);
			if (IsColumnExists(columnName, tableName))
			{
				var sqlCeCommand = new SqlCeCommand("alter table " + tableName + " alter column " + columnName.ToString() + " " + columnType.ToString(), connection);
				connection.Open();
				sqlCeCommand.ExecuteNonQuery();
			}
			connection.Close();
			connection.Dispose();
		}

		public static void DropColumn(string columnName, string tableName)
		{
			if (!File.Exists(AppDataFolderHelper.GetDBFile("GkJournalDatabase.sdf")))
				return;
			var connection = new SqlCeConnection(ConnectionString);
			if (IsColumnExists(columnName, tableName))
			{
				var sqlCeCommand = new SqlCeCommand("alter table " + tableName + " drop column " + columnName.ToString(), connection);
				connection.Open();
				sqlCeCommand.ExecuteNonQuery();
			}
			connection.Close();
			connection.Dispose();
		}

		public static void ExecuteNonQuery(string commandString)
		{
			if (!File.Exists(AppDataFolderHelper.GetDBFile("GkJournalDatabase.sdf")))
				return;
			var connection = new SqlCeConnection(ConnectionString);
			var sqlCeCommand = new SqlCeCommand(commandString, connection);
			connection.Open();
			sqlCeCommand.ExecuteNonQuery();
			connection.Close();
			connection.Dispose();
		}

		static bool IsColumnExists(string columnName, string tableName)
		{
			if (!File.Exists(AppDataFolderHelper.GetDBFile("GkJournalDatabase.sdf")))
				return false;
			var connection = new SqlCeConnection(ConnectionString);
			var sqlCeCommand = new SqlCeCommand(
				"select column_name from INFORMATION_SCHEMA.columns where column_name = '" + columnName + "' and table_name = '" + tableName + "'",
				connection);
			connection.Open();
			var reader = sqlCeCommand.ExecuteReader();
			bool result = reader.Read();
			connection.Close();
			connection.Dispose();
			return result;
		}

        static int GetColumnLength(string columnName, string tableName)
		{
			if (!File.Exists(AppDataFolderHelper.GetDBFile("GkJournalDatabase.sdf")))
				return 0;
			int result = 0;
			var connection = new SqlCeConnection(ConnectionString);
			var sqlCeCommand = new SqlCeCommand(
				"select character_maximum_length from INFORMATION_SCHEMA.columns where column_name = '" + columnName + "' and table_name = '" + tableName + "'",
				connection);
			connection.Open();
			var reader = sqlCeCommand.ExecuteReader(CommandBehavior.CloseConnection);
			if (reader.Read())
			{
				result = reader.GetInt32(0);
			}
			else
			{
				result = 0;
			}
			connection.Close();
			connection.Dispose();
			return result;
		}

        public static void AddIndex(string indexName, string tableName, string columnName)
        {
            if (IsIndexExists(indexName, tableName))
                ExecuteNonQuery(@"DROP INDEX " + tableName + "." + indexName);
            ExecuteNonQuery(@"CREATE INDEX " + indexName + " ON " + tableName + " ( " + columnName + " )");
        }

        static bool IsIndexExists(string indexName, string tableName)
        {
            if (!File.Exists(AppDataFolderHelper.GetDBFile("GkJournalDatabase.sdf")))
                return false;
            var connection = new SqlCeConnection(ConnectionString);
            var sqlCeCommand = new SqlCeCommand(
                "select INDEX_NAME from INFORMATION_SCHEMA.INDEXES where INDEX_Name = '" + indexName + "' and table_name = '" + tableName + "'",
                connection);
            connection.Open();
            var reader = sqlCeCommand.ExecuteReader();
            bool result = reader.Read();
            connection.Close();
            connection.Dispose();
            return result;
        }

        public static void DeletePatch(string patchName)
        {
            ExecuteNonQuery("DELETE FROM Patches WHERE Id = " + patchName);
        }

    }
}