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
			PathArgument = new Argument();
			ParametersArgument = new Argument();
		}

		[DataMember]
		public Argument PathArgument { get; set; }

		[DataMember]
		public Argument ParametersArgument { get; set; }
	}
}