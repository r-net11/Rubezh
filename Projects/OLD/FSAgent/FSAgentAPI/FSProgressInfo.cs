using System.Runtime.Serialization;

namespace FSAgentAPI
{
	[DataContract]
	public class FSProgressInfo
	{
		[DataMember]
		public int Stage { get; set; }

		[DataMember]
		public string Comment { get; set; }

		[DataMember]
		public int PercentComplete { get; set; }

		[DataMember]
		public int BytesRW { get; set; }
	}
}