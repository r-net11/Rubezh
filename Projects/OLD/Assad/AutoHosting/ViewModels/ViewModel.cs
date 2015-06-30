using System;
using AssadProcessor;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace AutoHosting
{
	public class ViewModel : BaseViewModel
	{
		Controller Controller;

		public ViewModel()
		{
			StartCommand = new RelayCommand(OnStart);
			StopCommand = new RelayCommand(OnStop);
			TestCommand = new RelayCommand(OnTest);
		}

		int _commandCount = 0;
		void MessageProcessor_NewMessage(string message)
		{
			LastCommand = (_commandCount++).ToString() + " - " + message;
		}

		string _status = "None";
		public string Status
		{
			get { return _status; }
			set
			{
				_status = value;
				OnPropertyChanged("Status");
			}
		}

		string _lastCommand;
		public string LastCommand
		{
			get { return _lastCommand; }
			set
			{
				_lastCommand = value;
				OnPropertyChanged("LastCommand");
			}
		}

		public RelayCommand StartCommand { get; private set; }
		void OnStart()
		{
			Controller = new Controller();
			Controller.Start();
			Status = "Running";
			MessageProcessor.NewMessage += new Action<string>(MessageProcessor_NewMessage);
		}

		public RelayCommand StopCommand { get; private set; }
		void OnStop()
		{
			if (Controller != null)
			{
				Controller.Stop();
				Controller = null;
			}
			Status = "Stopped";
		}

		public RelayCommand TestCommand { get; private set; }
		void OnTest()
		{
		}
	}
}