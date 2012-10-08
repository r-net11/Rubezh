using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using Infrastructure.Common.Windows.ViewModels;

namespace FiresecService.ViewModels
{
	public class MainViewModel : ApplicationViewModel
	{
		public static MainViewModel Current { get; private set; }

		public MainViewModel()
		{
			Current = this;
			Clients = new ObservableCollection<ClientViewModel>();
			Title = "Сервер ОПС FireSec-2";
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

		public bool IsDebug
		{
			get { return AppSettings.IsDebug; }
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

		public void AddClient(FiresecService.Service.FiresecService firesecService)
		{
			Dispatcher.BeginInvoke(new Action(
			delegate()
			{
				var connectionViewModel = new ClientViewModel()
				{
					FiresecService = firesecService,
					UID = firesecService.UID,
					UserName = firesecService.ClientCredentials.UserName,
					IpAddress = firesecService.ClientIpAddressAndPort,
				};
				Clients.Add(connectionViewModel);
			}
			));
		}
		public void RemoveClient(Guid uid)
		{
			Dispatcher.BeginInvoke(new Action(
			delegate()
			{
				var connectionViewModel = MainViewModel.Current.Clients.FirstOrDefault(x => x.UID == uid);
				Clients.Remove(connectionViewModel);
			}
			));
		}
		public void EditClient(Guid uid, string userName)
		{
			Dispatcher.BeginInvoke(new Action(
			delegate()
			{
				var connectionViewModel = MainViewModel.Current.Clients.FirstOrDefault(x => x.UID == uid);
				connectionViewModel.UserName = userName;
			}
			));
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