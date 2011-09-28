using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [DataContract]
    public class Sound
    {
        [DataMember]
        public StateType StateType { get; set; }

        [DataMember]
        public string SoundName { get; set; }

        [DataMember]
        public BeeperType BeeperType { get; set; }

        [DataMember]
        public bool IsContinious { get; set; }
    }
}
