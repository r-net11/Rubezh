using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [DataContract]
    public class Sound
    {
        [DataMember]
        public string StateName { get; set; }

        [DataMember]
        public string SoundName { get; set; }

        [DataMember]
        public string SpeakerName { get; set; }

        [DataMember]
        public bool IsContinious { get; set; }
    }
}
