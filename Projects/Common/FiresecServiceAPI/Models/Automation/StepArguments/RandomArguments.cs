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
			MaxValueParameter = new ArithmeticParameter();
		}

		[DataMember]
		public ArithmeticParameter MaxValueParameter { get; set; }
	}
}