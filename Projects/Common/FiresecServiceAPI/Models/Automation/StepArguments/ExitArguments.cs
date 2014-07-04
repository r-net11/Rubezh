using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class ExitArguments
	{
		public ExitArguments()
		{
			Uid = Guid.NewGuid();
			ExitCode = new ExitCode();
		}

		[DataMember]
		public Guid Uid { get; set; }

		[DataMember]
		public string Message { get; set; }
		
		[DataMember]
		public ExitCode ExitCode { get; set; }
	}
}