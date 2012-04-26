using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Configuration;
using FiresecService.Infrastructure;
using Infrastructure.Common;

namespace FiresecService.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public static MainViewModel Current { get; private set; }

        public MainViewModel()
        {
            Current = this;

            ShowImitatorCommand = new RelayCommand(OnShowImitator);
            ReloadCommand = new RelayCommand(OnReload);
            Connections = new ObservableCollection<ConnectionViewModel>();

            Start();
        }

        void Start()
        {
            //FiresecServiceManager.Open();
        }

        public RelayCommand ReloadCommand { get; private set; }
        void OnReload()
        {
            //FiresecServiceManager.Close();

            Connections = new ObservableCollection<ConnectionViewModel>();
            //Start();
        }

        public RelayCommand ShowImitatorCommand { get; private set; }
        void OnShowImitator()
        {
            UserDialogService.ShowModalWindow(new ImitatorViewModel(null));
        }

        public string FiresecConnectionSatus
        {
            get
            {
                //if (FiresecManager.IsConnected)
				if (true)
                    return "Соединение с сервером Firesec установленно";
                else
                    return "Соединение с сервером Firesec НЕ установленно";
            }
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

        public void AddConnection(Guid uid, string userLogin, string userIpAddress, string clientType)
        {
            var connectionViewModel = new ConnectionViewModel()
            {
                UID = uid,
                UserName = userLogin,
                IpAddress = userIpAddress,
                ClientType = clientType,
                ConnectionDate = DateTime.Now
            };
            Connections.Add(connectionViewModel);
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
    }
}