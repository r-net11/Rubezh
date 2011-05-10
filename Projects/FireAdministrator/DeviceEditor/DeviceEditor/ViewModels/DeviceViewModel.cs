using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using Common;
using DeviceControls;
using Firesec;
using ListView = DeviceEditor.Views.ListView;

namespace DeviceEditor.ViewModels
{
    public class DeviceViewModel : BaseViewModel
    {
        private List<string> _additionalStatesViewModel;
        private DeviceControl _deviceControl;
        private string _iconPath;
        private string _id;
        private string _name;
        private ObservableCollection<StateViewModel> _statesViewModel;
        private StateViewModel _selectedStateViewModel;
        public DeviceViewModel()
        {
            Current = this;
            DeviceControl = new DeviceControl();
            ShowDevicesCommand = new RelayCommand(OnShowDevicesCommand);
            RemoveDeviceCommand = new RelayCommand(OnRemoveDeviceCommand);
            ShowStatesCommand = new RelayCommand(OnShowStatesCommand);
            AdditionalStatesViewModel = new List<string>();
        }
        public static DeviceViewModel Current { get; private set; }
        public DeviceControl DeviceControl
        {
            get { return _deviceControl; }
            set
            {
                _deviceControl = value;
                OnPropertyChanged("DeviceControl");
            }
        }
        /// <summary>
        /// Путь к иконке устройства
        /// </summary>
        public string IconPath
        {
            get { return _iconPath; }
            set
            {
                _iconPath = value;
                OnPropertyChanged("IconPath");
            }
        }
        /// <summary>
        /// Команда, показывающая список всех доступных устройств.
        /// </summary>
        public RelayCommand ShowDevicesCommand { get; private set; }
        private static void OnShowDevicesCommand(object obj)
        {
            var devicesListView = new ListView();
            var devicesListViewModel = new DevicesListViewModel();
            devicesListView.DataContext = devicesListViewModel;
            devicesListView.ShowDialog();
        }
        /// <summary>
        /// Команда, показывающая список всех доступных состояний.
        /// </summary>
        public RelayCommand ShowStatesCommand { get; private set; }
        private static void OnShowStatesCommand(object obj)
        {
            var statesListView = new ListView();
            var statesListViewModel = new StatesListViewModel();
            statesListView.DataContext = statesListViewModel;
            statesListView.ShowDialog();
        }
        /// <summary>
        /// Команда удаляющая устройство из списка.
        /// </summary>
        public RelayCommand RemoveDeviceCommand { get; private set; }
        private void OnRemoveDeviceCommand(object obj)
        {
            ViewModel.Current.DeviceViewModels.Remove(this);
        }
        /// <summary>
        /// Идентификатор устройства.
        /// </summary>
        public string Id
        {
            get { return _id; }
            set
            {
                _id = value;
                Name = _name = DriversHelper.GetDriverNameById(_id);
            }
        }
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }
        /// <summary>
        /// Выбранное состояние.
        /// </summary>
        public StateViewModel SelectedStateViewModel
        {
            get { return _selectedStateViewModel; }
            set
            {
                if (value == null)
                    return;
                _selectedStateViewModel = value;
                ViewModel.Current.SelectedStateViewModel = _selectedStateViewModel;
                OnPropertyChanged("SelectedStateViewModel");
            }
        }
        /// <summary>
        /// Список всех используемых состояний для данного устройства
        /// </summary>
        public ObservableCollection<StateViewModel> StatesViewModel
        {
            get { return _statesViewModel; }
            set
            {
                _statesViewModel = value;
                OnPropertyChanged("StatesViewModel");
            }
        }
        public List<string> AdditionalStatesViewModel;

    }
}