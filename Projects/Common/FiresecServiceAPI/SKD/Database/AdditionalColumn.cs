using System;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace FiresecAPI
{
	[DataContract]
	public class AdditionalColumn
	{
		public AdditionalColumn()
		{
			Uid = Guid.NewGuid();
		}

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
		[Description("Текствовый")]
		Text,

		[Description("Графический")]
		Graphics,

		[Description("Смешанный")]
		Mixed
	}
}