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
			ReloadCommand = new RelayCommand(OnReload);
			Connections = new ObservableCollection<ConnectionViewModel>();
			Title = "Сервер ОПС Firesec-2";
		}

		public RelayCommand ReloadCommand { get; private set; }
		void OnReload()
		{
			Connections = new ObservableCollection<ConnectionViewModel>();
		}
		public RelayCommand ShowImitatorCommand { get; private set; }
		void OnShowImitator()
		{
			foreach (var connection in Connections)
			{
				if (connection.ClientType == "ITV")
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
		public string ComServersSatus { get; set; }
		public void UpdateComServersCount()
		{
			ComServersSatus = "Свободных: " + ServiceCash.FreeManagers.Count.ToString() + "\n";
			ComServersSatus += "Работающих: " + ServiceCash.RunningManagers.Count.ToString();
			OnPropertyChanged("ComServersSatus");
		}

		public bool IsDebug
		{
			get { return AppSettings.IsDebug; }
		}

		public ObservableCollection<ConnectionViewModel> Connections { get; private set; }

		ConnectionViewModel _selectedConnection;
		public ConnectionViewModel SelectedConnection
		{
			get { return _selectedConnection; }
			set
			{
				_selectedConnection = value;
				OnPropertyChanged("SelectedConnection");
			}
		}

		public void AddConnection(FiresecService.Service.FiresecService firesecService, Guid uid, string userLogin, string userIpAddress, string clientType)
		{
			Dispatcher.Invoke(new Action(
			delegate()
			{
				var connectionViewModel = new ConnectionViewModel()
				{
					FiresecService = firesecService,
					UID = uid,
					UserName = userLogin,
					IpAddress = userIpAddress,
					ClientType = clientType,
					ConnectionDate = DateTime.Now
				};
				Connections.Add(connectionViewModel);
			}
			));
		}
		public void RemoveConnection(Guid uid)
		{
			Dispatcher.Invoke(new Action(
			delegate()
			{
				var connectionViewModel = MainViewModel.Current.Connections.FirstOrDefault(x => x.UID == uid);
				Connections.Remove(connectionViewModel);
			}
			));
		}
		public void EditConnection(Guid uid, string userName)
		{
			Dispatcher.Invoke(new Action(
			delegate()
			{
				var connectionViewModel = MainViewModel.Current.Connections.FirstOrDefault(x => x.UID == uid);
				connectionViewModel.ConnectionDate = DateTime.Now;
				connectionViewModel.UserName = userName;
			}
			));
		}
		public void UpdateCurrentOperationName(Guid uid, string operationName)
		{
			Dispatcher.Invoke(new Action(
			delegate()
			{
				var connectionViewModel = MainViewModel.Current.Connections.FirstOrDefault(x => x.UID == uid);
				if (connectionViewModel != null)
				{
					connectionViewModel.CurrentOperationName = operationName;
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