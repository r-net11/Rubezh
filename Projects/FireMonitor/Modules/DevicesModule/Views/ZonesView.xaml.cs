using System.Windows.Controls;
using System.Windows.Input;
using Controls;

namespace DevicesModule.Views
{
    public partial class ZonesView : UserControl
    {
        public ZonesView()
        {
            InitializeComponent();
            _zonesListBox.SelectionChanged += new SelectionChangedEventHandler(_zonesListBox_SelectionChanged);

            _scrollViewer.PreviewMouseDown += OnMouseMiddleDown;
            _scrollViewer.PreviewMouseUp += OnMouseMiddleUp;
            _scrollViewer.PreviewMouseMove += OnMiddleMouseMove;
            _scrollViewer.MouseLeave += OnMiddleMouseLeave;
        }

        void _zonesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
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