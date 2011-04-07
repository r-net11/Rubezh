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
        }

        public ViewModel Parent { get; private set; }
        public static DeviceViewModel Current { get; private set; }

        public RelayCommand AddStateCommand { get; private set; }
        void OnAddStateCommand(object obj)
        {
            ListView statesListView = new ListView();
            StatesListViewModel statesListViewModel = new StatesListViewModel();
            statesListView.DataContext = statesListViewModel;
            statesListView.ShowDialog();
        }

        public RelayCommand AddDeviceCommand { get; private set; }
        void OnAddDeviceCommand(object obj)
        {
            ListView devicesListView = new ListView();
            DevicesListViewModel devicesListViewModel = new DevicesListViewModel();
            devicesListView.DataContext = devicesListViewModel;
            devicesListView.ShowDialog();
            //DeviceViewModel newDevice = new DeviceViewModel();
            //newDevice.Parent = this.Parent;
            //newDevice.Id = "Новое устройство";
            //StateViewModel newState = new StateViewModel();
            //newState.Id = "Новое состояние";
            //FrameViewModel newFrame = new FrameViewModel();
            //newFrame.Duration = 300;
            //newFrame.Image = "<svg width=\"500\" height=\"500\" xmlns:xlink=\"http://www.w3.org/1999/xlink\" xmlns=\"http://www.w3.org/2000/svg\"> <g><title>Layer 1</title></g></svg>";
            //newDevice.StateViewModels = new ObservableCollection<StateViewModel>();
            //newState.FrameViewModels = new ObservableCollection<FrameViewModel>();
            //newState.FrameViewModels.Add(newFrame);
            //newDevice.StateViewModels.Add(newState);
            //this.Parent.DeviceViewModels.Insert(this.Parent.DeviceViewModels.IndexOf(this) + 1, newDevice);
        }

        ObservableCollection<StateViewModel> statesListViewModel;
        public ObservableCollection<StateViewModel> StatesListViewModel
        {
            get { return statesListViewModel; }
            set
            {
                statesListViewModel = value;
                OnPropertyChanged("StatesListViewModel");
            }
        }

        public void LoadStates()
        {
            StatesListViewModel = new ObservableCollection<StateViewModel>();
            StateViewModel stateViewModel1 = new StateViewModel();
            StateViewModel stateViewModel2 = new StateViewModel();
            StateViewModel stateViewModel3 = new StateViewModel();
            StateViewModel stateViewModel4 = new StateViewModel();
            StateViewModel stateViewModel5 = new StateViewModel();
            StateViewModel stateViewModel6 = new StateViewModel();
            StateViewModel stateViewModel7 = new StateViewModel();
            StateViewModel stateViewModel8 = new StateViewModel();
            StateViewModel stateViewModel9 = new StateViewModel();
            stateViewModel1.Id = "Базовый рисунок";
            stateViewModel2.Id = "Тревога";
            stateViewModel3.Id = "Внимание (предтревожное)";
            stateViewModel4.Id = "Неисправность";
            stateViewModel5.Id = "Требуется обслуживание";
            stateViewModel6.Id = "Обход устройств";
            stateViewModel7.Id = "Неизвестно";
            stateViewModel8.Id = "Норма(*)";
            stateViewModel9.Id = "Норма";

            if (ViewModel.Current.SelectedDeviceViewModel != null)
            {
                if (ViewModel.Current.SelectedDeviceViewModel.StateViewModels.FirstOrDefault(x => x.Id == stateViewModel1.Id) == null)
                    StatesListViewModel.Add(stateViewModel1);
                if (ViewModel.Current.SelectedDeviceViewModel.StateViewModels.FirstOrDefault(x => x.Id == stateViewModel2.Id) == null)
                    StatesListViewModel.Add(stateViewModel2);
                if (ViewModel.Current.SelectedDeviceViewModel.StateViewModels.FirstOrDefault(x => x.Id == stateViewModel3.Id) == null)
                    StatesListViewModel.Add(stateViewModel3);
                if (ViewModel.Current.SelectedDeviceViewModel.StateViewModels.FirstOrDefault(x => x.Id == stateViewModel4.Id) == null)
                    StatesListViewModel.Add(stateViewModel4);
                if (ViewModel.Current.SelectedDeviceViewModel.StateViewModels.FirstOrDefault(x => x.Id == stateViewModel5.Id) == null)
                    StatesListViewModel.Add(stateViewModel5);
                if (ViewModel.Current.SelectedDeviceViewModel.StateViewModels.FirstOrDefault(x => x.Id == stateViewModel6.Id) == null)
                    StatesListViewModel.Add(stateViewModel6);
                if (ViewModel.Current.SelectedDeviceViewModel.StateViewModels.FirstOrDefault(x => x.Id == stateViewModel7.Id) == null)
                    StatesListViewModel.Add(stateViewModel7);
                if (ViewModel.Current.SelectedDeviceViewModel.StateViewModels.FirstOrDefault(x => x.Id == stateViewModel8.Id) == null)
                    StatesListViewModel.Add(stateViewModel8);
                if (ViewModel.Current.SelectedDeviceViewModel.StateViewModels.FirstOrDefault(x => x.Id == stateViewModel9.Id) == null)
                    StatesListViewModel.Add(stateViewModel9);
            }
        }

        public RelayCommand RemoveDeviceCommand { get; private set; }
        void OnRemoveDeviceCommand(object obj)
        {
            this.Parent.DeviceViewModels.Remove(this);
        }

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
    }
}
