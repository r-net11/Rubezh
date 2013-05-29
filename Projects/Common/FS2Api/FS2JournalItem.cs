using System.Runtime.Serialization;

namespace FS2Api
{
	[DataContract]
	public class FS2JournalItem
	{
		[DataMember]
		public int No { get; set; }

		[DataMember]
		public string Date { get; set; }

		[DataMember]
		public string EventName { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public int Flag { get; set; }

		[DataMember]
		public int ShleifNo { get; set; }

		[DataMember]
		public int IntType { get; set; }

		[DataMember]
		public int FirstAddress { get; set; }

		[DataMember]
		public int Address { get; set; }

		[DataMember]
		public int State { get; set; }

		[DataMember]
		public int ZoneNo { get; set; }

		[DataMember]
		public int DescriptorNo { get; set; }

		[DataMember]
		public string StringType { get; set; }

		[DataMember]
		public int EventClass { get; set; }

		[DataMember]
		public string ByteTracer { get; set; }

		[DataMember]
		public uint IntDate { get; set; }
	}
}