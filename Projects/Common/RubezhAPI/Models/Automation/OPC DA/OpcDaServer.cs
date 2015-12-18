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
			Tags = new OpcDaTag[0];
		}

		[DataMember]
		public string ServerName { get; set; }

		[DataMember]
		public Guid Uid { get; set; }

		/// <summary>
		/// Путь к хост-компьютеру на котором установлен данный сервер 
		/// </summary>
		[DataMember]
		public string Url { get; set; }

		[DataMember]
		public string Login { get; set; }

		[DataMember]
		public string Password { get; set; }

		[DataMember]
		public OpcDaTag[] Tags { get; set; }

		public bool IsChecked { get; set; }
	}
}
