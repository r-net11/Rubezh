using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Infrastructure.Common;

namespace LibraryModule.ViewModels
{
    public class StateViewModel : BaseViewModel
    {
        public StateViewModel(DeviceLibrary.Models.State state, DeviceViewModel parent)
        {
            SetFrames(state);
            Parent = parent;
            IsAdditional = state.IsAdditional;
            ConfigDrvState = Parent.Driver.States.FirstOrDefault(x => x.name == state.Name);
            //if (IsAdditional)
            //{
            //    Id = ConfigDrvState.id;
            //}
            //else
            //{
            Id = state.Id;
            //}

            Initialize();
        }

        public StateViewModel(Firesec.Metadata.configDrvState configDrvState, DeviceViewModel parent)
        {
            SetDefaultFrame();
            Parent = parent;
            IsAdditional = true;
            ConfigDrvState = configDrvState;
            Id = configDrvState.id;

            Initialize();
        }

        public StateViewModel(string id, DeviceViewModel parent)
        {
            SetDefaultFrame();
            Parent = parent;
            IsAdditional = false;
            Id = id;

            Initialize();
        }

        void Initialize()
        {
            RemoveStateCommand = new RelayCommand(OnRemoveState);
        }

        public Firesec.Metadata.configDrvState ConfigDrvState { get; private set; }
        public DeviceViewModel Parent { get; private set; }

        string _id;
        public string Id
        {
            get
            {
                return _id;
            }

            private set
            {
                _id = value;
            }
        }

        bool _isAdditional;
        public bool IsAdditional
        {
            get
            {
                return _isAdditional;
            }

            private set
            {
                _isAdditional = value;
            }
        }

        bool _isChecked;
        public bool IsChecked
        {
            get
            {
                return _isChecked;
            }

            set
            {
                _isChecked = value;
                if (Parent.SelectedState != this)
                {
                    Parent.SelectedState = this;
                }
                if (value)
                {
                    Parent.AdditionalStates.Add(Id);
                }
                else
                {
                    Parent.AdditionalStates.Remove(Id);
                }

                OnPropertyChanged("IsChecked");
            }
        }

        public string Class
        {
            get
            {
                if (Parent == null) return "";
                var state = Parent.Driver.States.FirstOrDefault(x => x.id == Id);
                if (state == null) return "";
                return state.@class;
            }
        }

        public string ClassName
        {
            get
            {
                return new FiresecClient.Models.State(int.Parse(Class)).ToString();
            }
        }

        public string Name
        {
            get
            {
                if (Parent == null) return "";
                if (IsAdditional)
                {
                    var state = Parent.Driver.States.FirstOrDefault(x => x.id == Id);
                    if (state == null) return "Unknown";
                    return state.name;
                }
                else
                {
                    return new FiresecClient.Models.State(int.Parse(Id)).ToString();
                }
            }
        }

        ObservableCollection<FrameViewModel> _frames;
        public ObservableCollection<FrameViewModel> Frames
        {
            get
            {
                return _frames;
            }

            set
            {
                _frames = value;
                OnPropertyChanged("Frames");
            }
        }

        FrameViewModel _selectedFrame;
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

        void SetDefaultFrame()
        {
            Frames = new ObservableCollection<FrameViewModel>();
            Frames.Add(new FrameViewModel(this));
        }

        void SetFrames(DeviceLibrary.Models.State state)
        {
            Frames = new ObservableCollection<FrameViewModel>();
            foreach (var frame in state.Frames)
            {
                Frames.Add(new FrameViewModel(this, frame));
            }
        }

        public RelayCommand RemoveStateCommand { get; private set; }
        void OnRemoveState()
        {
            if (Name == "Базовый рисунок")
            {
                MessageBox.Show("Невозможно удалить базовый рисунок");
            }
            else
            {
                var dialogResult = MessageBox.Show("Удалить выбранное состояние?",
                          "Окно подтверждения", MessageBoxButton.OKCancel,
                          MessageBoxImage.Question);
                if (dialogResult == MessageBoxResult.OK)
                {
                    IsChecked = false;
                    Parent.States.Remove(this);
                    SelectedFrame = null;
                    Parent.DeviceControl.StateCanvases = null;
                }
            }
        }
    }
}
