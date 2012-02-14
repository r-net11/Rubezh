using System.Windows.Controls;
using Controls;
using System.Windows.Input;

namespace DevicesModule.Views
{
    public partial class ZonesView : UserControl
    {
        public ZonesView()
        {
            InitializeComponent();
            this.Loaded += new System.Windows.RoutedEventHandler(OnLoaded);

            _scrollViewer.PreviewMouseDown += OnMouseMiddleDown;
            _scrollViewer.PreviewMouseUp += OnMouseMiddleUp;
            _scrollViewer.PreviewMouseMove += OnMiddleMouseMove;
            _scrollViewer.MouseLeave += OnMiddleMouseLeave;
        }

        void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (_zonesListBox.SelectedItem != null)
                _zonesListBox.ScrollIntoView(_zonesListBox.SelectedItem);
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
    }
}