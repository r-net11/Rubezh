using System.Runtime.Serialization;

namespace GroupControllerModule.Models
{
    [DataContract]
    public class XZone
    {
        public XZone()
        {
            DetectorCount = 2;
        }

        [DataMember]
        public ulong No { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public int DetectorCount { get; set; }

        public string PresentationName
        {
            get { return No + "." + Name; }
        }
    }
}