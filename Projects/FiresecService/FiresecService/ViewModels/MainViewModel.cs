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
		public string HostStatus { get; private set; }
		public string ComServersStatus { get; private set; }
		public string CurrentStatus { get; private set; }

		public MainViewModel()
		{
			Current = this;
			ShowOperationHistoryCommand = new RelayCommand(OnShowOperationHistory);
			ShowImitatorCommand = new RelayCommand(OnShowImitator);
			Clients = new ObservableCollection<ClientViewModel>();
			Title = "Сервер ОПС FireSec-2";
			ComServersStatus = "устанавливается";
			HostStatus = "устанавливается";
		}

		public RelayCommand ShowImitatorCommand { get; private set; }
		void OnShowImitator()
		{
			//var firesecService = Clients[0].FiresecService;
			ClientsCash.NotifyClients("Запущен имитатор");
			var imitatorViewModel1 = new ImitatorViewModel();
			DialogService.ShowModalWindow(imitatorViewModel1);
			return;

			foreach (var connection in Clients)
				if (connection.ClientType == ClientType.Itv)
				{
					var imitatorViewModel = new ImitatorViewModel();
					DialogService.ShowModalWindow(imitatorViewModel);
					break;
				}
		}

		public RelayCommand ShowOperationHistoryCommand { get; private set; }
		void OnShowOperationHistory()
		{
			if (SelectedClient != null)
			{
				var operationHistoryViewModel = new OperationHistoryViewModel(SelectedClient);
				DialogService.ShowModalWindow(operationHistoryViewModel);
			}
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

		public bool CanShowImitator
		{
			get { return true; }
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

		public void UpdateStatus(string hostStatus, string comServersStatus)
		{
			Dispatcher.BeginInvoke(new Action(
			delegate()
			{
				HostStatus = hostStatus;
				ComServersStatus = comServersStatus;
				OnPropertyChanged("HostStatus");
				OnPropertyChanged("ComServersStatus");
			}
			));
		}

		public void UpdateCurrentStatus(string currentStatus)
		{
			Dispatcher.BeginInvoke(new Action(
			delegate()
			{
				CurrentStatus = currentStatus;
				OnPropertyChanged("CurrentStatus");
			}
			));
		}

		public override bool OnClosing(bool isCanceled)
		{
			Minimize();
			return true;
		}
	}
}