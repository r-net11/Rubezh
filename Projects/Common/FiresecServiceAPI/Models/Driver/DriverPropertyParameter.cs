using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
    [DataContract]
    public class DriverPropertyParameter
    {
        [DataMember]
        public string Name { get; set; }

		[DataMember]
		public string AlternativeName { get; set; }

        [DataMember]
        public string Value { get; set; }
    }
}