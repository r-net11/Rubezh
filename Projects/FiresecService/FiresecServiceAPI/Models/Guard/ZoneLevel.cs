using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [DataContract]
    public class ZoneLevel
    {
        [DataMember]
        public ZoneActionType ZoneActionType { get; set; }

        [DataMember]
        public string ZoneNo { get; set; }
    }
}
