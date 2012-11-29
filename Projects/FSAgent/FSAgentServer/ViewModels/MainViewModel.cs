using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;

namespace FSAgentServer.ViewModels
{
	public class MainViewModel : ApplicationViewModel
	{
		public static MainViewModel Current { get; private set; }

		public MainViewModel()
		{
			Current = this;
			Title = "Агент FireSec-2";
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

		public void AddLog(string message, bool isError = false)
		{
			Dispatcher.BeginInvoke(new Action(
			delegate()
			{
				LastLog = message;
				if (isError)
				{
					ErrorLog += message + "\n";
				}
				else
				{
					InfoLog += message + "\n";
				}
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

		string _errorLog = "";
		public string ErrorLog
		{
			get { return _errorLog; }
			set
			{
				_errorLog = value;
				OnPropertyChanged("ErrorLog");
			}
		}

		public override bool OnClosing(bool isCanceled)
		{
			Minimize();
			return true;
		}
	}
}