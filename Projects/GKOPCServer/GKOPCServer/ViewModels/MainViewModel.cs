using System;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace GKOPCServer.ViewModels
{
	public class MainViewModel : ApplicationViewModel
	{
		public static MainViewModel Current { get; private set; }

		public MainViewModel()
		{
			Current = this;
			Title = "OPC Сервер ГК ОПС FireSec";
			RegisterCommand = new RelayCommand(OnRegister);
		}

		public void AddLog(string message)
		{
			Dispatcher.BeginInvoke(new Action(
			delegate()
			{
				Log += message + "\n";
			}
			));
		}

		string _log = "";
		public string Log
		{
			get { return _log; }
			set
			{
				_log = value;
				OnPropertyChanged("Log");
			}
		}

		public RelayCommand RegisterCommand { get; private set; }
		void OnRegister()
		{
			GKOPCManager.OPCRegister();
		}

		public override bool OnClosing(bool isCanceled)
		{
			ApplicationMinimizeCommand.ForceExecute();
			return true;
		}
	}
}