using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Controls
{
    public class XRichTextBox : RichTextBox
    {
        public static readonly DependencyProperty TextProperty =
                DependencyProperty.Register("Text", typeof(string), typeof(XRichTextBox),
                new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnTextPropertyChanged)));

        public string Text
        {
            get { return GetValue(TextProperty) as string; }
            set { SetValue(TextProperty, value); }
        }

        private static void OnTextPropertyChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            XRichTextBox xRichTextBox = dp as XRichTextBox;
            if (xRichTextBox == null)
            {
                return;
            }

            if (e.NewValue != null)
            {
                var text = e.NewValue as string;
                if (!string.IsNullOrEmpty(text))
                {
                    MemoryStream memoryStream = new MemoryStream(ASCIIEncoding.UTF8.GetBytes(text));
                    xRichTextBox.Selection.Load(memoryStream, DataFormats.Rtf); 
                }
            }
            else xRichTextBox.Visibility = Visibility.Collapsed;
        }
    }
}
