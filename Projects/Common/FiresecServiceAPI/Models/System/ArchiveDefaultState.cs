using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    public enum ArchiveDefaultStateType
    {
        LastHours,
        LastDays,
        All,
        FromDate,
        RangeDate
    }

    [DataContract]
    public class ArchiveDefaultState
    {
        [DataMember]
        public ArchiveDefaultStateType ArchiveDefaultStateType { get; set; }

        [DataMember]
        public int? Count { get; set; }

        [DataMember]
        public DateTime? StartDate { get; set; }

        [DataMember]
        public DateTime? EndDate { get; set; }
    }
}