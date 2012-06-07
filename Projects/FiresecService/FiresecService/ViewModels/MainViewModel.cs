using System;
using System.Collections.ObjectModel;
using System.Linq;
using Infrastructure.Common;
using FiresecService.Service;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Windows;

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
			Title = "Сервер ОПС Firesec-2";
		}
		public RelayCommand ShowImitatorCommand { get; private set; }
		void OnShowImitator()
		{
			foreach (var connection in Clients)
			{
				if (connection.ClientType == ClientType.Itv)
				{
					var imitatorViewModel = new ImitatorViewModel(connection.FiresecService);
					DialogService.ShowModalWindow(imitatorViewModel);
					break;
				}
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
			Dispatcher.Invoke(new Action(
			delegate()
			{
				var connectionViewModel = new ClientViewModel()
				{
					FiresecService = firesecService,
					UID = firesecService.UID,
					UserName = firesecService.ClientCredentials.UserName,
					ClientType = firesecService.ClientCredentials.ClientType,
					IpAddress = firesecService.ClientIpAddressAndPort,
					ConnectionDate = DateTime.Now
				};
				Clients.Add(connectionViewModel);
			}
			));
		}
		public void RemoveClient(Guid uid)
		{
			Dispatcher.Invoke(new Action(
			delegate()
			{
				var connectionViewModel = MainViewModel.Current.Clients.FirstOrDefault(x => x.UID == uid);
				Clients.Remove(connectionViewModel);
			}
			));
		}
		public void EditClient(Guid uid, string userName)
		{
			Dispatcher.Invoke(new Action(
			delegate()
			{
				var connectionViewModel = MainViewModel.Current.Clients.FirstOrDefault(x => x.UID == uid);
				connectionViewModel.ConnectionDate = DateTime.Now;
				connectionViewModel.UserName = userName;
			}
			));
		}
		public void UpdateClientOperation(Guid uid, string operationName)
		{
			Dispatcher.Invoke(new Action(
			delegate()
			{
				var connectionViewModel = MainViewModel.Current.Clients.FirstOrDefault(x => x.UID == uid);
				if (connectionViewModel != null)
				{
					connectionViewModel.CurrentOperationName = operationName;
				}
			}
			));
		}

		public void UpdateClientState(Guid uid, string state)
		{
			Dispatcher.Invoke(new Action(
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

		public override bool OnClosing(bool isCanceled)
		{
			Minimize();
			return true;
		}
	}
}