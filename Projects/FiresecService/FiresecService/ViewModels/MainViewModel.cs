using System;
using System.Collections.ObjectModel;
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

            ConnectCommand = new RelayCommand(OnConnect);
            ShowImitatorCommand = new RelayCommand(OnShowImitator);
            ConvertConfigurationCommand = new RelayCommand(OnConvertConfiguration);
            ConvertJournalCommand = new RelayCommand(OnConvertConfiguration);

            Connections = new ObservableCollection<ConnectionViewModel>();
        }

        public RelayCommand ConnectCommand { get; private set; }
        void OnConnect()
        {
            FiresecManager.ConnectFiresecCOMServer("adm", "");
            FiresecServiceManager.Open();
        }

        public RelayCommand ShowImitatorCommand { get; private set; }
        void OnShowImitator()
        {
            UserDialogService.ShowModalWindow(new ImitatorViewModel());
        }

        public RelayCommand ConvertConfigurationCommand { get; private set; }
        void OnConvertConfiguration()
        {
            ConfigurationConverter.Convert();
        }

        public RelayCommand ConvertJournalCommand { get; private set; }
        void OnConvertJournal()
        {
            JournalDataConverter.Convert();
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
