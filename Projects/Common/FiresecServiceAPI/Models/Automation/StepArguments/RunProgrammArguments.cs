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
			PathParameter = new Variable();
			ParametersParameter = new Variable();
		}

		[DataMember]
		public Variable PathParameter { get; set; }

		[DataMember]
		public Variable ParametersParameter { get; set; }
	}
}