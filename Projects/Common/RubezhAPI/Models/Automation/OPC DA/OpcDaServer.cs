using System;
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
		}

		[DataMember]
		public string ServerName { get; set; }

		[DataMember]
		public Guid Id { get; set; }
	}
}
