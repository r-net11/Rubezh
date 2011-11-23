using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PlansModule.ViewModels;

namespace PlansModule.Views
{
    public partial class PlanDesignerView : UserControl
    {
        public static PlanDesignerView Current { get; set; }

        public static void Update()
        {
            if (Current != null)
            {
                Current.slider.Value = 1;
                Current.scrollViewer.ScrollToHorizontalOffset(0);
                Current.scrollViewer.ScrollToVerticalOffset(0);
                Current.slider.Value = 1;
            }
        }

        public PlanDesignerView()
        {
            Current = this;
            InitializeComponent();

            scrollViewer.PreviewMouseWheel += OnPreviewMouseWheel;
            slider.ValueChanged += OnSliderValueChanged;
        }

        void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                slider.Value += 0.2;
            }
            else if (e.Delta < 0)
            {
                slider.Value -= 0.2;
            }

            e.Handled = true;
        }

        private void OnZoomIn(object sender, RoutedEventArgs e)
        {
            slider.Value += 1;
        }

        private void OnZoomOut(object sender, RoutedEventArgs e)
        {
            slider.Value -= 1;
        }

        void OnSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double newZoom = e.NewValue;
            double oldZoom = e.OldValue;

            if (newZoom < 1)
            {
                newZoom = 1 / (- newZoom + 2);
            }

            if (oldZoom < 1)
            {
                oldZoom = 1 / (- oldZoom + 2);
            }

            double deltaZoom = newZoom / oldZoom;

            (DataContext as PlansViewModel).PlanDesignerViewModel.ChangeZoom(newZoom);

            var position = Mouse.GetPosition(grid);

            double newOffsetX = scrollViewer.HorizontalOffset + (position.X - 25) * (deltaZoom - 1) / deltaZoom;
            double newOffsetY = scrollViewer.VerticalOffset + (position.Y - 25) * (deltaZoom - 1) / deltaZoom;

            scrollViewer.ScrollToHorizontalOffset(newOffsetX);
            scrollViewer.ScrollToVerticalOffset(newOffsetY);
        }
    }
}
