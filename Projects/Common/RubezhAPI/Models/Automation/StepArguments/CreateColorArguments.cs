﻿using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class CreateColorArguments
	{
		public CreateColorArguments()
		{
			AArgument = new Argument();
			RArgument = new Argument();
			GArgument = new Argument();
			BArgument = new Argument();
			ResultArgument = new Argument();
		}

		[DataMember]
		public Argument AArgument { get; set; }

		[DataMember]
		public Argument RArgument { get; set; }

		[DataMember]
		public Argument GArgument { get; set; }

		[DataMember]
		public Argument BArgument { get; set; }

		[DataMember]
		public Argument ResultArgument { get; set; }
	}
}