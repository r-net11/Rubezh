using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using System.IO;
using System.Collections.ObjectModel;
using Firesec.Metadata;
using System.Xml.Serialization;

namespace DeviceEditor
{
    class StatesListViewModel:BaseViewModel
    {
        public StatesListViewModel()
        {
            Current = this;
            Items = DeviceViewModel.Current.StatesListViewModel;
            AddCommand = new RelayCommand(OnAddCommand);
        }
        public static string metadata_xml = @"c:\Rubezh\Assad\Projects\Assad\DeviceModelManager\metadata.xml";
        public static StatesListViewModel Current;
        public RelayCommand AddCommand { get; private set; }
        void OnAddCommand(object obj)
        {
            StateViewModel stateViewModel = new StateViewModel();
            stateViewModel.Id = selectedItem.Id;
            FrameViewModel frameViewModel = new FrameViewModel();
            frameViewModel.Duration = 500;
            frameViewModel.Image = "<svg width=\"500\" height=\"500\" xmlns:xlink=\"http://www.w3.org/1999/xlink\" xmlns=\"http://www.w3.org/2000/svg\">\n<g>\n<title>Layer</title>\n</g>\n</svg>";
            stateViewModel.FrameViewModels = new ObservableCollection<FrameViewModel>();
            stateViewModel.FrameViewModels.Add(frameViewModel);
            ViewModel.Current.SelectedDeviceViewModel.StateViewModels.Add(stateViewModel);
            DeviceViewModel.Current.StatesListViewModel.Remove(selectedItem);
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

        //public void LoadMetadata()
        //{
        //    FileStream file_xml = new FileStream(metadata_xml, FileMode.Open, FileAccess.Read, FileShare.Read);
        //    XmlSerializer serializer = new XmlSerializer(typeof(config));
        //    config metadata = (config)serializer.Deserialize(file_xml);
        //    file_xml.Close();

        //    foreach (drvType drvs in metadata.drv)
        //    {
        //        if (drvs.name == ViewModel.Current.SelectedDeviceViewModel.Id)
        //        {
        //            Items = new ObservableCollection<stateType>();
        //            foreach (stateType state in drvs.state)
        //            {
        //                Items.Add(state);
        //            }
        //        }
        //    }
        //}
    }
}
