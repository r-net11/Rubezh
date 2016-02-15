using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RubezhAPI
{
	[DataContract]
	public class RviCallbackResult
	{
		public RviCallbackResult()
		{
			RviStates = new List<RviState>();
		}
		[DataMember]
		public List<RviState> RviStates { get; set; }
	}
}