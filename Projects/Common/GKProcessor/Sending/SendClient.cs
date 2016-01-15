using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GKProcessor
{
	public class SendClient
	{
		public string IpAddress { get; set; }
		public DateTime ExecuteTime { get; set; }
		public object locker;

		public SendClient(string ipAddress)
		{
			IpAddress = ipAddress;
			ExecuteTime = DateTime.Now;
			locker = new object();
		}
	}
}
