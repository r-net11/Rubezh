using System.Runtime.Serialization;

namespace FiresecAPI.Models.Plans
{
    [DataContract]
    public class CaptionBox
    {
        [DataMember]
        public int idElementCanvas { get; set; }

        [DataMember]
        public double Left { get; set; }

        [DataMember]
        public double Top { get; set; }

        [DataMember]
        public string Text { get; set; }

        [DataMember]
        public double FontSize { get; set; }

        [DataMember]
        public string Color { get; set; }

        [DataMember]
        public string BorderColor { get; set; }
    }
}
