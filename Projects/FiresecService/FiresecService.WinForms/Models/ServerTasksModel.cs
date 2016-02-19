using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using FiresecService;

namespace FiresecService.Models
{
	public class ServerTasksModel
	{
		Dispatcher _dispatcher;

		public ServerTasksModel()
		{
			_dispatcher = Dispatcher.CurrentDispatcher;
			ServerTaskList = new List<ServerTaskModel>();
		}

		public List<ServerTaskModel> ServerTaskList { get; private set; }

		public void Add(ServerTask serverTask)
		{
			_dispatcher.BeginInvoke((Action)(() =>
			{
				var serverTaskViewModel = new ServerTaskModel(serverTask);
				ServerTaskList.Add(serverTaskViewModel);
			}));
		}
		public void Remove(ServerTask serverTask)
		{
			_dispatcher.BeginInvoke((Action)(() =>
			{
				var serverTaskViewModel = ServerTaskList.FirstOrDefault(x => x.Task.UID == serverTask.UID);
				if (serverTaskViewModel != null)
					ServerTaskList.Remove(serverTaskViewModel);
			}));
		}
		public void Edit(ServerTask serverTask)
		{
			_dispatcher.BeginInvoke((Action)(() =>
			{
				var serverTaskViewModel = ServerTaskList.FirstOrDefault(x => x.Task.UID == serverTask.UID);
				if (serverTaskViewModel != null)
					serverTaskViewModel.Task = serverTask;
			}));
		}
	}
}