using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;

namespace ServerFS2.Monitor
{
	public class CommandItem
	{
		Device Device { get; set; }
		string CommandName { get; set; }

		public CommandItem(Device device, string commandName)
		{
			Device = device;
			CommandName = commandName;
		}

		public void Execute()
		{
			ServerHelper.ExecuteCommand(Device, CommandName);
		}
	}
}