using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [DataContract]
    public class ZoneState
    {
        public ZoneState()
        {
            StateType = StateType.No;
        }

        [DataMember]
        public string No { get; set; }

        [DataMember]
        public StateType StateType { get; set; }
    }
}