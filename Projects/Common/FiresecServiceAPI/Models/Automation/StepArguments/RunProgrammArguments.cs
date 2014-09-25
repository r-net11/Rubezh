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
			PathParameter = new Argument();
			ParametersParameter = new Argument();
		}

		[DataMember]
		public Argument PathParameter { get; set; }

		[DataMember]
		public Argument ParametersParameter { get; set; }
	}
}