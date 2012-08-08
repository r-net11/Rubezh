using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using System.ServiceModel;
using FiresecService.Service;

namespace FiresecService.ViewModels
{
	public class MainViewModel : ApplicationViewModel
	{
		public static MainViewModel Current { get; private set; }

		public MainViewModel()
		{
			Current = this;
			ShowImitatorCommand = new RelayCommand(OnShowImitator);
			Clients = new ObservableCollection<ClientViewModel>();
			Title = "Сервер ОПС FireSec-2";
		}

		public RelayCommand ShowImitatorCommand { get; private set; }
		void OnShowImitator()
		{
			ClientsCash.NotifyClients("Запущен имитатор");
			var imitatorViewModel1 = new ImitatorViewModel();
			DialogService.ShowModalWindow(imitatorViewModel1);
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

		public bool IsImitatorVisible
		{
			get { return AppSettings.IsImitatorVisible || AppSettings.IsDebug; }
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
				var endpointAddress = new EndpointAddress(new Uri(firesecService.ClientCredentials.ClientCallbackAddress));
				var port = endpointAddress.Uri.Port;
				var connectionViewModel = new ClientViewModel()
				{
					FiresecService = firesecService,
					UID = firesecService.UID,
					UserName = firesecService.ClientCredentials.UserName,
					ClientType = firesecService.ClientCredentials.ClientType,
					IpAddress = firesecService.ClientIpAddressAndPort,
					CallbackPort = port,
					ConnectionDate = DateTime.Now
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
				connectionViewModel.ConnectionDate = DateTime.Now;
				connectionViewModel.UserName = userName;
			}
			));
		}

		public void BeginAddOperation(Guid uid, OperationDirection operationDirection, string operationName)
		{
			Dispatcher.BeginInvoke(new Action(
			delegate()
			{
				var connectionViewModel = MainViewModel.Current.Clients.FirstOrDefault(x => x.UID == uid);
				if (connectionViewModel != null)
				{
					connectionViewModel.BeginAddOperation(operationDirection, operationName);
				}
			}
			));
		}

		public void EndAddOperation(Guid uid, OperationDirection operationDirection)
		{
			Dispatcher.BeginInvoke(new Action(
			delegate()
			{
				var connectionViewModel = MainViewModel.Current.Clients.FirstOrDefault(x => x.UID == uid);
				if (connectionViewModel != null)
				{
					connectionViewModel.EndAddOperation(operationDirection);
				}
			}
			));
		}

		public void UpdateClientState(Guid uid, string state)
		{
			Dispatcher.BeginInvoke(new Action(
			delegate()
			{
				var connectionViewModel = MainViewModel.Current.Clients.FirstOrDefault(x => x.UID == uid);
				if (connectionViewModel != null)
				{
					connectionViewModel.State = state;
				}
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