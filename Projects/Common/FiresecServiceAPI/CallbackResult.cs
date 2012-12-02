using System.Collections.Generic;
using System.Runtime.Serialization;
using FiresecAPI.Models;

namespace FiresecAPI
{
    [DataContract]
    public class CallbackResult
    {
        [DataMember]
        public CallbackResultType CallbackResultType { get; set; }

        [DataMember]
        public List<JournalRecord> JournalRecords { get; set; }
    }

    public enum CallbackResultType
    {
        NewEvents,
        ArchiveCompleted,
        ConfigurationChanged,
        Disconnecting
    }
}