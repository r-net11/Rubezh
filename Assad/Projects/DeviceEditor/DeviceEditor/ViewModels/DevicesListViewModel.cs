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
            FrameViewModel frameViewModel = new FrameViewModel();
            frameViewModel.Duration = 0;
            frameViewModel.Image = "<svg width=\"500\" height=\"500\" xmlns:xlink=\"http://www.w3.org/1999/xlink\" xmlns=\"http://www.w3.org/2000/svg\">\n<g>\n<title>Layer</title>\n</g>\n</svg>";
            stateViewModel.FrameViewModels = new ObservableCollection<FrameViewModel>();
            stateViewModel.FrameViewModels.Add(frameViewModel);
            deviceViewModel.StateViewModels = new ObservableCollection<StateViewModel>();
            deviceViewModel.StateViewModels.Add(stateViewModel);
            ViewModel.Current.DeviceViewModels.Add(deviceViewModel);
            items.Remove(selectedItem);
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

        string title = "Список устройств";
        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                OnPropertyChanged("Title");
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
                try
                {
                    if (item.options.Contains("Placeable")&&(ViewModel.Current.DeviceViewModels.FirstOrDefault(x => x.Id == item.name) == null))
                    {
                        DeviceViewModel deviceViewModel = new DeviceViewModel();
                        deviceViewModel.Id = item.name;
                        deviceViewModel.IconPath = @"C:/Program Files/Firesec/Icons/" + item.dev_icon + ".ico";
                        Items.Add(deviceViewModel);
                    }
                }
                catch { }
        }
    }
}
