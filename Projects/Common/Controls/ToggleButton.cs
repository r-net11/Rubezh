using System.Windows;
using System.Windows.Controls;

namespace Controls
{
    public class ToggleButton : System.Windows.Controls.Primitives.ToggleButton
    {
        static ToggleButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ToggleButton),
                                                     new FrameworkPropertyMetadata(typeof(ToggleButton)));
        }

        public static readonly DependencyProperty ImageSourceProperty =
    DependencyProperty.Register("ImageSource", typeof(string), typeof(ToggleButton));

        public string ImageSource
        {
            get { return (string)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

    }
}