using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using FiresecAPI.Models;
using Infrastructure.Common;

namespace LibraryModule.ViewModels
{
    public class StateViewModel : BaseViewModel
    {
        public static readonly string DefaultClassId = "8";

        public StateViewModel(DeviceLibrary.Models.State state, FiresecAPI.Models.Driver parentDriver)
        {
            Initialize(parentDriver);

            State.Class = state.Class;
            State.Code = state.Code;
            if (state.Frames != null)
            {
                State.Frames = new List<DeviceLibrary.Models.Frame>(state.Frames);
            }
        }

        public StateViewModel(InnerState innerState, FiresecAPI.Models.Driver parentDriver)
        {
            Initialize(parentDriver);

            State.Class = innerState.State.Id.ToString();
            State.Code = innerState.Code;
        }

        public StateViewModel(string classId, FiresecAPI.Models.Driver parentDriver)
        {
            Initialize(parentDriver);

            State.Class = classId;
        }

        public void Initialize(FiresecAPI.Models.Driver parentDriver)
        {
            ParentDriver = parentDriver;
            State = new DeviceLibrary.Models.State();
            SetDefaultFrameTo(State);

            AddFrameCommand = new RelayCommand(OnAddFrame);
            RemoveFrameCommand = new RelayCommand(OnRemoveFrame);
        }

        public DeviceLibrary.Models.State State { get; private set; }
        public FiresecAPI.Models.Driver ParentDriver { get; private set; }

        bool isChecked;
        public bool IsChecked
        {
            get
            {
                return isChecked;
            }
            set
            {
                isChecked = value;
            }
        }

        public bool IsAdditional
        {
            get { return State.Code != null; }
        }

        public string ClassName
        {
            get { return new State() { Id = int.Parse(State.Class) }.ToString(); }
        }

        public string Name
        {
            get
            {
                var name = ClassName;
                if (IsAdditional)
                {
                    name = name + "." + AdditionalName;
                }

                return name;
            }
        }

        public string AdditionalName
        {
            get
            {
                if (IsAdditional)
                {
                    return ParentDriver.States.First(x => x.Code == State.Code).Name;
                }

                return null;
            }
        }

        public ObservableCollection<FrameViewModel> FrameViewModels
        {
            get
            {
                var frameViewModels = new ObservableCollection<FrameViewModel>();
                foreach (var frame in State.Frames)
                {
                    frameViewModels.Add(new FrameViewModel(frame));
                }
                SelectedFrameViewModel = frameViewModels[0];

                return frameViewModels;
            }
        }

        FrameViewModel _selectedFrameViewModel;
        public FrameViewModel SelectedFrameViewModel
        {
            get { return _selectedFrameViewModel; }
            set
            {
                _selectedFrameViewModel = value;
                OnPropertyChanged("SelectedFrameViewModel");
            }
        }

        public static void SetDefaultFrameTo(DeviceLibrary.Models.State state)
        {
            state.Frames = new List<DeviceLibrary.Models.Frame>();
            state.Frames.Add(FrameViewModel.GetDefaultFrameWith(state.Frames.Count));
        }

        public static DeviceLibrary.Models.State GetDefaultState()
        {
            var state = new DeviceLibrary.Models.State();
            state.Class = DefaultClassId;
            SetDefaultFrameTo(state);

            return state;
        }

        public RelayCommand AddFrameCommand { get; private set; }
        void OnAddFrame()
        {
            State.Frames.Add(FrameViewModel.GetDefaultFrameWith(State.Frames.Count));
            OnPropertyChanged("FrameViewModels");
        }

        public RelayCommand RemoveFrameCommand { get; private set; }
        void OnRemoveFrame()
        {
            if (State.Frames.Count == 1)
            {
                MessageBox.Show("Невозможно удалить единственный кадр", "Ошибка");
            }
            else
            {
                var result = MessageBox.Show("Удалить выбранный кадр?",
                                            "Окно подтверждения",
                                            MessageBoxButton.OKCancel,
                                            MessageBoxImage.Question);

                if (result == MessageBoxResult.OK)
                {
                    State.Frames.Remove(SelectedFrameViewModel.Frame);
                    OnPropertyChanged("FrameViewModels");
                }
            }
        }
    }
}
