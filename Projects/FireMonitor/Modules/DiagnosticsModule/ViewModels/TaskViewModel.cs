using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Models;

namespace DiagnosticsModule.ViewModels
{
	public class TaskViewModel : BaseViewModel
	{
		public ServerTask ServerTask { get; private set; }

		public TaskViewModel(ServerTask serverTask)
		{
			ServerTask = serverTask;
			Name = serverTask.Name;
		}

		public string Name { get; private set; }
	}
}