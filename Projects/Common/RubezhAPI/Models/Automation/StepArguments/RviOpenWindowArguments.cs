﻿using RubezhAPI.Automation;
using System.Runtime.Serialization;

namespace RubezhAPI.StepArguments
{

	public class RviOpenWindowArguments
	{
		public RviOpenWindowArguments()
		{
			NameArgument = new Argument();
			NameArgument.ExplicitValue.StringValue = "";
			XArgument = new Argument();
			XArgument.ExplicitValue.IntValue = 0;
			YArgument = new Argument();
			YArgument.ExplicitValue.IntValue = 0;
			MonitorNumberArgument = new Argument();
			MonitorNumberArgument.ExplicitValue.IntValue = 1;
			LoginArgument = new Argument();
			LoginArgument.ExplicitValue.StringValue = "";
			IpArgument = new Argument();
			IpArgument.ExplicitValue.StringValue = "";
		}
		[DataMember]
		public Argument NameArgument { get; set; }
		[DataMember]
		public Argument XArgument { get; set; }
		[DataMember]
		public Argument YArgument { get; set; }
		[DataMember]
		public Argument MonitorNumberArgument { get; set; }
		[DataMember]
		public Argument LoginArgument { get; set; }
		[DataMember]
		public Argument IpArgument { get; set; }
	}
}