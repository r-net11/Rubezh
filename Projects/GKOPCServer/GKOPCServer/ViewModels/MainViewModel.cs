using System;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using Infrastructure.Common.Windows.Windows;

namespace GKOPCServer.ViewModels
{
	public class MainViewModel : ApplicationViewModel
	{
		public static MainViewModel Current { get; private set; }

		public MainViewModel()
		{
			Current = this;
			Title = "OPC Сервер Рубеж Глобал";
			RegisterCommand = new RelayCommand(OnRegister);
		}

		public void AddLog(string message)
		{
			ApplicationService.BeginInvoke(new Action(
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
				OnPropertyChanged(() => Log);
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