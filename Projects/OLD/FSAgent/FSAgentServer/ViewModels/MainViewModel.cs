using System;
using Infrastructure.Common.Windows.ViewModels;

namespace FSAgentServer.ViewModels
{
	public class MainViewModel : ApplicationViewModel
	{
		public static MainViewModel Current { get; private set; }

		public MainViewModel()
		{
			Current = this;
			Title = "Драйвер ОПС FireSec";
		}

		private string _status;
		public string Satus
		{
			get { return _status; }
			set
			{
				_status = value;
				OnPropertyChanged("Status");
			}
		}

		public void AddLog(string message)
		{
			Dispatcher.BeginInvoke(new Action(
			delegate()
			{
				LastLog = message;
				InfoLog += message + "\n";
			}
			));
		}

		string _lastLog = "";
		public string LastLog
		{
			get { return _lastLog; }
			set
			{
				_lastLog = value;
				OnPropertyChanged("LastLog");
			}
		}

		string _infoLog = "";
		public string InfoLog
		{
			get { return _infoLog; }
			set
			{
				_infoLog = value;
				OnPropertyChanged("InfoLog");
			}
		}

		public override bool OnClosing(bool isCanceled)
		{
			ApplicationMinimizeCommand.ForceExecute();
			return true;
		}
	}
}