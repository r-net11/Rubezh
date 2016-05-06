using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrazhAPI.Models
{
	[DataContract]
	public class RemoteAccess
	{
		public RemoteAccess()
		{
			HostNameOrAddressList = new List<string>();
		}

		[DataMember]
		public RemoteAccessType RemoteAccessType { get; set; }

		[DataMember]
		public List<string> HostNameOrAddressList { get; set; }
	}
}