using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FS2Api;

namespace FS2Client
{
	public partial class FS2ClientContract : IFS2Contract
	{
		FS2Factory FS2Factory;
		public IFS2Contract FS2Contract { get; set; }
		string _serverAddress;

		public FS2ClientContract(string serverAddress)
		{
			_serverAddress = serverAddress;
			FS2Factory = new FS2Factory();
			FS2Contract = FS2Factory.Create(serverAddress);
		}
	}
}