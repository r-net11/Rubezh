using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LibraryModule.ViewModels;

namespace LibraryModule.Views
{
    public partial class LibraryTree : UserControl
    {
        public LibraryTree()
        {
            InitializeComponent();
        }

        void LibraryTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var treeView = sender as TreeView;
            if ((treeView.SelectedItem as DeviceViewModel) != null)
            {
                (DataContext as LibraryViewModel).SelectedDeviceViewModel =
                    treeView.SelectedItem as DeviceViewModel;
            }
            if ((treeView.SelectedItem as StateViewModel) != null)
            {
                var stateViewModel = treeView.SelectedItem as StateViewModel;
                var library = (DataContext as LibraryViewModel);
                var parentDevice =
                    library.DeviceViewModels.First(x => x.Id == stateViewModel.ParentDriver.Id);

                if (library.SelectedDeviceViewModel != parentDevice)
                {
                    library.SelectedDeviceViewModel = parentDevice;
                }
                parentDevice.SelectedStateViewModel = stateViewModel;
            }
        }

        void ContextMenu_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            var treeView = (sender as TreeView);
            var contextMenu = treeView.ContextMenu;
            foreach (MenuItem item in contextMenu.Items)
            {
                item.Visibility =
                    System.Windows.Visibility.Collapsed;
            }
            if (treeView.SelectedItem == null)
            {
                (contextMenu.Items[0] as MenuItem).Visibility =
                    System.Windows.Visibility.Visible;
            }
            if ((treeView.SelectedItem as StateViewModel) != null)
            {
                (contextMenu.Items[contextMenu.Items.Count - 1] as MenuItem).Visibility =
                    System.Windows.Visibility.Visible;
            }
            if ((treeView.SelectedItem as DeviceViewModel) != null)
            {
                for (var i = 0; i < contextMenu.Items.Count - 1; ++i)
                {
                    (contextMenu.Items[i] as MenuItem).Visibility =
                        System.Windows.Visibility.Visible;
                }
            }
        }
    }
}