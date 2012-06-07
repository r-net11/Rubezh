using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace LibraryModule.ViewModels
{
    public class StateViewModel : BaseViewModel
    {
        public const StateType DefaultStateType = StateType.No;

        public StateViewModel(LibraryState state, Driver parentDriver)
        {
            State = state;
            if (state.Frames == null)
                SetDefaultFrameTo(State);

            ParentDriver = parentDriver;

            FrameViewModels = new ObservableCollection<FrameViewModel>(
                State.Frames.Select(frame => new FrameViewModel(frame))
            );
            SelectedFrameViewModel = FrameViewModels[0];

            AddFrameCommand = new RelayCommand(OnAddFrame);
            RemoveFrameCommand = new RelayCommand(OnRemoveFrame, CanRemoveFrame);
        }

        public LibraryState State { get; private set; }
        public Driver ParentDriver { get; private set; }

        public bool IsChecked { get; set; }

        public bool IsAdditional
        {
            get { return State.Code != null; }
        }

        public string ClassName
        {
            get { return EnumsConverter.StateTypeToLibraryStateName(State.StateType); }
        }

        public string Name
        {
            get
            {
                return IsAdditional ? String.Format("{0}. {1}", ClassName, AdditionalName) : ClassName;
            }
        }

        public string AdditionalName
        {
            get
            {
                var driverState = ParentDriver.States.FirstOrDefault(x => x.Code != null && x.Code == State.Code);
                if (driverState != null)
                {
                    return IsAdditional ? driverState.Name : null;
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

        public static void SetDefaultFrameTo(LibraryState state)
        {
            state.Frames = new List<LibraryFrame>();
            state.Frames.Add(FrameViewModel.GetDefaultFrameWith(state.Frames.Count));
        }

        public static LibraryState GetDefaultStateWith(StateType stateType = DefaultStateType, string code = null)
        {
            var state = new LibraryState();
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

            ServiceFactory.SaveService.LibraryChanged = true;
        }

        public RelayCommand RemoveFrameCommand { get; private set; }
        void OnRemoveFrame()
        {
            var result = MessageBoxService.ShowQuestion("Удалить выбранный кадр?");

            if (result == MessageBoxResult.Yes)
            {
                State.Frames.Remove(SelectedFrameViewModel.Frame);
                FrameViewModels.Remove(SelectedFrameViewModel);

                ServiceFactory.SaveService.LibraryChanged = true;
            }
        }

        bool CanRemoveFrame()
        {
            return SelectedFrameViewModel != null && FrameViewModels.Count > 1;
        }
    }
}