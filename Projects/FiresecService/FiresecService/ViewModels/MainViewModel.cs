using System;
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
            FiresecInternalClient.Disconnect();

            string oldFiresecLogin = ConfigurationManager.AppSettings["OldFiresecLogin"] as string;
            string oldFiresecPassword = ConfigurationManager.AppSettings["OldFiresecPassword"] as string;
            FiresecManager.ConnectFiresecCOMServer(oldFiresecLogin, oldFiresecPassword);
            FiresecServiceManager.Open();
        }

        public RelayCommand ReloadCommand { get; private set; }
        void OnReload()
        {
            FiresecInternalClient.Disconnect();
            FiresecServiceManager.Close();

            Connections = new ObservableCollection<ConnectionViewModel>();
            Start();
        }

        public RelayCommand ShowImitatorCommand { get; private set; }
        void OnShowImitator()
        {
            UserDialogService.ShowModalWindow(new ImitatorViewModel());
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

        public void RemoveConnection(ConnectionViewModel connectionViewModel)
        {
            Dispatcher.Invoke(new Action(
            delegate()
            {
                Connections.Remove(connectionViewModel);
            }
            ));
        }
    }
}