using System;
using System.Collections.Generic;
using Common;
using FiresecAPI.Models;
using FiresecService.Properties;
using System.Data.SqlServerCe;

namespace FiresecService.Database
{
    public static class DatabaseHelper
    {
        public static void AddJournalRecords(List<JournalRecord> journalRecords)
        {
            try
            {
                foreach (var journalRecord in journalRecords)
                {
                    InsertJournalRecordToDB(journalRecord);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "Исключение при вызове DatabaseHelper.AddJournalRecords");
            }
        }

        public static bool AddJournalRecord(JournalRecord journalRecord)
        {
            try
            {
                using (var dataContext = new SqlCeConnection(Settings.Default.FiresecConnectionString))
                {
                    var query = "SELECT * FROM Journal WHERE " +
                                "\n SystemTime = '" + journalRecord.SystemTime.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                                "\n AND OldId = " + journalRecord.OldId;
                    var result = new SqlCeCommand(query, dataContext);
                    dataContext.Open();
                    var reader = result.ExecuteReader();
                    if (reader.Read() == false)
                    {
                        InsertJournalRecordToDB(journalRecord);
                    }
                    return true;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "Исключение при вызове DatabaseHelper.AddJournalRecord");
            }
            return false;
        }

        public static int GetLastOldId()
        {
            try
            {
                using (var dataContext = new SqlCeConnection(Settings.Default.FiresecConnectionString))
                {
                    var query = "SELECT MAX(OldId) FROM Journal";
                    var result = new SqlCeCommand(query, dataContext);
                    dataContext.Open();
                    var reader = result.ExecuteReader();
                    int? firstResult = null;
                    if (reader.Read() != false)
                        firstResult = (int)reader[0];
                    dataContext.Close();
                    if (firstResult != null)
                        return (int)firstResult;
                    return -1;
                }
            }
            catch (Exception e)
                {
                    Logger.Error(e, "Исключение при вызове DatabaseHelper.GetLastOldId");
                }
                return -1;
        }

        public static void InsertJournalRecordToDB(JournalRecord journalRecord)
        {
            using (var dataContext = new SqlCeConnection(Settings.Default.FiresecConnectionString))
            {
                dataContext.Open();
                var cmd = new SqlCeCommand();
                cmd.Connection = dataContext;
                cmd.CommandText = @"Insert Into Journal" +
                                  "(Description,Detalization,DeviceCategory,DeviceDatabaseId,DeviceName,DeviceTime,OldId,PanelDatabaseId,PanelName,StateType,SubsystemType,SystemTime,UserName,ZoneName) Values" +
                                  "(@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11,@p12,@p13,@p14)";
                cmd.Parameters.AddWithValue("@p1", (object)journalRecord.Description ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@p2", (object)journalRecord.Detalization ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@p3", (object)journalRecord.DeviceCategory ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@p4", (object)journalRecord.DeviceDatabaseId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@p5", (object)journalRecord.DeviceName ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@p6", (object)journalRecord.DeviceTime ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@p7", (object)journalRecord.OldId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@p8", (object)journalRecord.PanelDatabaseId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@p9", (object)journalRecord.PanelName ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@p10", (object)journalRecord.StateType ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@p11", (object)journalRecord.SubsystemType ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@p12", (object)journalRecord.SystemTime ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@p13", (object)journalRecord.User ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@p14", (object)journalRecord.ZoneName ?? DBNull.Value);
                cmd.ExecuteNonQuery();
                dataContext.Close();
            }
        }
    }
}