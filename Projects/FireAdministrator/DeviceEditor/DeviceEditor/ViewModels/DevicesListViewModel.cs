using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Common;
using Firesec.Metadata;
using Resources;

namespace DeviceEditor
{
    internal class DevicesListViewModel : BaseViewModel
    {
        private ObservableCollection<DeviceViewModel> items;
        private DeviceViewModel selectedItem;

        /// <summary>
        /// Заголовок окна - "Список устройств".
        /// </summary>
        private string title = "Список устройств";

        public DevicesListViewModel()
        {
            LoadMetadata();
            AddCommand = new RelayCommand(OnAddCommand);
        }

        public RelayCommand AddCommand { get; private set; }

        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                OnPropertyChanged("Title");
            }
        }

        public ObservableCollection<DeviceViewModel> Items
        {
            get { return items; }
            set
            {
                items = value;
                OnPropertyChanged("Items");
            }
        }

        public DeviceViewModel SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
        }

        private void OnAddCommand(object obj)
        {
            var deviceViewModel = new DeviceViewModel();
            deviceViewModel.StatesViewModel = new ObservableCollection<StateViewModel>();
            deviceViewModel.Id = selectedItem.Id;
            deviceViewModel.IconPath = selectedItem.IconPath;
            deviceViewModel.StatesViewModel = new ObservableCollection<StateViewModel>();
            LoadBaseStates(deviceViewModel);
            ViewModel.Current.DeviceViewModels.Add(deviceViewModel);
            items.Remove(selectedItem);
        }

        /// <summary>
        /// Загрузка основный состояний.
        /// </summary>
        public void LoadBaseStates(DeviceViewModel deviceViewModel)
        {
            var stateViewModel1 = new StateViewModel();
            var stateViewModel2 = new StateViewModel();
            var stateViewModel3 = new StateViewModel();
            var stateViewModel4 = new StateViewModel();
            var stateViewModel5 = new StateViewModel();
            var stateViewModel6 = new StateViewModel();
            var stateViewModel7 = new StateViewModel();
            var stateViewModel8 = new StateViewModel();
            var stateViewModel9 = new StateViewModel();

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
                var frameViewModel = new FrameViewModel();
                frameViewModel.Duration = 0;
                frameViewModel.Image =
                    "<svg width=\"500\" height=\"500\" xmlns:xlink=\"http://www.w3.org/1999/xlink\" xmlns=\"http://www.w3.org/2000/svg\">\n<g>\n<title>Layer</title>\n</g>\n</svg>";
                stateViewModel.FrameViewModels = new ObservableCollection<FrameViewModel>();
                stateViewModel.FrameViewModels.Add(frameViewModel);
                stateViewModel.FrameViewModels = new ObservableCollection<FrameViewModel>();
                stateViewModel.FrameViewModels.Add(frameViewModel);
            }
        }

        public void LoadMetadata()
        {
            var file_xml = new FileStream(References.metadata_xml, FileMode.Open, FileAccess.Read, FileShare.Read);
            var serializer = new XmlSerializer(typeof (config));
            var metadata = (config) serializer.Deserialize(file_xml);
            file_xml.Close();

            Items = new ObservableCollection<DeviceViewModel>();
            foreach (drvType item in metadata.drv)
                try
                {
                    if (item.options.Contains("Placeable") &&
                        (ViewModel.Current.DeviceViewModels.FirstOrDefault(x => x.Id == item.name) == null))
                    {
                        var deviceViewModel = new DeviceViewModel();
                        deviceViewModel.Id = item.name;
                        deviceViewModel.IconPath = @"C:/Program Files/Firesec/Icons/" + item.dev_icon + ".ico";
                        Items.Add(deviceViewModel);
                    }
                }
                catch
                {
                }
        }
    }
}