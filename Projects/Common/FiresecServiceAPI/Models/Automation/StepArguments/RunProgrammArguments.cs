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

		}

		[DataMember]
		public string Path { get; set; }

		[DataMember]
		public string Parameters { get; set; }
	}
}