using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Firesec.Metadata;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Serialization;
using Common;

namespace DeviceEditor
{
    class DevicesListViewModel : BaseViewModel
    {
        public DevicesListViewModel()
        {
            LoadMetadata();
            AddCommand = new RelayCommand(OnAddCommand);
        }
        public static string metadata_xml = @"c:\Rubezh\Assad\Projects\Assad\DeviceModelManager\metadata.xml";
        public RelayCommand AddCommand { get; private set; }
        void OnAddCommand(object obj)
        {
            DeviceViewModel deviceViewModel = new DeviceViewModel();
            deviceViewModel.StateViewModels = new ObservableCollection<StateViewModel>();
            deviceViewModel.Id = selectedItem.Id;
            StateViewModel stateViewModel = new StateViewModel();
            stateViewModel.Id = "Базовый рисунок";
            deviceViewModel.StateViewModels = new ObservableCollection<StateViewModel>();
            deviceViewModel.StateViewModels[0] = stateViewModel;
            ViewModel.Current.DeviceViewModels.Add(deviceViewModel);
        }

        ObservableCollection<DeviceViewModel> items;
        public ObservableCollection<DeviceViewModel> Items
        {
            get { return items; }
            set
            {
                items = value;
                OnPropertyChanged("Items");
            }
        }

        DeviceViewModel selectedItem;
        public DeviceViewModel SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
        }

        public void LoadMetadata()
        {
            FileStream file_xml = new FileStream(metadata_xml, FileMode.Open, FileAccess.Read, FileShare.Read);
            XmlSerializer serializer = new XmlSerializer(typeof(config));
            config metadata = (config)serializer.Deserialize(file_xml);
            file_xml.Close();

            Items = new ObservableCollection<DeviceViewModel>();
            foreach (drvType item in metadata.drv)
            {
                DeviceViewModel deviceViewModel = new DeviceViewModel();
                deviceViewModel.Id = item.name;
                Items.Add(deviceViewModel);
            }
        }
    }
}
