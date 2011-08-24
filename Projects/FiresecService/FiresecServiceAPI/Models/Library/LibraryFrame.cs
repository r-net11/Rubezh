using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [DataContract]
    public class LibraryFrame
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int Duration { get; set; }

        [DataMember]
        public int Layer { get; set; }

        [DataMember]
        public string Image { get; set; }
    }
}