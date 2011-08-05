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
            _device = device;
            _driver = FiresecManager.Drivers.First(x => x.Id == _device.Id);
            if (_device.States == null)
            {
                SetDefaultStateTo(_device);
            }

            RemoveStateCommand = new RelayCommand(OnRemoveState);
            AddStateCommand = new RelayCommand(OnAddState);
            AddAdditionalStateCommand = new RelayCommand(OnShowAdditionalStates);

            Initialize();
        }

        void Initialize()
        {
            StateViewModels = new ObservableCollection<StateViewModel>();
            foreach (var state in _device.States)
            {
                StateViewModels.Add(new StateViewModel(state, _driver));
            }
        }

        readonly Driver _driver;
        readonly DeviceLibrary.Models.Device _device;

        public string Name
        {
            get { return _driver.Name; }
        }

        public string ImageSource
        {
            get { return _driver.ImageSource; }
        }

        public string Id
        {
            get { return _device.Id; }
        }

        ObservableCollection<StateViewModel> _stateViewModels;
        public ObservableCollection<StateViewModel> StateViewModels
        {
            get { return _stateViewModels; }
            set
            {
                _stateViewModels = value;
                OnPropertyChanged("StateViewModels");
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
            device.States.Add(StateViewModel.GetDefaultStateWith());
        }

        public static DeviceLibrary.Models.Device GetDefaultDriverWith(string id)
        {
            var device = new DeviceLibrary.Models.Device();
            device.Id = id;
            SetDefaultStateTo(device);

            return device;
        }

        public DeviceLibrary.Models.Device GetModel()
        {
            _device.States = new List<DeviceLibrary.Models.State>();
            foreach (var stateViewModel in StateViewModels)
            {
                _device.States.Add(stateViewModel.GetModel());
            }

            return _device;
        }

        public RelayCommand AddStateCommand { get; private set; }
        void OnAddState()
        {
            var addStateViewModel = new StateDetailsViewModel(_device);
            if (ServiceFactory.UserDialogs.ShowModalWindow(addStateViewModel))
            {
                StateViewModels.Add(addStateViewModel.SelectedItem);
            }
        }

        public RelayCommand AddAdditionalStateCommand { get; private set; }
        void OnShowAdditionalStates()
        {
            var addAdditionalStateViewModel = new AdditionalStateDetailsViewModel(_device);
            if (ServiceFactory.UserDialogs.ShowModalWindow(addAdditionalStateViewModel))
            {
                StateViewModels.Add(addAdditionalStateViewModel.SelectedItem);
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
                    StateViewModels.Remove(SelectedStateViewModel);
                }
            }
        }
    }
}
