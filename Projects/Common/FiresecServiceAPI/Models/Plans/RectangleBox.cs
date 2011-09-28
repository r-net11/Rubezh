using System.Runtime.Serialization;

namespace FiresecAPI.Models.Plans
{
    [DataContract]
    public class RectangleBox
    {
        public int idElementCanvas { get; set; }

        [DataMember]
        public double Left { get; set; }

        [DataMember]
        public double Top { get; set; }

        [DataMember]
        public double Height { get; set; }

        [DataMember]
        public double Width { get; set; }

        [DataMember]
        public byte[] BackgroundPixels { get; set; }
    }
}
