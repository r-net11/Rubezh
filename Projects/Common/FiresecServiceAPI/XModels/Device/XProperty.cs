using System.Runtime.Serialization;

namespace XFiresecAPI
{
    [DataContract]
    public class XProperty
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public short Value { get; set; }

		[DataMember]
		public string StringValue { get; set; }
    }
}