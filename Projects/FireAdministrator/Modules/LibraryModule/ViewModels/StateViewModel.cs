using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Firesec.Metadata;
using FiresecClient;
using Infrastructure.Common;

namespace LibraryModule.ViewModels
{
    public class StateViewModel : BaseViewModel
    {
        public static StateViewModel Current { get; private set; }
                
        public StateViewModel(string id, DeviceViewModel parentDevice, bool isAdditional)
        {
            ParentDevice = parentDevice;
            IsAdditional = isAdditional;
            Id = id;

            Initialize();
        }

        void Initialize()
        {
            Current = this;
            Frames = new ObservableCollection<FrameViewModel>();

            RemoveStateCommand = new RelayCommand(OnRemoveState);
            ShowStatesCommand = ParentDevice.ShowStatesCommand;
            ShowAdditionalStatesCommand = ParentDevice.ShowAdditionalStatesCommand;
        }

        public DeviceViewModel ParentDevice { get; set; }

        private FrameViewModel _selectedFrame;
        public FrameViewModel SelectedFrame
        {
            get
            {
                return _selectedFrame;
            }

            set
            {
                _selectedFrame = value;
                OnPropertyChanged("SelectedFrame");
            }
        }

        private bool _isChecked;
        public bool IsChecked
        {
            get
            {
                return _isChecked;
            }

            set
            {
                _isChecked = value;
                if (_isChecked)
                {
                    ParentDevice.AdditionalStates.Add(Id);
                }
                else
                {
                    ParentDevice.AdditionalStates.Remove(Id);
                }

                //if (LibraryViewModel.Current.SelectedState.IsAdditional == false)
                //{
                //    var tempAstate = new List<string>();
                //    foreach (var stateId in LibraryViewModel.Current.SelectedState.ParentDevice.AdditionalStates)
                //    {
                //        var state = ParentDevice.States.FirstOrDefault(x => (x.Id == stateId) && (x.IsAdditional));
                //        if (state.Class == LibraryViewModel.Current.SelectedState.Id)
                //            tempAstate.Add(state.Id);
                //    }
                //    LibraryViewModel.Current.SelectedState.ParentDevice.DeviceControl.AdditionalStates = tempAstate;
                //}

                OnPropertyChanged("IsChecked");
            }
        }

        private string _class;
        public string Class
        {
            get
            {
                if (!IsAdditional)
                    return "";
                var driver = FiresecManager.Configuration.Drivers.FirstOrDefault(
                    x => x.Id == ParentDevice.Id);
                if (driver == null) return "";
                var state = driver.States.FirstOrDefault(x => x.id == Id);
                if (state == null)
                {
                    return "";
                }
                return state.@class;
            }

            set
            {
                _class = value;
                OnPropertyChanged("Class");
            }
        }

        private string _className;
        public string ClassName
        {
            get
            {
                _className = "";
                if (string.IsNullOrWhiteSpace(Class) == false)
                {
                    _className = Helper.BaseStatesList[Convert.ToInt16(Class)];
                }
                return _className;
            }
            set
            {
                _className = value;
                OnPropertyChanged("ClassName");
            }
        }

        private string _iconPath;
        public string IconPath
        {
            get { return _iconPath; }
            set
            {
                _iconPath = value;
                OnPropertyChanged("IconPath");
            }
        }

        private string _id;
        public string Id
        {
            get
            {
                return _id;
            }

            set
            {
                _id = value;
                Update();
            }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        private ObservableCollection<FrameViewModel> _frames;
        public ObservableCollection<FrameViewModel> Frames
        {
            get { return _frames; }
            set
            {
                _frames = value;
                OnPropertyChanged("Frames");
            }
        }

        private bool _isAdditional;
        public bool IsAdditional
        {
            get { return _isAdditional; }
            set
            {
                _isAdditional = value;
                OnPropertyChanged("IsAdditional");
            }
        }

        void Update()
        {
            var driver = FiresecManager.Configuration.Drivers.FirstOrDefault(
                x => x.Id == ParentDevice.Id);
            if (driver != null)
            {
                if (IsAdditional)
                {
                    Name = "Имени с id=" + Id + "не существует";
                    configDrvState tmp = driver.States.FirstOrDefault(x => x.id == Id);
                    if (tmp != null)
                    {
                        Name = tmp.name;
                    }
                }
                else
                {
                    Name = Helper.BaseStatesList[Convert.ToInt16(Id)];
                }                        
            }
        }

        public RelayCommand ShowStatesCommand { get; private set; }
        public RelayCommand ShowAdditionalStatesCommand { get; private set; }

        public RelayCommand RemoveStateCommand { get; private set; }
        void OnRemoveState()
        {
            if (Name == "Базовый рисунок")
            {
                MessageBox.Show("Невозможно удалить базовый рисунок");
                return;
            }

            if ((_isAdditional == false) && 
                (ParentDevice.States.Any(x => x.Class == Id)))
            {
                var result = MessageBox.Show(
                    @"Состояние, которое Вы пытаетесь удалить содержит дополнительные состояния.
                      Вы уверены что хотите удалить основное состояние вместе с дополнительными?",
                                          "Окно подтверждения", MessageBoxButton.OKCancel,
                                          MessageBoxImage.Question);
                if (result == MessageBoxResult.Cancel) return;

                StateViewModel state;
                while ((state = ParentDevice.States.FirstOrDefault(x => x.Class == Id)) != null)
                {
                    IsChecked = false;
                    ParentDevice.States.Remove(state);
                    ParentDevice.AdditionalStates.Remove(state.Id);
                }
            }
            else
            {
                var result = MessageBox.Show("Удалить выбранное состояние?",
                          "Окно подтверждения", MessageBoxButton.OKCancel,
                          MessageBoxImage.Question);
                if (result == MessageBoxResult.Cancel) return;
            }

            IsChecked = false;
            ParentDevice.States.Remove(this);
            if (_isAdditional)
                ParentDevice.AdditionalStates.Remove(Id);
            LibraryViewModel.Current.SelectedState = null;
        }
    }
}
