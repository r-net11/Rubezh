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
        public const StateType DefaultStateType = StateType.No;

        public StateViewModel(
            FiresecAPI.Models.DeviceLibrary.State state, FiresecAPI.Models.Driver parentDriver)
        {
            State = state;
            if (state.Frames == null)
            {
                SetDefaultFrameTo(State);
            }
            ParentDriver = parentDriver;

            AddFrameCommand = new RelayCommand(OnAddFrame);
            RemoveFrameCommand = new RelayCommand(OnRemoveFrame, CanRemoveFrame);

            Initialize();
        }

        void Initialize()
        {
            FrameViewModels = new ObservableCollection<FrameViewModel>();
            State.Frames.ForEach(frame => FrameViewModels.Add(new FrameViewModel(frame)));
            SelectedFrameViewModel = FrameViewModels[0];
        }

        public FiresecAPI.Models.DeviceLibrary.State State { get; private set; }
        public FiresecAPI.Models.Driver ParentDriver { get; private set; }

        public bool IsChecked { get; set; }

        public bool IsAdditional
        {
            get { return State.Code != null; }
        }

        public string ClassName
        {
            get { return EnumsConverter.StateTypeToClassName(State.StateType); }
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

        public ObservableCollection<FrameViewModel> FrameViewModels { get; private set; }

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

        public static void SetDefaultFrameTo(FiresecAPI.Models.DeviceLibrary.State state)
        {
            state.Frames = new List<FiresecAPI.Models.DeviceLibrary.Frame>();
            state.Frames.Add(FrameViewModel.GetDefaultFrameWith(state.Frames.Count));
        }

        public static FiresecAPI.Models.DeviceLibrary.State GetDefaultStateWith(
            StateType stateType = DefaultStateType,
            string code = null)
        {
            var state = new FiresecAPI.Models.DeviceLibrary.State();
            state.StateType = stateType;
            state.Code = code;
            SetDefaultFrameTo(state);

            return state;
        }

        public RelayCommand AddFrameCommand { get; private set; }
        void OnAddFrame()
        {
            var defaultFrame = FrameViewModel.GetDefaultFrameWith(FrameViewModels.Count);
            State.Frames.Add(defaultFrame);
            FrameViewModels.Add(new FrameViewModel(defaultFrame));
        }

        public RelayCommand RemoveFrameCommand { get; private set; }
        void OnRemoveFrame()
        {
            var result = MessageBox.Show("Удалить выбранный кадр?",
                                        "Окно подтверждения",
                                        MessageBoxButton.OKCancel,
                                        MessageBoxImage.Question);

            if (result == MessageBoxResult.OK)
            {
                State.Frames.Remove(SelectedFrameViewModel.Frame);
                FrameViewModels.Remove(SelectedFrameViewModel);
            }
        }

        bool CanRemoveFrame(object obj)
        {
            return SelectedFrameViewModel != null && FrameViewModels.Count > 1;
        }
    }
}