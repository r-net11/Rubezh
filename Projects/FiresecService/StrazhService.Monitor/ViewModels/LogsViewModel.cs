using System;
using Infrastructure.Common.Windows.ViewModels;
using StrazhService.Monitor.Events;

namespace StrazhService.Monitor.ViewModels
{
	public class LogsViewModel : BaseViewModel
	{
		private string _log;

		public string Log
		{
			get { return _log; }
			set
			{
				if (_log == value)
					return;
				_log = value;
				OnPropertyChanged(() => Log);
			}
		}

		public LogsViewModel()
		{
			ServiceRepository.Instance.Events.GetEvent<ServerLogsReceivedEvent>().Unsubscribe(OnServerLogsReceivedEvent);
			ServiceRepository.Instance.Events.GetEvent<ServerLogsReceivedEvent>().Subscribe(OnServerLogsReceivedEvent);
		}

		private void OnServerLogsReceivedEvent(string logs)
		{
			Log = logs;
		}
	}
}