using System;
using System.Linq;
using System.Collections.Generic;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using System.Data.SqlServerCe;


namespace FiresecDB
{
    public static class DatabaseHelper
    {

        public static string ConnectionString { get; set; }
        public static void AddJournalRecords(List<JournalRecord> journalRecords)
        {
            try
            {
                foreach (var journalRecord in journalRecords)
                {
                    InsertJournalRecordToDb(journalRecord);
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
                using (var dataContext = new SqlCeConnection(ConnectionString))
                {
                    var query = "SELECT * FROM Journal WHERE " +
                                "\n SystemTime = '" + journalRecord.SystemTime.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                                "\n AND OldId = " + journalRecord.OldId;
                    var result = new SqlCeCommand(query, dataContext);
                    dataContext.Open();
                    var reader = result.ExecuteReader();
                    if (reader.Read() == false)
                    {
                        InsertJournalRecordToDb(journalRecord);
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
                using (var dataContext = new SqlCeConnection(ConnectionString))
                {
                    var query = "SELECT MAX(OldId) FROM Journal";
                    var result = new SqlCeCommand(query, dataContext);
                    dataContext.Open();
                    var reader = result.ExecuteReader();
                    int? firstResult = null;
                    if (reader.Read())
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

        public static void InsertJournalRecordToDb(JournalRecord journalRecord)
        {
            using (var dataContext = new SqlCeConnection(ConnectionString))
            {
                dataContext.ConnectionString = ConnectionString;
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

        public static OperationResult<List<JournalRecord>> GetFilteredJournal(JournalFilter journalFilter)
        {
            var operationResult = new OperationResult<List<JournalRecord>>();
            operationResult.Result = new List<JournalRecord>();
            try
            {
                bool hasWhere = false;
                string query = "";
                if (journalFilter.IsLastDaysCountActive)
                {
                    query = "SELECT * FROM Journal WHERE " +
                    "\n SystemTime > '" + DateTime.Now.AddDays(-journalFilter.LastDaysCount).ToString("yyyy-MM-dd HH:mm:ss") + "'";
                    hasWhere = true;
                }
                else
                {
                    query = "SELECT TOP (" + journalFilter.LastRecordsCount + ") * FROM Journal ";
                }

                if (journalFilter.StateTypes.Count > 0)
                {
                    if (hasWhere == false)
                    {
                        query += " WHERE ";
                        hasWhere = true;
                    }
                    else
                    {
                        query += "\n AND ";
                    }
                    query += " (";
                    for (int i = 0; i < journalFilter.StateTypes.Count; i++)
                    {
                        if (i > 0)
                            query += "\n OR ";
                        var stateType = journalFilter.StateTypes[i];
                        query += " StateType = " + ((int)stateType).ToString();
                    }
                    query += ")";
                }

                if (journalFilter.Categories.Count > 0)
                {
                    if (hasWhere == false)
                    {
                        query += " WHERE ";
                        hasWhere = true;
                    }
                    else
                    {
                        query += "\n AND ";
                    }
                    query += " (";
                    for (int i = 0; i < journalFilter.Categories.Count; i++)
                    {
                        if (i > 0)
                            query += "\n OR ";
                        var category = journalFilter.Categories[i];
                        query += " DeviceCategory = " + ((int)category).ToString();
                    }
                    query += ")";
                }

                query += "\n ORDER BY SystemTime DESC";

                using (var DataBaseContext = new SqlCeConnection(ConnectionString))
                {
                    DataBaseContext.ConnectionString = ConnectionString;
                    var result = new SqlCeCommand(query, DataBaseContext);
                    DataBaseContext.Open();
                    var reader = result.ExecuteReader();
                    while (reader.Read())
                    {
                        var journalRecord = new JournalRecord();
                        journalRecord.Description = reader.GetString(reader.GetOrdinal("Description"));
                        journalRecord.Detalization = reader.IsDBNull(12)
                                                         ? null
                                                         : reader.GetString(reader.GetOrdinal("Detalization"));
                        journalRecord.DeviceCategory = reader.GetInt32(reader.GetOrdinal("DeviceCategory"));
                        journalRecord.DeviceDatabaseId = reader.GetString(reader.GetOrdinal("DeviceDatabaseId"));
                        journalRecord.DeviceName = reader.GetString(reader.GetOrdinal("DeviceName"));
                        journalRecord.DeviceTime = reader.GetDateTime(reader.GetOrdinal("DeviceTime"));
                        journalRecord.OldId = reader.GetInt32(reader.GetOrdinal("OldId"));
                        journalRecord.PanelDatabaseId = reader.GetString(reader.GetOrdinal("PanelDatabaseId"));
                        journalRecord.PanelName = reader.GetString(reader.GetOrdinal("PanelName"));
                        journalRecord.StateType = (StateType) reader.GetInt32(reader.GetOrdinal("StateType"));
                        journalRecord.SubsystemType =
                            (SubsystemType) reader.GetInt32(reader.GetOrdinal("SubsystemType"));
                        journalRecord.SystemTime = reader.GetDateTime(reader.GetOrdinal("SystemTime"));
                        journalRecord.User = reader.GetString(reader.GetOrdinal("UserName"));
                        journalRecord.ZoneName = reader.GetString(reader.GetOrdinal("ZoneName"));
                        journalRecord.No = reader.GetInt32(reader.GetOrdinal("Id"));
                        operationResult.Result.Add(journalRecord);
                    }
                    DataBaseContext.Close();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "Исключение при вызове FiresecService.GetFilteredJournal");
                operationResult.HasError = true;
                operationResult.Error = e.Message.ToString();
            }
            return operationResult;
        }

        public static OperationResult<List<JournalRecord>> OnGetFilteredArchive(ArchiveFilter archiveFilter)
        {
            var operationResult = new OperationResult<List<JournalRecord>>();
            operationResult.Result = new List<JournalRecord>();
            try
            {
                string dateInQuery = "DeviceTime";
                if (archiveFilter.UseSystemDate)
                    dateInQuery = "SystemTime";

                var query =
                    "SELECT * FROM Journal WHERE " +
                    "\n " + dateInQuery + " > '" + archiveFilter.StartDate.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                    "\n AND " + dateInQuery + " < '" + archiveFilter.EndDate.ToString("yyyy-MM-dd HH:mm:ss") + "'";

                if (archiveFilter.Descriptions.Count > 0)
                {
                    query += "\n AND (";
                    for (int i = 0; i < archiveFilter.Descriptions.Count; i++)
                    {
                        if (i > 0)
                            query += "\n OR ";
                        var description = archiveFilter.Descriptions[i];
                        query += " Description = '" + description + "'";
                    }
                    query += ")";
                }

                if (archiveFilter.Subsystems.Count > 0)
                {
                    query += "\n AND (";
                    for (int i = 0; i < archiveFilter.Subsystems.Count; i++)
                    {
                        if (i > 0)
                            query += "\n OR ";
                        var subsystem = archiveFilter.Subsystems[i];
                        query += " SubSystemType = '" + ((int) subsystem).ToString() + "'";
                    }
                    query += ")";
                }

                if (archiveFilter.DeviceNames.Count > 0)
                {
                    query += "\n AND (";
                    for (int i = 0; i < archiveFilter.DeviceNames.Count; i++)
                    {
                        var deviceName = archiveFilter.DeviceNames[i];
                        if (deviceName != null)
                        {
                            if (i > 0)
                                query += "\n OR ";
                            query += " PanelName = '" + deviceName + "'";
                        }
                    }
                    query += ")";
                }

                query += "\n ORDER BY " + dateInQuery + " DESC";

                using (var DataBaseContext = new SqlCeConnection(ConnectionString))
                {
                    DataBaseContext.ConnectionString = ConnectionString;
                    var journalRecords = new List<JournalRecord>();
                    var result = new SqlCeCommand(query, DataBaseContext);
                    DataBaseContext.Open();
                    var reader = result.ExecuteReader();
                    while (reader.Read())
                    {
                        try
                        {
                            var journalRecord = new JournalRecord();
                            journalRecord.Description = reader.GetString(reader.GetOrdinal("Description"));
                            journalRecord.Detalization = reader.IsDBNull(12)
                                                             ? null
                                                             : reader.GetString(reader.GetOrdinal("Detalization"));
                            journalRecord.DeviceCategory = reader.GetInt32(reader.GetOrdinal("DeviceCategory"));
                            journalRecord.DeviceDatabaseId = reader.GetString(reader.GetOrdinal("DeviceDatabaseId"));
                            journalRecord.DeviceName = reader.GetString(reader.GetOrdinal("DeviceName"));
                            journalRecord.DeviceTime = reader.GetDateTime(reader.GetOrdinal("DeviceTime"));
                            journalRecord.OldId = reader.GetInt32(reader.GetOrdinal("OldId"));
                            journalRecord.PanelDatabaseId = reader.GetString(reader.GetOrdinal("PanelDatabaseId"));
                            journalRecord.PanelName = reader.GetString(reader.GetOrdinal("PanelName"));
                            journalRecord.StateType = (StateType) reader.GetInt32(reader.GetOrdinal("StateType"));
                            journalRecord.SubsystemType =
                                (SubsystemType) reader.GetInt32(reader.GetOrdinal("SubsystemType"));
                            journalRecord.SystemTime = reader.GetDateTime(reader.GetOrdinal("SystemTime"));
                            journalRecord.User = reader.GetString(reader.GetOrdinal("UserName"));
                            journalRecord.ZoneName = reader.GetString(reader.GetOrdinal("ZoneName"));
                            journalRecord.No = reader.GetInt32(reader.GetOrdinal("Id"));
                            operationResult.Result.Add(journalRecord);
                            journalRecords.Add(journalRecord);
                            if (journalRecords.Count > 10)
                            {
                                if (ArchivePortionReady != null)
                                    ArchivePortionReady(journalRecords.ToList());

                                journalRecords.Clear();
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                    if (journalRecords.Count > 0)
                    {
                        if (ArchivePortionReady != null)
                            ArchivePortionReady(journalRecords.ToList());
                    }

                    DataBaseContext.Close();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "Исключение при вызове FiresecService.GetFilteredArchive");
                operationResult.HasError = true;
                operationResult.Error = e.Message.ToString();
            }
            return operationResult;
        }

        public static event Action<List<JournalRecord>> ArchivePortionReady;

        public static OperationResult<List<JournalDescriptionItem>> GetDistinctDescriptions()
        {
            var operationResult = new OperationResult<List<JournalDescriptionItem>>();
            operationResult.Result = new List<JournalDescriptionItem>();
            try
            {
                string query = "SELECT DISTINCT StateType, Description FROM Journal ORDER BY Description";
                using (var DataBaseContext = new SqlCeConnection(ConnectionString))
                {
                    DataBaseContext.ConnectionString = ConnectionString;
                    var result = new SqlCeCommand(query, DataBaseContext);
                    DataBaseContext.Open();
                    var reader = result.ExecuteReader();
                    while (reader.Read())
                    {
                        var journalDescriptionItem = new JournalDescriptionItem();
                        journalDescriptionItem.Description = reader.GetString(reader.GetOrdinal("Description"));
                        journalDescriptionItem.StateType = (StateType) reader.GetInt32(reader.GetOrdinal("StateType"));
                        operationResult.Result.Add(journalDescriptionItem);
                    }
                    DataBaseContext.Close();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "Исключение при вызове FiresecService.GetDistinctDescriptions");
                operationResult.HasError = true;
                operationResult.Error = e.Message.ToString();
            }
            return operationResult;
        }

        public static OperationResult<DateTime> GetArchiveStartDate()
        {
            var operationResult = new OperationResult<DateTime>();
            try
            {
                string query = "SELECT MIN(SystemTime) AS SystemTime FROM Journal";
                using (var DataBaseContext = new SqlCeConnection(ConnectionString))
                {
                    DataBaseContext.ConnectionString = ConnectionString;
                    var result = new SqlCeCommand(query, DataBaseContext);
                    DataBaseContext.Open();
                    var reader = result.ExecuteReader();
                    var dateTime = new DateTime();
                    if (reader.Read())
                    {
                        operationResult.Result = reader.GetDateTime(reader.GetOrdinal("SystemTime"));
                    }
                    DataBaseContext.Close();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "Исключение при вызове FiresecService.GetArchiveStartDate");
                operationResult.Result = DateTime.Now;
                operationResult.HasError = true;
                operationResult.Error = e.Message.ToString();
            }
            return operationResult;
        }

        public static void SetJournal(List<JournalRecord> journalRecords)
        {
            using (var DataBaseContext = new SqlCeConnection(ConnectionString))
            {
                DataBaseContext.ConnectionString = ConnectionString;
                var result = new SqlCeCommand("DELETE FROM Journal", DataBaseContext);
                DataBaseContext.Open();
                result.ExecuteReader();
                foreach (var journalRecord in journalRecords)
                {
                    InsertJournalRecordToDb(journalRecord);
                }
                DataBaseContext.Close();
            }
        }
    }
}
