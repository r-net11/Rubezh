using System.Windows;
using System.Windows.Controls;

namespace Controls
{
    public class ToolBarRadioButton : RadioButton
    {
        static ToolBarRadioButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ToolBarRadioButton), new FrameworkPropertyMetadata(typeof(ToolBarRadioButton)));
        }

        public static readonly DependencyProperty ImageSourceProperty =
			DependencyProperty.Register("ImageSource", typeof(string), typeof(ToolBarRadioButton));

        public string ImageSource
        {
            get { return (string)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }
    }
}
