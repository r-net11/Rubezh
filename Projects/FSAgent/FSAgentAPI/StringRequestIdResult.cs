using System.Runtime.Serialization;

namespace FSAgentAPI
{
	[DataContract]
	public class StringRequestIdResult
	{
		[DataMember]
		public string Result { get; set; }

		[DataMember]
		public int ReguestId { get; set; }
	}
}