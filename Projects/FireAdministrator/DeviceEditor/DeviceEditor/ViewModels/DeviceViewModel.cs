using System.Collections.ObjectModel;
using System.Windows.Controls;
using Common;

namespace DeviceEditor
{
    public class DeviceViewModel : BaseViewModel
    {
        /// <summary>
        /// Путь к иконке устройства
        /// </summary>
        private string iconPath;

        /// <summary>
        /// Идентификатор устройства.
        /// </summary>
        public string id;

        /// <summary>
        /// Выбранное состояние.
        /// </summary>
        private StateViewModel selectedStateViewModel;

        public ObservableCollection<Canvas> statesPicture;

        /// <summary>
        /// Список всех используемых состояний для данного устройства
        /// </summary>
        public ObservableCollection<StateViewModel> statesViewModel;

        public DeviceViewModel()
        {
            Current = this;
            AddDeviceCommand = new RelayCommand(OnAddDeviceCommand);
            RemoveDeviceCommand = new RelayCommand(OnRemoveDeviceCommand);
            AddStateCommand = new RelayCommand(OnAddStateCommand);
            StatesPicture = new ObservableCollection<Canvas>();
        }

        public static DeviceViewModel Current { get; private set; }


        public string IconPath
        {
            get { return iconPath; }
            set
            {
                iconPath = value;
                OnPropertyChanged("IconPath");
            }
        }

        /// <summary>
        /// Команда, показывающая список всех доступных устройств.
        /// </summary>
        public RelayCommand AddDeviceCommand { get; private set; }

        /// <summary>
        /// Команда, показывающая список всех доступных состояний.
        /// </summary>
        public RelayCommand AddStateCommand { get; private set; }

        /// <summary>
        /// Команда удаляющая устройство из списка.
        /// </summary>
        public RelayCommand RemoveDeviceCommand { get; private set; }

        public string Id
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged("Id");
            }
        }

        public StateViewModel SelectedStateViewModel
        {
            get { return selectedStateViewModel; }
            set
            {
                if (value == null)
                    return;
                selectedStateViewModel = value;
                if (ViewModel.Current != null)
                    ViewModel.Current.SelectedStateViewModel = selectedStateViewModel;
                OnPropertyChanged("SelectedStateViewModel");
            }
        }

        public ObservableCollection<StateViewModel> StatesViewModel
        {
            get { return statesViewModel; }
            set
            {
                statesViewModel = value;
                OnPropertyChanged("StatesViewModel");
            }
        }

        public ObservableCollection<Canvas> StatesPicture
        {
            get { return statesPicture; }
            set
            {
                statesPicture = value;
                OnPropertyChanged("StatesPicture");
            }
        }

        private void OnAddDeviceCommand(object obj)
        {
            var devicesListView = new ListView();
            var devicesListViewModel = new DevicesListViewModel();
            devicesListView.DataContext = devicesListViewModel;
            devicesListView.ShowDialog();
        }

        private void OnAddStateCommand(object obj)
        {
            var statesListView = new ListView();
            var statesListViewModel = new StatesListViewModel();
            statesListView.DataContext = statesListViewModel;
            statesListView.ShowDialog();
        }

        private void OnRemoveDeviceCommand(object obj)
        {
            ViewModel.Current.DeviceViewModels.Remove(this);
        }
    }
}