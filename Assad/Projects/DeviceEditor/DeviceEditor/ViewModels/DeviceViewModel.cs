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

namespace DeviceEditor
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
            AdditionalStatesPicture = new ObservableCollection<Canvas>();
            MainStatePicture = new ObservableCollection<Canvas>();
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
        /// Загрузка доступных для выбора состояний в
        /// StatesAvailableListViewModel(список доступных для выбора состояний).
        /// </summary>
        public void LoadStates()
        {
            StateViewModel stateViewModel2 = new StateViewModel();
            StateViewModel stateViewModel3 = new StateViewModel();
            StateViewModel stateViewModel4 = new StateViewModel();
            StateViewModel stateViewModel5 = new StateViewModel();
            StateViewModel stateViewModel6 = new StateViewModel();
            StateViewModel stateViewModel7 = new StateViewModel();
            StateViewModel stateViewModel8 = new StateViewModel();
            StateViewModel stateViewModel9 = new StateViewModel();
            stateViewModel2.Id = "Тревога";
            stateViewModel3.Id = "Внимание (предтревожное)";
            stateViewModel4.Id = "Неисправность";
            stateViewModel5.Id = "Требуется обслуживание";
            stateViewModel6.Id = "Обход устройств";
            stateViewModel7.Id = "Неизвестно";
            stateViewModel8.Id = "Норма(*)";
            stateViewModel9.Id = "Норма";
            StatesAvailableListViewModel = new ObservableCollection<StateViewModel>();
            if (ViewModel.Current.SelectedDeviceViewModel != null)
            {
                if (ViewModel.Current.SelectedDeviceViewModel.StateViewModels.FirstOrDefault(x => x.Id == stateViewModel2.Id) == null)
                    StatesAvailableListViewModel.Add(stateViewModel2);
                if (ViewModel.Current.SelectedDeviceViewModel.StateViewModels.FirstOrDefault(x => x.Id == stateViewModel3.Id) == null)
                    StatesAvailableListViewModel.Add(stateViewModel3);
                if (ViewModel.Current.SelectedDeviceViewModel.StateViewModels.FirstOrDefault(x => x.Id == stateViewModel4.Id) == null)
                    StatesAvailableListViewModel.Add(stateViewModel4);
                if (ViewModel.Current.SelectedDeviceViewModel.StateViewModels.FirstOrDefault(x => x.Id == stateViewModel5.Id) == null)
                    StatesAvailableListViewModel.Add(stateViewModel5);
                if (ViewModel.Current.SelectedDeviceViewModel.StateViewModels.FirstOrDefault(x => x.Id == stateViewModel6.Id) == null)
                    StatesAvailableListViewModel.Add(stateViewModel6);
                if (ViewModel.Current.SelectedDeviceViewModel.StateViewModels.FirstOrDefault(x => x.Id == stateViewModel7.Id) == null)
                    StatesAvailableListViewModel.Add(stateViewModel7);
                if (ViewModel.Current.SelectedDeviceViewModel.StateViewModels.FirstOrDefault(x => x.Id == stateViewModel8.Id) == null)
                    StatesAvailableListViewModel.Add(stateViewModel8);
                if (ViewModel.Current.SelectedDeviceViewModel.StateViewModels.FirstOrDefault(x => x.Id == stateViewModel9.Id) == null)
                    StatesAvailableListViewModel.Add(stateViewModel9);
            }
        }

        /// <summary>
        /// Список доступных для выбора состояний. Для каждого устройства свой список.
        /// Доступные состояния для устройства - все состояния за исключением тех,
        /// которые уже используются на данном устройстве.
        /// </summary> 
        ObservableCollection<StateViewModel> statesAvailableListViewModel;
        public ObservableCollection<StateViewModel> StatesAvailableListViewModel
        {
            get { return statesAvailableListViewModel; }
            set
            {
                statesAvailableListViewModel = value;
                OnPropertyChanged("StatesAvailableListViewModel");
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
            devicesListView.ShowDialog();
        }

        /// <summary>
        /// Команда, показывающая список всех доступных состояний.
        /// </summary>
        public RelayCommand AddStateCommand { get; private set; }
        void OnAddStateCommand(object obj)
        {
            ListView statesListView = new ListView();
            StatesListViewModel statesListViewModel = new StatesListViewModel();
            StatesAvailableListViewModel = new ObservableCollection<StateViewModel>();
            statesListView.DataContext = statesListViewModel;
            statesListView.ShowDialog();
        }

        /// <summary>
        /// Команда удаляющая устройство из списка.
        /// </summary>
        public RelayCommand RemoveDeviceCommand { get; private set; }
        void OnRemoveDeviceCommand(object obj)
        {
            this.Parent.DeviceViewModels.Remove(this);
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
                selectedStateViewModel = value;
                ViewModel.Current.SelectedStateViewModel = selectedStateViewModel;
                OnPropertyChanged("SelectedStateViewModel");
            }
        }

        /// <summary>
        /// Список всех используемых состояний для данного устройства
        /// </summary>
        public ObservableCollection<StateViewModel> stateViewModels;
        public ObservableCollection<StateViewModel> StateViewModels
        {
            get { return stateViewModels; }
            set
            {
                stateViewModels = value;
                OnPropertyChanged("StateViewModels");
            }
        }

        public ObservableCollection<Canvas> additionalStatesPicture;
        public ObservableCollection<Canvas> AdditionalStatesPicture
        {
            get { return additionalStatesPicture; }
            set
            {
                additionalStatesPicture = value;
                OnPropertyChanged("AdditionalStatesPicture");
            }
        }

        public ObservableCollection<Canvas> mainStatePicture;
        public ObservableCollection<Canvas> MainStatePicture
        {
            get { return mainStatePicture; }
            set
            {
                mainStatePicture = value;
                OnPropertyChanged("MainStatePicture");
            }
        }

    }
}
