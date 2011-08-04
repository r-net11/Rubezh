using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LibraryModule.ViewModels;

namespace LibraryModule.Views
{
    public partial class LibraryTree : UserControl, INotifyPropertyChanged
    {
        public LibraryTree()
        {
            InitializeComponent();
        }

        public DeviceViewModel SelectedDevice
        {
            get
            {
                return DevicesTree.SelectedItem as DeviceViewModel;
            }
            set { }
        }

        public StateViewModel SelectedState
        {
            get
            {
                return DevicesTree.SelectedItem as StateViewModel;
            }
            set { }
        }

        private void LibraryTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var treeView = sender as TreeView;
            if ((treeView.SelectedItem as DeviceViewModel) != null)
            {
                (DataContext as LibraryViewModel).SelectedDeviceViewModel = treeView.SelectedItem as DeviceViewModel;
            }
            if ((treeView.SelectedItem as StateViewModel) != null)
            {
                var stateViewModel = treeView.SelectedItem as StateViewModel;
                var library = (DataContext as LibraryViewModel);
                var parentDevice =
                    library.DeviceViewModels.First(x => x.Driver.Id == stateViewModel.ParentDriver.Id);

                if (library.SelectedDeviceViewModel != parentDevice)
                {
                    library.SelectedDeviceViewModel = parentDevice;
                }
                parentDevice.SelectedStateViewModel = stateViewModel;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
