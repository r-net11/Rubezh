using System.Runtime.Serialization;

namespace XFiresecAPI
{
    [DataContract]
    public class XDeviceParameter
    {
        [DataMember]
        public byte No { get; set; }

        [DataMember]
        public short Value { get; set; }
    }
}