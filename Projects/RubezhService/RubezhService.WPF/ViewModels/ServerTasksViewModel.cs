using Infrastructure.Common.Windows.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace RubezhService.Models
{
	public class ServerTasksViewModel : BaseViewModel
	{
		Dispatcher _dispatcher;

		public ServerTasksViewModel()
		{
			_dispatcher = Dispatcher.CurrentDispatcher;
			ServerTasks = new ObservableCollection<ServerTaskViewModel>();
		}

		public ObservableCollection<ServerTaskViewModel> ServerTasks { get; private set; }

		ServerTaskViewModel _selectedServerTask;
		public ServerTaskViewModel SelectedServerTask
		{
			get { return _selectedServerTask; }
			set
			{
				_selectedServerTask = value;
				OnPropertyChanged(() => SelectedServerTask);
			}
		}

		public void Add(ServerTask serverTask)
		{
			_dispatcher.BeginInvoke((Action)(() =>
			{
				var serverTaskViewModel = new ServerTaskViewModel(serverTask);
				ServerTasks.Add(serverTaskViewModel);
			}));
		}
		public void Remove(ServerTask serverTask)
		{
			_dispatcher.BeginInvoke((Action)(() =>
			{
				var serverTaskViewModel = ServerTasks.FirstOrDefault(x => x.ServerTask.UID == serverTask.UID);
				if (serverTaskViewModel != null)
					ServerTasks.Remove(serverTaskViewModel);
			}));
		}
		public void Edit(ServerTask serverTask)
		{
			_dispatcher.BeginInvoke((Action)(() =>
			{
				var serverTaskViewModel = ServerTasks.FirstOrDefault(x => x.ServerTask.UID == serverTask.UID);
				if (serverTaskViewModel != null)
					serverTaskViewModel.ServerTask = serverTask;
			}));
		}
	}

	public class ServerTaskViewModel : BaseViewModel
	{
		public ServerTask ServerTask { get; set; }

		public ServerTaskViewModel(ServerTask serverTask)
		{
			ServerTask = serverTask;
		}

		public string Name { get; private set; }
	}
}