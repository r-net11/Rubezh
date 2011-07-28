using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [DataContract]
    public class ZoneState
    {
        public ZoneState(string no)
        {
            No = no;
            State = new State(8);
        }

        [DataMember]
        public string No { get; set; }

        [DataMember]
        public State State { get; set; }
    }
}
