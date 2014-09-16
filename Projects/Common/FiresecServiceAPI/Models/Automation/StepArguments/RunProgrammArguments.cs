using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class RunProgrammArguments
	{
		public RunProgrammArguments()
		{
			PathParameter = new ArithmeticParameter();
			ParametersParameter = new ArithmeticParameter();
		}

		[DataMember]
		public ArithmeticParameter PathParameter { get; set; }

		[DataMember]
		public ArithmeticParameter ParametersParameter { get; set; }
	}
}