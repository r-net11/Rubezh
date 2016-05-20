using System.Collections.Generic;

namespace RubezhService
{
	static class OperationsPresenter
	{
		public static List<Operation> Operations { get; private set; }
		static OperationsPresenter()
		{
			Operations = new List<Operation>();
		}
		public static void AddServerTask(ServerTask serverTask)
		{
			Operations.Add(new Operation(serverTask));
			PageController.OnPageChanged(Page.Operations);
		}
		public static void RemoveServerTask(ServerTask serverTask)
		{
			Operations.RemoveAll(x => x.ServerTask == serverTask);
			PageController.OnPageChanged(Page.Operations);
		}
	}

	class Operation
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
