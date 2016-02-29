using System;
using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class OPCServer
	{
		public OPCServer()
		{
			Uid = Guid.NewGuid();
			Name = "Название OPC Сервера";
			Address = "opc.tcp://localhost:51510/UA/DemoServer";
		}

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Address { get; set; }

		[DataMember]
		public Guid Uid { get; set; }
	}
}