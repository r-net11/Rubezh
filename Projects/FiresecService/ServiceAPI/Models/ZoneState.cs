
using System.Runtime.Serialization;
namespace FiresecClient.Models
{
    [DataContract]
    public class ZoneState
    {
        [DataMember]
        public string No { get; private set; }

        [DataMember]
        public State State { get; set; }

        public ZoneState(string no)
        {
            No = no;
            State = new State(8);
        }
    }
}
