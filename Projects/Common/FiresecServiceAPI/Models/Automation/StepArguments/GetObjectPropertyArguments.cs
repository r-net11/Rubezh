using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class GetObjectPropertyArguments
	{
		public GetObjectPropertyArguments()
		{
			ObjectParameter = new Variable();
			ResultParameter = new Variable();
		}

		[DataMember]
		public Variable ObjectParameter { get; set; }

		[DataMember]
		public Variable ResultParameter { get; set; }

		[DataMember]
		public Property Property { get; set; }
	}
}
