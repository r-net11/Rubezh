using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
using System.Windows;
using ChinaSKDDriverNativeApi;
using ChinaSKDDriverAPI;
using System.Collections.ObjectModel;

namespace ControllerSDK.ViewModels
{
	public class LogItemsViewModel : BaseViewModel
	{
		public LogItemsViewModel()
		{
			GetLogsCountCommand = new RelayCommand(OnGetLogsCount);
			GetAllLogsCommand = new RelayCommand(OnGetAllLogs);
		}

		public RelayCommand GetLogsCountCommand { get; private set; }
		void OnGetLogsCount()
		{
			MessageBox.Show("Всего тревог: " + MainViewModel.Wrapper.GetLogsCount());
		}

		ObservableCollection<LogItem> _logs;
		public ObservableCollection<LogItem> Logs
		{
			get { return _logs; }
			set
			{
				_logs = value;
				OnPropertyChanged(() => Logs);
			}
		}

		public RelayCommand GetAllLogsCommand { get; private set; }
		void OnGetAllLogs()
		{
			Logs = new ObservableCollection<LogItem>();
			var logs = MainViewModel.Wrapper.GetAllLogs();
			foreach (var log in logs)
			{
				Logs.Add(log);
			}
		}
	}
}