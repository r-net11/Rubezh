using System.Runtime.Serialization;
namespace FiresecAPI.Models
{
    [DataContract]
    public class Event
    {
        public Event(string name)
        {
            Name = name;
        }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public bool IsEnable { get; set; }
    }
}
