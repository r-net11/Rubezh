using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    public class PolygonPoint
    {
        [DataMember]
        public double X { get; set; }

        [DataMember]
        public double Y { get; set; }
    }
}
