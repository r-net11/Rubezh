using System;
using System.Collections.ObjectModel;
using FiresecService.Infrastructure;
using Infrastructure.Common;
using System.Windows;

namespace FiresecService.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public static MainViewModel Current { get; private set; }
        public MainViewModel()
        {
            Current = this;

            ConvertConfigurationCommand = new RelayCommand(OnConvertConfiguration);
            ConvertJournalCommand = new RelayCommand(OnConvertConfiguration);
            ShowImitatorCommand = new RelayCommand(OnShowImitator);

            Connections = new ObservableCollection<ConnectionViewModel>();

            Start();
        }

        void Start()
        {
            FiresecManager.ConnectFiresecCOMServer("adm", "");
            FiresecServiceManager.Open();
        }

        public RelayCommand ConvertConfigurationCommand { get; private set; }
        void OnConvertConfiguration()
        {
            if (Connections.Count > 0)
            {
                MessageBox.Show("Один или несколько клиентов соединены с сервером. Клиенты должны быть отсоединены");
                return;
            }
            if (MessageBox.Show("Вы уверены, что хотите конвертировать конфигурацию?", "Вы уверены, что хотите конвертировать конфигурацию?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                ConfigurationConverter.Convert();
                Start();
            }
        }

        public RelayCommand ConvertJournalCommand { get; private set; }
        void OnConvertJournal()
        {
            if (Connections.Count > 0)
            {
                MessageBox.Show("Один или несколько клиентов соединены с сервером. Клиенты должны быть отсоединены");
                return;
            }
            if (MessageBox.Show("Вы уверены, что хотите конвертировать журнал событий?", "Вы уверены, что хотите конвертировать журнал событий?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                JournalDataConverter.Convert();
                Start();
            }
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