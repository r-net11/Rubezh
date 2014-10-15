using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract, Serializable]
	public class RandomArguments
	{
		public RandomArguments()
		{
			MaxValueArgument = new Argument();
			ResultArgument = new Argument();
		}

		[DataMember]
		public Argument MaxValueArgument { get; set; }

		[DataMember]
		public Argument ResultArgument { get; set; }
	}
}