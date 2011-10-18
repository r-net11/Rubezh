using System.Runtime.Serialization;
using System.Windows.Media;

namespace FiresecAPI.Models.Plans
{
    [DataContract]
    public class ElementRectangle : ElementBase
    {
        public int idElementCanvas { get; set; }

        [DataMember]
        public byte[] BackgroundPixels { get; set; }

        [DataMember]
        public Color BackgroundColor { get; set; }

        [DataMember]
        public Color BorderColor { get; set; }

        [DataMember]
        public double BorderThickness { get; set; }
    }
}
