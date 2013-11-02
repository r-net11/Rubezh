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
                "CREATE TABLE Patches (Type nvarchar(20) not null, Id nvarchar(20) not null)",
                connection);
            connection.Open();
            sqlCeCommand.ExecuteNonQuery();
            connection.Close();
            connection.Dispose();
        }

        public static void AddPatchToDB(PatchIndex patch)
        {
            if (!File.Exists(AppDataFolderHelper.GetDBFile("GkJournalDatabase.sdf")))
                return;
            if (patch == null || patch.Id == null || patch.Type == null)
                return;
            var connection = new SqlCeConnection(ConnectionString);
            var sqlCeCommand = new SqlCeCommand("Insert Into Patches (Type,Id) Values (@p1,@p2)", connection);
            sqlCeCommand.Parameters.AddWithValue("@p1", (object)patch.Type);
            sqlCeCommand.Parameters.AddWithValue("@p2", (object)patch.Id);
            connection.Open();
            sqlCeCommand.ExecuteNonQuery();
            connection.Close();
            connection.Dispose();
        }

        static List<PatchIndex> GetAllPatches()
        {
            if (!File.Exists(AppDataFolderHelper.GetDBFile("GkJournalDatabase.sdf")))
                return null;
            var connection = new SqlCeConnection(ConnectionString);
            var sqlCeCommand = new SqlCeCommand("SELECT * FROM Patches", connection);
            var patchIndexes = new List<PatchIndex>();
            connection.Open();
            var reader = sqlCeCommand.ExecuteReader(CommandBehavior.CloseConnection);
            while (reader.Read())
            {
                patchIndexes.Add(new PatchIndex(reader.GetValue(0).ToString(), reader.GetValue(1).ToString()));
            }
            connection.Close();
            connection.Dispose();

            return patchIndexes;
        }

        public static List<PatchIndex> ReadAllPatches()
        {
            if (IsPatchesTableExists())
                return GetAllPatches();
            else
            {
                CreatePatchesTable();
                return new List<PatchIndex>();
            }
        }
    }
}