using System;
using System.Runtime.Serialization;

namespace FiresecAPI
{
    [DataContract]
    public class AdditionalColumn
	{
        [DataMember]
        public Guid Uid;
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public AdditionalColumnType Type { get; set; }
        [DataMember]
        public string TextData { get; set; }
        [DataMember]
        public byte[] GraphicsData { get; set; }
	}

    [DataContract]
    public enum AdditionalColumnType
	{
		Text,
		Graphics,
		Mixed
	}
}
