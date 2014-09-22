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
			MaxValueParameter = new Variable();
			ResultParameter = new Variable();
		}

		[DataMember]
		public Variable MaxValueParameter { get; set; }

		[DataMember]
		public Variable ResultParameter { get; set; }
	}
}