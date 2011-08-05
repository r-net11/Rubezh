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
        public const string DefaultClassId = "8";

        public StateViewModel(
            DeviceLibrary.Models.State state, FiresecAPI.Models.Driver parentDriver)
        {
            State = state;
            if (state.Frames == null)
            {
                SetDefaultFrameTo(State);
            }
            ParentDriver = parentDriver;

            AddFrameCommand = new RelayCommand(OnAddFrame);
            RemoveFrameCommand = new RelayCommand(OnRemoveFrame);

            Initialize();
        }

        void Initialize()
        {
            FrameViewModels = new ObservableCollection<FrameViewModel>();
            foreach (var frame in State.Frames)
            {
                FrameViewModels.Add(new FrameViewModel(frame));
            }
            SelectedFrameViewModel = FrameViewModels[0];
        }

        public DeviceLibrary.Models.State State { get; private set; }
        public FiresecAPI.Models.Driver ParentDriver { get; private set; }

        public bool IsChecked { get; set; }

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

        ObservableCollection<FrameViewModel> _frameViewModels;
        public ObservableCollection<FrameViewModel> FrameViewModels
        {
            get { return _frameViewModels; }
            set
            {
                _frameViewModels = value;
                OnPropertyChanged("FrameViewModels");
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

        public static DeviceLibrary.Models.State GetDefaultStateWith(
            string classId = DefaultClassId, string code = null)
        {
            var state = new DeviceLibrary.Models.State();
            state.Class = classId;
            state.Code = code;
            SetDefaultFrameTo(state);

            return state;
        }

        public DeviceLibrary.Models.State GetModel()
        {
            State.Frames = new List<DeviceLibrary.Models.Frame>();
            foreach (var frameViewModel in FrameViewModels)
            {
                State.Frames.Add(frameViewModel.Frame);
            }

            return State;
        }

        public RelayCommand AddFrameCommand { get; private set; }
        void OnAddFrame()
        {
            FrameViewModels.Add(
                new FrameViewModel(FrameViewModel.GetDefaultFrameWith(FrameViewModels.Count)));
        }

        public RelayCommand RemoveFrameCommand { get; private set; }
        void OnRemoveFrame()
        {
            if (FrameViewModels.Count == 1)
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
                    FrameViewModels.Remove(SelectedFrameViewModel);
                }
            }
        }
    }
}
