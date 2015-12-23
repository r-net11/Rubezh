﻿using System.ComponentModel;
using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class HttpRequestArguments
	{
		public HttpRequestArguments()
		{
			UrlArgument = new Argument();
			ContentArgument = new Argument();
			ResponseArgument = new Argument();
		}

		[DataMember]
		public HttpMethod HttpMethod { get; set; }

		[DataMember]
		public Argument UrlArgument { get; set; }

		[DataMember]
		public Argument ContentArgument { get; set; }

		[DataMember]
		public Argument ResponseArgument { get; set; }
	}

	public enum HttpMethod
	{
		[Description("GET")]
		Get,

		[Description("POST")]
		Post
	}
}