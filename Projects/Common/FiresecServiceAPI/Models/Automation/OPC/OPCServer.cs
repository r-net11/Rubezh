using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class OPCServer
	{
		public OPCServer()
		{
			Uid = Guid.NewGuid();
			Name = "Название OPC Сервера";
		}

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Address { get; set; }

		[DataMember]
		public Guid Uid { get; set; }
	}
}