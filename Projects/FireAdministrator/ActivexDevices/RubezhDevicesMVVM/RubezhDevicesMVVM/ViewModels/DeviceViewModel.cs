using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Common;
using System.Windows.Threading;
using RubezhDevices;
using System.Windows.Input;
using System.IO;
using System.Xml;
using System.Windows.Controls;
using System.Windows.Markup;

namespace RubezhDevicesMVVM
{
    public class DeviceViewModel : BaseViewModel
    {
        public DeviceViewModel()
        {
            Current = this;
            Parent = ViewModel.Current;
            AddDeviceCommand = new RelayCommand(OnAddDeviceCommand);
            RemoveDeviceCommand = new RelayCommand(OnRemoveDeviceCommand);
            AddStateCommand = new RelayCommand(OnAddStateCommand);
            StatesPicture = new ObservableCollection<Canvas>();
            StatesPicture = new ObservableCollection<Canvas>();
        }
        public static DeviceViewModel Current { get; private set; }
        public ViewModel Parent { get; private set; }

        /// <summary>
        /// Путь к иконке устройства
        /// </summary>
        string iconPath;
        public string IconPath
        {
            get
            {
                return iconPath;
            }
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
        void OnAddDeviceCommand(object obj)
        {
            ListView devicesListView = new ListView();
            DevicesListViewModel devicesListViewModel = new DevicesListViewModel();
            devicesListView.DataContext = devicesListViewModel;
            //devicesListView.ShowDialog();
        }

        /// <summary>
        /// Команда, показывающая список всех доступных состояний.
        /// </summary>
        public RelayCommand AddStateCommand { get; private set; }
        void OnAddStateCommand(object obj)
        {
            ListView statesListView = new ListView();
            StatesListViewModel statesListViewModel = new StatesListViewModel();
            statesListView.DataContext = statesListViewModel;
            //statesListView.ShowDialog();
        }

        /// <summary>
        /// Команда удаляющая устройство из списка.
        /// </summary>
        public RelayCommand RemoveDeviceCommand { get; private set; }
        void OnRemoveDeviceCommand(object obj)
        {
            this.Parent.DevicesViewModel.Remove(this);
        }

        /// <summary>
        /// Идентификатор устройства.
        /// </summary>
        public string id;
        public string Id
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged("Id");
            }
        }

        /// <summary>
        /// Выбранное состояние.
        /// </summary>
        StateViewModel selectedStateViewModel;
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

        /// <summary>
        /// Список всех используемых состояний для данного устройства
        /// </summary>
        public ObservableCollection<StateViewModel> statesViewModel;
        public ObservableCollection<StateViewModel> StatesViewModel
        {
            get { return statesViewModel; }
            set
            {
                statesViewModel = value;
                OnPropertyChanged("StatesViewModel");
            }
        }

        public ObservableCollection<Canvas> statesPicture;
        public ObservableCollection<Canvas> StatesPicture
        {
            get { return statesPicture; }
            set
            {
                statesPicture = value;
                OnPropertyChanged("StatesPicture");
            }
        }
    }
}
