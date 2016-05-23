using System.Collections.Generic;

namespace RubezhService.Models
{
	static class OperationsModel
	{
		public static List<Operation> Operations { get; private set; }
		static OperationsModel()
		{
			Operations = new List<Operation>();
		}
		public static void AddServerTask(ServerTask serverTask)
		{
			Operations.Add(new Operation(serverTask));
            // TODO: Notify
        }
        public static void RemoveServerTask(ServerTask serverTask)
		{
			Operations.RemoveAll(x => x.ServerTask == serverTask);
            // TODO: Notify
        }
    }

	public class Operation
	{
		public string Name
		{
			get
			{
				return ServerTask == null ? "" : ServerTask.Name;
			}
		}
		public ServerTask ServerTask { get; private set; }
		public Operation(ServerTask serverTask)
		{
			ServerTask = serverTask;
		}
	}
}
