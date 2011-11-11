using System.Windows;
using System.Windows.Media;

namespace Controls
{
    public class DataGridProperties
    {
        public static readonly DependencyProperty HeaderBrushProperty;

        public static Brush GetHeaderBrush(DependencyObject obj)
        {
            return (Brush)obj.GetValue(HeaderBrushProperty);
        }

        public static void SetHeaderBrush(DependencyObject obj, Brush value)
        {
            obj.SetValue(HeaderBrushProperty, value);
        }

        static DataGridProperties()
        {
            var metadata = new FrameworkPropertyMetadata((Brush)null);
            HeaderBrushProperty = DependencyProperty.RegisterAttached("HeaderBrush",
                                                                typeof(Brush),
                                                                typeof(DataGridProperties), metadata);
        }
    }
}
