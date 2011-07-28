using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [DataContract]
    public class ZoneState
    {
        public ZoneState()
        {
            State = new State();
        }

        [DataMember]
        public string No { get; set; }

        [DataMember]
        public State State { get; set; }
    }
}
