using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using Common;
using Firesec;

namespace DeviceEditor
{
    public class DeviceViewModel : BaseViewModel
    {
        /// <summary>
        /// Путь к иконке устройства
        /// </summary>
        private string iconPath;


        /// <summary>
        /// Выбранное состояние.
        /// </summary>
        private StateViewModel selectedStateViewModel;
        private List<string> additionalStatesViewModel;
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
            AdditionalStatesViewModel = new List<string>();
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

        string id;
        /// <summary>
        /// Идентификатор устройства.
        /// </summary>
        public string Id
        {
            get { return id; }
            set
            {
                id = value;
                Name = name = DriversHelper.GetDriverNameById(id);
            }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
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

        public List<string> AdditionalStatesViewModel
        {
            get { return additionalStatesViewModel; }
            set
            {
                additionalStatesViewModel = value;
                OnPropertyChanged("AdditionalStatesViewModel");
            }
        }
    }
}