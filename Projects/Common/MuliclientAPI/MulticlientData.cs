using System;
using System.Runtime.Serialization;

namespace MuliclientAPI
{
	[Serializable]
	[DataContract]
	public class MulticlientData
	{
		public MulticlientData()
		{
			Name = "Название сервера";
			Address = "localhost";
			Port = 8000;
			Login = "adm";
			Password = "";
		}

		public string Id { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Address { get; set; }

		[DataMember]
		public int Port { get; set; }

		[DataMember]
		public string Login { get; set; }

		[DataMember]
		public string Password { get; set; }

		[DataMember]
		public bool IsNotUsed { get; set; }
	}
}