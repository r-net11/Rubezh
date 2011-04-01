using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Common;

namespace DeviceEditor
{
    public class DeviceViewModel : BaseViewModel
    {
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
