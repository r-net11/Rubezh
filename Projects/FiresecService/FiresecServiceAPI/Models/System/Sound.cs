using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    public enum BeeperType
    {
        None = 0,
        Alarm = 200,
        Attention = 50
    }

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
