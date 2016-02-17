using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecService;

namespace FiresecService.Models
{
	public class ServerTaskModel
	{
		public ServerTask Task { get; set; }

		public ServerTaskModel(ServerTask serverTask)
		{
			Task = serverTask;
		}

		public string Name { get; private set; }
	}
}
