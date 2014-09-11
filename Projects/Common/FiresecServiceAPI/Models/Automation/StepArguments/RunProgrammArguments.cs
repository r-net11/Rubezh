using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	public class RunProgrammArguments
	{
		public RunProgrammArguments()
		{
			Path = new ArithmeticParameter();
			Parameters = new ArithmeticParameter();
		}

		[DataMember]
		public ArithmeticParameter Path { get; set; }

		[DataMember]
		public ArithmeticParameter Parameters { get; set; }
	}
}