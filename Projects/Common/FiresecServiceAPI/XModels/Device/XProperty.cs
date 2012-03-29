using System.Runtime.Serialization;

namespace XFiresecAPI
{
    [DataContract]
    public class XProperty
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public object Value { get; set; }
    }
}