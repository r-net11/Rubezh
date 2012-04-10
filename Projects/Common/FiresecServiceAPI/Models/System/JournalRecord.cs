using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [Table(Name = "Journal")]
    [DataContract]
    public class JournalRecord : IEqualityComparer<JournalRecord>
    {
        [Column(AutoSync = AutoSync.OnInsert, DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true, Name = "Id")]
        [DataMember]
        public int No { get; set; }

        [Column(DbType = "DateTime")]
        [DataMember]
        public DateTime DeviceTime { get; set; }

        [Column(DbType = "DateTime")]
        [DataMember]
        public DateTime SystemTime { get; set; }

        [Column(DbType = "NVarChar(MAX)")]
        [DataMember]
        public string ZoneName { get; set; }

        [Column(DbType = "NVarChar(MAX)")]
        [DataMember]
        public string Description { get; set; }

        [Column(DbType = "NVarChar(MAX)")]
        [DataMember]
        public string DeviceName { get; set; }

        [Column(DbType = "NVarChar(MAX)")]
        [DataMember]
        public string PanelName { get; set; }

        [Column(DbType = "NVarChar(MAX)")]
        [DataMember]
        public string DeviceDatabaseId { get; set; }

        [Column(DbType = "NVarChar(MAX)")]
        [DataMember]
        public string PanelDatabaseId { get; set; }

        [Column(Name = "UserName", DbType = "NVarChar(MAX)")]
        [DataMember]
        public string User { get; set; }

        [Column(Name = "SubSystemType", DbType = "Int")]
        [DataMember]
        public SubsystemType SubsystemType { get; set; }

        [Column(DbType = "Int")]
        [DataMember]
        public StateType StateType { get; set; }

        [Column(DbType = "NVarChar(MAX)")]
        [DataMember]
        public string Detalization { get; set; }

        public bool Equals(JournalRecord x, JournalRecord y)
        {
            if (object.ReferenceEquals(x, y)) return true;

            if (object.ReferenceEquals(x, null) || object.ReferenceEquals(y, null))
                return false;

            return x.Description == y.Description;
        }

        public int GetHashCode(JournalRecord obj)
        {
            return obj.Description.GetHashCode();
        }
    }
}