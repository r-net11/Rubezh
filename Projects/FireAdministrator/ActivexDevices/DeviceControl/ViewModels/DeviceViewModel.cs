using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Common;
using System.Windows.Threading;
using Resources;
using System.Windows.Input;
using System.IO;
using System.Xml;
using System.Windows.Controls;
using System.Windows.Markup;

namespace DevicesControl
{
    public class DeviceViewModel : BaseViewModel
    {
        public DeviceViewModel()
        {
            Current = this;
            StatesPicture = new ObservableCollection<Canvas>();
        }
        public static DeviceViewModel Current { get; private set; }

        /// <summary>
        /// Идентификатор устройства.
        /// </summary>
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

        /// <summary>
        /// Список всех используемых состояний для данного устройства
        /// </summary>
        public ObservableCollection<StateViewModel> statesViewModel;
        public ObservableCollection<StateViewModel> StatesViewModel
        {
            get { return statesViewModel; }
            set
            {
                statesViewModel = value;
                OnPropertyChanged("StatesViewModel");
            }
        }

        public ObservableCollection<Canvas> statesPicture;
        public ObservableCollection<Canvas> StatesPicture
        {
            get { return statesPicture; }
            set
            {
                statesPicture = value;
                OnPropertyChanged("StatesPicture");
            }
        }
    }
}
