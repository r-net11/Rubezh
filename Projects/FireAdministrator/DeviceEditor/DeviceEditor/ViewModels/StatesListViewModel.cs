using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using System.IO;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using Firesec.Metadata;
using Resources;

namespace DeviceEditor
{
    class StatesListViewModel:BaseViewModel
    {
        public StatesListViewModel()
        {
            Current = this;
            LoadStates();
            AddCommand = new RelayCommand(OnAddCommand);
        }
        public static StatesListViewModel Current;
        public RelayCommand AddCommand { get; private set; }
        void OnAddCommand(object obj)
        {
            StateViewModel stateViewModel = new StateViewModel();
            stateViewModel.Id = selectedItem.Id;
            stateViewModel.IsAdditional = selectedItem.IsAdditional;
            stateViewModel.ParentDevice = ViewModel.Current.SelectedDeviceViewModel;
            FrameViewModel frameViewModel = new FrameViewModel();
            frameViewModel.Duration = 500;
            frameViewModel.Image = "<svg width=\"500\" height=\"500\" xmlns:xlink=\"http://www.w3.org/1999/xlink\" xmlns=\"http://www.w3.org/2000/svg\">\n<g>\n<title>Layer</title>\n</g>\n</svg>";
            stateViewModel.FrameViewModels = new ObservableCollection<FrameViewModel>();
            stateViewModel.FrameViewModels.Add(frameViewModel);
            ViewModel.Current.SelectedDeviceViewModel.StatesViewModel.Add(stateViewModel);
            Items.Remove(selectedItem);
        }

        /// <summary>
        /// Заголовок окна - "Список состояний".
        /// </summary>
        string title = "Список состояний";
        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                OnPropertyChanged("Title");
            }
        }

        StateViewModel selectedItem;
        public StateViewModel SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
        }

        ObservableCollection<StateViewModel> items;
        public ObservableCollection<StateViewModel> Items
        {
            get { return items; }
            set
            {
                items = value;
                OnPropertyChanged("Items");
            }
        }

        public void LoadStates()
        {
            FileStream file_xml = new FileStream(References.metadata_xml, FileMode.Open, FileAccess.Read, FileShare.Read);
            XmlSerializer serializer = new XmlSerializer(typeof(config));
            config metadata = (config)serializer.Deserialize(file_xml);
            file_xml.Close();

            Items = new ObservableCollection<StateViewModel>();
            drvType driver = metadata.drv.FirstOrDefault(x => x.name == ViewModel.Current.SelectedDeviceViewModel.Id);
            foreach (stateType item in driver.state)
                try
                {
                    StateViewModel stateViewModel = new StateViewModel();
                    stateViewModel.Id = item.name;
                    stateViewModel.IsAdditional = true;
                    if (ViewModel.Current.SelectedDeviceViewModel.StatesViewModel.FirstOrDefault(x=>x.Id == stateViewModel.Id) == null)
                        Items.Add(stateViewModel);
                }
                catch { }
        }
    }
}
