using System;
using System.Collections.ObjectModel;
using System.Linq;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;
using FiresecAPI.Models;
using Infrastructure.Common.BalloonTrayTip;

namespace FiresecService.ViewModels
{
	public class MainViewModel : ApplicationViewModel
	{
		public static MainViewModel Current { get; private set; }

		public MainViewModel()
		{
			Current = this;
			Title = "Сервер приложений ОПС FireSec-2";
			Clients = new ObservableCollection<ClientViewModel>();
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

		public ObservableCollection<ClientViewModel> Clients { get; private set; }

		ClientViewModel _selectedClient;
		public ClientViewModel SelectedClient
		{
			get { return _selectedClient; }
			set
			{
				_selectedClient = value;
				OnPropertyChanged("SelectedClient");
			}
		}

		public void AddClient(ClientCredentials clientCredentials)
		{
			Dispatcher.BeginInvoke(new Action(
			delegate()
			{
				var connectionViewModel = new ClientViewModel(clientCredentials);
				Clients.Add(connectionViewModel);
			}
			));
		}
		public void RemoveClient(Guid uid)
		{
			Dispatcher.BeginInvoke(new Action(
			delegate()
			{
				var connectionViewModel = Clients.FirstOrDefault(x => x.UID == uid);
				if (connectionViewModel != null)
				{
					Clients.Remove(connectionViewModel);
				}
			}
			));
		}
		public void EditClient(Guid uid, string userName)
		{
			Dispatcher.BeginInvoke(new Action(
			delegate()
			{
				var connectionViewModel = Clients.FirstOrDefault(x => x.UID == uid);
				if (connectionViewModel != null)
				{
					connectionViewModel.FriendlyUserName = userName;
				}
			}
			));
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
			ApplicationMinimizeCommand.Execute();
			return true;
		}
	}
}