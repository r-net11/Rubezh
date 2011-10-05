using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    public enum RemoteAccessType
    {
        RemoteAccessBanned,
        RemoteAccessAllowed,
        SelectivlyAllowed
    }

    [DataContract]
    public class RemoteAccess
    {
        [DataMember]
        public RemoteAccessType RemoteAccessType { get; set; }

        [DataMember]
        public List<string> HostNameOrAddressList { get; set; }
    }
}