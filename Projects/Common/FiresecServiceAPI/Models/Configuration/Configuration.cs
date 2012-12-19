using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [DataContract]
    public class Configuration
    {
        [DataMember]
        public string Name{ get; set; }
        [DataMember]
        public int MajorVersion { get; set; }
        [DataMember]
        public int MinorVersion { get; set; }
    }
}
