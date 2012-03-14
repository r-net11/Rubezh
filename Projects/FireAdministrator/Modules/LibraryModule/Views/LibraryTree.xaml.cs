using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Controls;
using LibraryModule.ViewModels;

namespace LibraryModule.Views
{
    public partial class LibraryTree : UserControl
    {
        public LibraryTree()
        {
            InitializeComponent();
            CommandBinding DeleteCmdBinding = new CommandBinding(ApplicationCommands.Delete, DeleteCmdExecuted, DeleteCmdCanExecuted);
            CommandBindings.Add(DeleteCmdBinding);

            _scrollViewer.PreviewMouseDown += OnMouseMiddleDown;
            _scrollViewer.PreviewMouseUp += OnMouseMiddleUp;
            _scrollViewer.PreviewMouseMove += OnMiddleMouseMove;
            _scrollViewer.PreviewMouseWheel += OnPreviewMouseWheel;
            _scrollViewer.MouseLeave += OnMiddleMouseLeave;
        }

        void DeleteCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            if (treeView.SelectedItem is DeviceViewModel)
                (DataContext as LibraryViewModel).RemoveDeviceCommand.Execute();
            else if (treeView.SelectedItem is StateViewModel)
                (DataContext as LibraryViewModel).SelectedDeviceViewModel.RemoveStateCommand.Execute();
        }

        void DeleteCmdCanExecuted(object target, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            if (treeView.SelectedItem is DeviceViewModel)
                e.CanExecute = (DataContext as LibraryViewModel).RemoveDeviceCommand.CanExecute(null);
            else if (treeView.SelectedItem is StateViewModel)
                e.CanExecute = (DataContext as LibraryViewModel).SelectedDeviceViewModel.RemoveStateCommand.CanExecute(null);
        }

        void LibraryTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var treeView = sender as TreeView;
            if (treeView.SelectedItem is DeviceViewModel)
            {
                (DataContext as LibraryViewModel).SelectedDeviceViewModel = treeView.SelectedItem as DeviceViewModel;
                (DataContext as LibraryViewModel).SelectedDeviceViewModel.SelectedStateViewModel = null;
            }
            if (treeView.SelectedItem is StateViewModel)
            {
                var stateViewModel = treeView.SelectedItem as StateViewModel;
                var library = (DataContext as LibraryViewModel);
                var parentDevice = library.DeviceViewModels.First(x => x.Id == stateViewModel.ParentDriver.UID);

                if (library.SelectedDeviceViewModel != parentDevice)
                    library.SelectedDeviceViewModel = parentDevice;
                parentDevice.SelectedStateViewModel = stateViewModel;
            }
        }

        void ContextMenu_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            var treeView = (sender as TreeView);
            var contextMenu = treeView.ContextMenu;
            foreach (MenuItem item in contextMenu.Items)
                item.Visibility = Visibility.Collapsed;

            if (treeView.SelectedItem == null)
                (contextMenu.Items[0] as MenuItem).Visibility = Visibility.Visible;
            else if ((treeView.SelectedItem as StateViewModel) != null)
                (contextMenu.Items[contextMenu.Items.Count - 1] as MenuItem).Visibility = Visibility.Visible;
            else if ((treeView.SelectedItem as DeviceViewModel) != null)
                for (var i = 0; i < contextMenu.Items.Count - 1; i++)
                    (contextMenu.Items[i] as MenuItem).Visibility = Visibility.Visible;
        }

        void OnMouseMiddleDown(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                MiddleButtonScrollHelper.StartScrolling(_scrollViewer, e);
            }
        }

        void OnMouseMiddleUp(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Released)
            {
                MiddleButtonScrollHelper.StopScrolling();
            }
        }

        void OnMiddleMouseMove(object sender, MouseEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                MiddleButtonScrollHelper.UpdateScrolling(e);
            }
        }

        void OnMiddleMouseLeave(object sender, MouseEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                MiddleButtonScrollHelper.StopScrolling();
            }
        }

        void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                _scrollViewer.LineUp();
            }
            else if (e.Delta < 0)
            {
                _scrollViewer.LineDown();
            }

            e.Handled = true;
        }
    }
}