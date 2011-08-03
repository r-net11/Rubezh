using System.ComponentModel;
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
            if ((e.NewValue as DeviceViewModel) != null && (e.OldValue as DeviceViewModel) != null)
            {
                OnPropertyChanged("SelectedDevice");
            }

            if ((e.NewValue as StateViewModel) != null && (e.OldValue as StateViewModel) != null)
            {
                OnPropertyChanged("SelectedDevice");
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
