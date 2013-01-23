using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
	[DataContract]
	public class Email
	{
		public Email()
		{
			SendingStates = new List<StateType>();
		}

		[DataMember]
		public string Address { get; set; }

		[DataMember]
		public string FirstName { get; set; }

		[DataMember]
		public string LastName { get; set; }

		[DataMember]
		public List<StateType> SendingStates { get; set; }
	}
}