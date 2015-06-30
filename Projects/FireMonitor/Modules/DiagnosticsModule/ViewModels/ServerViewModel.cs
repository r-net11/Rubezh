using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace DiagnosticsModule.ViewModels
{
	public class ServerViewModel : ViewPartViewModel
	{
		public ServerViewModel()
		{
			RefreshCommand = new RelayCommand(OnRefresh);
			OnRefresh();
		}

		ObservableCollection<TaskViewModel> _tasks;
		public ObservableCollection<TaskViewModel> Tasks
		{
			get { return _tasks; }
			set
			{
				_tasks = value;
				OnPropertyChanged(() => Tasks);
			}
		}

		TaskViewModel _selectedTask;
		public TaskViewModel SelectedTask
		{
			get { return _selectedTask; }
			set
			{
				_selectedTask = value;
				OnPropertyChanged(() => SelectedTask);
			}
		}

		public RelayCommand RefreshCommand { get; private set; }
		void OnRefresh()
		{
			Tasks = new ObservableCollection<TaskViewModel>();
			var serverTasks = FiresecManager.FiresecService.GetServerTasks();
			if(serverTasks != null)
				foreach (var serverTask in serverTasks)
				{
					var taskViewModel = new TaskViewModel(serverTask);
					Tasks.Add(taskViewModel);
				}
			SelectedTask = Tasks.FirstOrDefault();
		}
	}
}