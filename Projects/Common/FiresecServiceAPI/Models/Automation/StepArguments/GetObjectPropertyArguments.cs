using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class GetObjectPropertyArguments
	{
		public GetObjectPropertyArguments()
		{
			ObjectParameter = new Argument();
			ResultParameter = new Argument();
		}

		[DataMember]
		public Argument ObjectParameter { get; set; }

		[DataMember]
		public Argument ResultParameter { get; set; }

		[DataMember]
		public Property Property { get; set; }
	}
}
