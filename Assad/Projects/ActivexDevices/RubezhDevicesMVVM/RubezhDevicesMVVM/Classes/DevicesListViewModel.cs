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
        public RelayCommand AddCommand { get; private set; }
        void OnAddCommand(object obj)
        {
            DeviceViewModel deviceViewModel = new DeviceViewModel();
            deviceViewModel.StatesViewModel = new ObservableCollection<StateViewModel>();
            deviceViewModel.Id = selectedItem.Id;
            deviceViewModel.IconPath = selectedItem.IconPath;
            deviceViewModel.StatesViewModel = new ObservableCollection<StateViewModel>();
            LoadBaseStates(deviceViewModel);
            ViewModel.Current.DeviceViewModels.Add(deviceViewModel);
            items.Remove(selectedItem);
        }

        /// <summary>
        /// Заголовок окна - "Список устройств".
        /// </summary>
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

        /// <summary>
        /// Загрузка основный состояний.
        /// </summary>
        public void LoadBaseStates(DeviceViewModel deviceViewModel)
        {
            StateViewModel stateViewModel1 = new StateViewModel();
            StateViewModel stateViewModel2 = new StateViewModel();
            StateViewModel stateViewModel3 = new StateViewModel();
            StateViewModel stateViewModel4 = new StateViewModel();
            StateViewModel stateViewModel5 = new StateViewModel();
            StateViewModel stateViewModel6 = new StateViewModel();
            StateViewModel stateViewModel7 = new StateViewModel();
            StateViewModel stateViewModel8 = new StateViewModel();
            StateViewModel stateViewModel9 = new StateViewModel();

            stateViewModel1.Id = "Базовый рисунок";
            stateViewModel2.Id = "Тревога";
            stateViewModel3.Id = "Внимание (предтревожное)";
            stateViewModel4.Id = "Неисправность";
            stateViewModel5.Id = "Требуется обслуживание";
            stateViewModel6.Id = "Обход устройств";
            stateViewModel7.Id = "Неизвестно";
            stateViewModel8.Id = "Норма(*)";
            stateViewModel9.Id = "Норма";
            deviceViewModel.StatesViewModel.Add(stateViewModel1);
            deviceViewModel.StatesViewModel.Add(stateViewModel2);
            deviceViewModel.StatesViewModel.Add(stateViewModel3);
            deviceViewModel.StatesViewModel.Add(stateViewModel4);
            deviceViewModel.StatesViewModel.Add(stateViewModel5);
            deviceViewModel.StatesViewModel.Add(stateViewModel6);
            deviceViewModel.StatesViewModel.Add(stateViewModel7);
            deviceViewModel.StatesViewModel.Add(stateViewModel8);
            deviceViewModel.StatesViewModel.Add(stateViewModel9);

            foreach (StateViewModel stateViewModel in deviceViewModel.StatesViewModel)
            {
                FrameViewModel frameViewModel = new FrameViewModel();
                frameViewModel.Duration = 0;
                frameViewModel.Image = "<svg width=\"500\" height=\"500\" xmlns:xlink=\"http://www.w3.org/1999/xlink\" xmlns=\"http://www.w3.org/2000/svg\">\n<g>\n<title>Layer</title>\n</g>\n</svg>";
                stateViewModel.FrameViewModels = new ObservableCollection<FrameViewModel>();
                stateViewModel.FrameViewModels.Add(frameViewModel);
                stateViewModel.FrameViewModels = new ObservableCollection<FrameViewModel>();
                stateViewModel.FrameViewModels.Add(frameViewModel);
            }
        }

        public void LoadMetadata()
        {
            FileStream file_xml = new FileStream(ViewModel.metadata_xml, FileMode.Open, FileAccess.Read, FileShare.Read);
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
