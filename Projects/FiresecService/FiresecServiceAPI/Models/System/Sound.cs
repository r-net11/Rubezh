using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    public enum SpeakerType
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
        public SpeakerType SpeakerType { get; set; }

        [DataMember]
        public bool IsContinious { get; set; }
    }
}
