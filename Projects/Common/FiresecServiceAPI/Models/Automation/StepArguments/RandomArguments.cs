using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
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