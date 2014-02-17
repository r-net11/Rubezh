using System.Collections.Generic;
using System.Data;
using System.Data.SqlServerCe;
using System.IO;
using Infrastructure.Common;

namespace GKProcessor
{
	public static partial class GKDBHelper
	{
		public static void ExecuteNonQuery(string commandString)
		{
			ExecuteNonQuery(new List<string> { commandString });
		}

		public static void ExecuteNonQuery(List<string> commandStrings)
		{
			if (!File.Exists(AppDataFolderHelper.GetDBFile("GkJournalDatabase.sdf")))
				return;
			var commands = new List<SqlCeCommand>();
			var connection = new SqlCeConnection(ConnectionString);
			foreach (var commandString in commandStrings)
				commands.Add(new SqlCeCommand(commandString, connection));
			connection.Open();
			foreach (var command in commands)
				command.ExecuteNonQuery();
			connection.Close();
			connection.Dispose();
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

		public static void DropTableIfExists(string Name)
		{
			if (IsTableExists(Name))
				ExecuteNonQuery("DROP TABLE " + Name);
		}

		static bool IsTableExists(string Name)
		{
			if (!File.Exists(AppDataFolderHelper.GetDBFile("GkJournalDatabase.sdf")))
				return false;
			var connection = new SqlCeConnection(ConnectionString);
			var sqlCeCommand = new SqlCeCommand(
				"SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_name = '" + Name + "'",
				connection);
			connection.Open();
			var reader = sqlCeCommand.ExecuteReader(CommandBehavior.CloseConnection);
			bool result = reader.Read();
			connection.Close();
			connection.Dispose();
			return result;
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
	}
}