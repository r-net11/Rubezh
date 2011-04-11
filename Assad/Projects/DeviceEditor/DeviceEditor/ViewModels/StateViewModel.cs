using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Common;
using System.Windows.Controls;
using RubezhDevices;
using System.IO;
using System.Xml;
using System.Windows.Markup;
using System.Windows.Input;
using Firesec.Metadata;

namespace DeviceEditor
{
    public class StateViewModel : BaseViewModel
    {

        public StateViewModel()
        {
            Current = this;
            Parent = DeviceViewModel.Current;
            RemoveStateCommand = new RelayCommand(OnRemoveStateCommand);
        }
        public DeviceViewModel Parent { get; set; }

        FrameViewModel selectedFrameViewModel;
        public FrameViewModel SelectedFrameViewModel
        {
            get { return selectedFrameViewModel; }
            set
            {
                selectedFrameViewModel = value;
                if (selectedFrameViewModel == null)
                    return;
                OnPropertyChanged("SelectedFrameViewModel");
            }
        }

        public RelayCommand RemoveStateCommand { get; private set; }
        void OnRemoveStateCommand(object obj)
        {
            ViewModel.Current.SelectedDeviceViewModel.StateViewModels.Remove(this);
            StateViewModel stateViewModel = new StateViewModel();
            stateViewModel.Id = this.Id;
            DeviceViewModel.Current.StatesListViewModel.Add(stateViewModel);
        }

        public static StateViewModel Current { get; private set; }
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

        public ObservableCollection<FrameViewModel> frameViewModels;
        public ObservableCollection<FrameViewModel> FrameViewModels
        {
            get { return frameViewModels; }
            set
            {
                frameViewModels = value;
                OnPropertyChanged("FrameViewModels");
            }
        }

    }
}
