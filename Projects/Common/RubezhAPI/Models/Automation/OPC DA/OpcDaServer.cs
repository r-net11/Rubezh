using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class OpcDaServer
	{
		public OpcDaServer()
		{
			Id = Guid.NewGuid();
			ServerName = "Название OPC DA сервера";
			Tags = new OpcDaTag[0];
		}

		[DataMember]
		public string ServerName { get; set; }

		[DataMember]
		public Guid Id { get; set; }

		[DataMember]
		public OpcDaTag[] Tags { get; set; }
	}
}
