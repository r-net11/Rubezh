using FiresecAPI.Models;

namespace ServerFS2.Monitoring
{
	public class CommandItem
	{
		
		Device Device;
		string CommandName;
		

		public CommandItem(Device device, string commandName)
		{
			Device = device;
			CommandName = commandName;
		}

		public void Send()
		{
			ServerHelper.ExecuteCommand(Device, CommandName);
		}
	}
}