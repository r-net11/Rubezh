using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Common;
using Firesec.Metadata;
using Resources;

namespace DeviceEditor
{
    internal class StatesListViewModel : BaseViewModel
    {
        public static StatesListViewModel Current;
        private ObservableCollection<StateViewModel> items;
        private StateViewModel selectedItem;

        /// <summary>
        /// Заголовок окна - "Список состояний".
        /// </summary>
        private string title = "Список состояний";

        public StatesListViewModel()
        {
            Current = this;
            LoadStates();
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

        public StateViewModel SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
        }

        public ObservableCollection<StateViewModel> Items
        {
            get { return items; }
            set
            {
                items = value;
                OnPropertyChanged("Items");
            }
        }

        private void OnAddCommand(object obj)
        {
            var stateViewModel = new StateViewModel();
            stateViewModel.Id = selectedItem.Id;
            stateViewModel.IsAdditional = selectedItem.IsAdditional;
            stateViewModel.ParentDevice = ViewModel.Current.SelectedDeviceViewModel;
            var frameViewModel = new FrameViewModel();
            frameViewModel.Duration = 500;
            frameViewModel.Image =
                "<svg width=\"500\" height=\"500\" xmlns:xlink=\"http://www.w3.org/1999/xlink\" xmlns=\"http://www.w3.org/2000/svg\">\n<g>\n<title>Layer</title>\n</g>\n</svg>";
            stateViewModel.FrameViewModels = new ObservableCollection<FrameViewModel>();
            stateViewModel.FrameViewModels.Add(frameViewModel);
            ViewModel.Current.SelectedDeviceViewModel.StatesViewModel.Add(stateViewModel);
            Items.Remove(selectedItem);
        }

        public void LoadStates()
        {
            var file_xml = new FileStream(References.metadata_xml, FileMode.Open, FileAccess.Read, FileShare.Read);
            var serializer = new XmlSerializer(typeof (config));
            var metadata = (config) serializer.Deserialize(file_xml);
            file_xml.Close();

            Items = new ObservableCollection<StateViewModel>();
            drvType driver = metadata.drv.FirstOrDefault(x => x.name == ViewModel.Current.SelectedDeviceViewModel.Id);
            foreach (stateType item in driver.state)
                try
                {
                    var stateViewModel = new StateViewModel();
                    stateViewModel.Id = item.name;
                    stateViewModel.IsAdditional = true;
                    if (
                        ViewModel.Current.SelectedDeviceViewModel.StatesViewModel.FirstOrDefault(
                            x => x.Id == stateViewModel.Id) == null)
                        Items.Add(stateViewModel);
                }
                catch
                {
                }
        }
    }
}