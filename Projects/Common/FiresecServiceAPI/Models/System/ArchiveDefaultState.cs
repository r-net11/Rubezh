using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    public enum ArchiveDefaultStateType
    {
        LastHour,
        LastDay,
        FromDate,
        Range
    }

    [DataContract]
    public class ArchiveDefaultState
    {
        [DataMember]
        public ArchiveDefaultStateType ArchiveDefaultStateType { get; set; }

        [DataMember]
        public ArchiveFilter ArchiveFilter { get; set; }
    }
}