using System.Runtime.Serialization;

namespace GroupControllerModule.Models
{
    [DataContract]
    public class XDriverPropertyParameter
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Value { get; set; }
    }
}