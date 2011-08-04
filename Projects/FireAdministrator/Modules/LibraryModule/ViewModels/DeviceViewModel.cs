using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;

namespace LibraryModule.ViewModels
{
    public class DeviceViewModel : BaseViewModel
    {
        public DeviceViewModel(DeviceLibrary.Models.Device device)
        {
            Initialize();

            Device.Id = device.Id;
            if (device.States != null)
            {
                Device.States = new List<DeviceLibrary.Models.State>(device.States);
            }
            Driver = FiresecManager.Drivers.FirstOrDefault(x => x.Id == device.Id);
        }
        public DeviceViewModel(Driver driver)
        {
            Initialize();

            Driver = driver;
            Device.Id = driver.Id;
        }
        public void Initialize()
        {
            Device = new DeviceLibrary.Models.Device();
            SetDefaultStateTo(Device);

            RemoveStateCommand = new RelayCommand(OnRemoveState);
            AddStateCommand = new RelayCommand(OnAddState);
            AddAdditionalStateCommand = new RelayCommand(OnShowAdditionalStates);
        }

        public Driver Driver { get; private set; }
        public DeviceLibrary.Models.Device Device { get; private set; }

        public ObservableCollection<StateViewModel> StateViewModels
        {
            get
            {
                var stateViewModels = new ObservableCollection<StateViewModel>();
                foreach (var state in Device.States)
                {
                    stateViewModels.Add(new StateViewModel(state, Driver));
                }

                return stateViewModels;
            }
        }

        StateViewModel _selectedStateViewModel;
        public StateViewModel SelectedStateViewModel
        {
            get { return _selectedStateViewModel; }
            set
            {
                _selectedStateViewModel = value;
                OnPropertyChanged("SelectedStateViewModel");
                OnPropertyChanged("CanvasesPresenter");
            }
        }

        public CanvasesPresenter CanvasesPresenter
        {
            get
            {
                if (SelectedStateViewModel == null) return null;

                var canvasesPresenter = new CanvasesPresenter(SelectedStateViewModel.State);
                if (!SelectedStateViewModel.IsAdditional)
                {
                    foreach (var stateViewModel in StateViewModels)
                    {

                        if (stateViewModel.IsAdditional &&
                            stateViewModel.IsChecked &&
                            stateViewModel.State.Class == SelectedStateViewModel.State.Class)
                        {
                            canvasesPresenter.AddCanvacesFrom(stateViewModel.State);
                        }
                    }
                }

                return canvasesPresenter;
            }
        }


        public static void SetDefaultStateTo(DeviceLibrary.Models.Device device)
        {
            device.States = new List<DeviceLibrary.Models.State>();
            device.States.Add(StateViewModel.GetDefaultState());
        }

        public RelayCommand AddStateCommand { get; private set; }
        void OnAddState()
        {
            var addStateViewModel = new StateDetailsViewModel(Device);
            if (ServiceFactory.UserDialogs.ShowModalWindow(addStateViewModel))
            {
                Device.States.Add(addStateViewModel.SelectedItem.State);
                OnPropertyChanged("StateViewModels");
            }
        }

        public RelayCommand AddAdditionalStateCommand { get; private set; }
        void OnShowAdditionalStates()
        {
            var addAdditionalStateViewModel = new AdditionalStateDetailsViewModel(Device);
            if (ServiceFactory.UserDialogs.ShowModalWindow(addAdditionalStateViewModel))
            {
                Device.States.Add(addAdditionalStateViewModel.SelectedItem.State);
                OnPropertyChanged("StateViewModels");
            }
        }

        public RelayCommand RemoveStateCommand { get; private set; }
        void OnRemoveState()
        {
            if (SelectedStateViewModel.State.Class == StateViewModel.DefaultClassId)
            {
                MessageBox.Show("Невозможно удалить базовый рисунок");
            }
            else
            {
                var dialogResult = MessageBox.Show("Удалить выбранное состояние?",
                                                    "Окно подтверждения",
                                                    MessageBoxButton.OKCancel,
                                                    MessageBoxImage.Question);

                if (dialogResult == MessageBoxResult.OK)
                {
                    Device.States.Remove(SelectedStateViewModel.State);
                    OnPropertyChanged("StateViewModels");
                }
            }
        }
    }
}
