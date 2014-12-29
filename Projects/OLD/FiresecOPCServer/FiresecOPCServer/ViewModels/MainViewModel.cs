using System;
using Firesec.Imitator;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace FiresecOPCServer.ViewModels
{
	public class MainViewModel : ApplicationViewModel
	{
		public static MainViewModel Current { get; private set; }

		public MainViewModel()
		{
			Current = this;
			Title = "OPC Сервер ОПС FireSec";
			ShowImitatorCommand = new RelayCommand(OnShowImitator);
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

		public RelayCommand ShowImitatorCommand { get; private set; }
		void OnShowImitator()
		{
			ImitatorService.Show();
		}

		public RelayCommand RegisterCommand { get; private set; }
		void OnRegister()
		{
			FiresecOPCManager.OPCRegister();
		}

		public override bool OnClosing(bool isCanceled)
		{
			ApplicationMinimizeCommand.ForceExecute();
			return true;
		}
	}
}