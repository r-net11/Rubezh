using System.Runtime.Serialization;

namespace FiresecAPI.Models.Plans
{
    [DataContract]
    public class ElementBase
    {
        [DataMember]
        public double Left { get; set; }

        [DataMember]
        public double Top { get; set; }

        [DataMember]
        public double Height { get; set; }

        [DataMember]
        public double Width { get; set; }
    }
}
